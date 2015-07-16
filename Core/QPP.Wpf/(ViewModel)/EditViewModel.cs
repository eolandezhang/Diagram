using QPP.Command;
using QPP.ComponentModel;
using QPP.ComponentModel.DataAnnotations;
using QPP.Dialog;
using QPP.Modularity;
using QPP.Navigation;
using QPP.Runtime.Serialization;
using QPP.Validation;
using QPP.Wpf.Command;
using QPP.Wpf.ComponentModel;
using QPP.Wpf.UI.Controls.AvalonDock;
using System;
using System.Collections.Generic;

namespace QPP.Wpf
{
    /// <summary>
    /// 編輯視圖模型
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public abstract class EditViewModel<TModel> : ViewModelBase
        where TModel : DataModel, System.ComponentModel.IDataErrorInfo, new()
    {
        QPP.Memento.CommandManager m_CommandManager = new QPP.Memento.CommandManager();
        /// <summary>
        /// 命令管理器，命令模式實現Undo\Redo
        /// </summary>
        public QPP.Memento.CommandManager CommandManager
        {
            get { return m_CommandManager; }
        }
        /// <summary>
        /// 錯誤信息
        /// </summary>
        public string ErrorMessage
        {
            get { return Get<string>("ErrorMessage"); }
            set { Set("ErrorMessage", value); }
        }

        /// <summary>
        /// 是否创建状态
        /// </summary>
        public bool IsCreating
        {
            get { return Get<bool>("IsCreating"); }
            set { Set("IsCreating", value); }
        }

        /// <summary>
        /// 绑定的一条记录
        /// </summary>
        public TModel Model
        {
            get { return Get<TModel>("Model"); }
            set
            {
                var m = Model;

                if (!object.ReferenceEquals(m, value))
                {
                    CommandManager.ClearAll();
                    if (m != null)
                    {
                        value.ValueChanged -= value_ValueChanged;
                        value.StateChanged -= Model_StateChanged;
                    }
                    if (value != null)
                    {
                        value.ValueChanged += value_ValueChanged;
                        value.StateChanged += Model_StateChanged;
                        IsCreating = value.DataState == DataState.Created;
                        value.SetValidator(DataValidator.Create(RuntimeContext.Service.L10N));
                    }
                    OnPropertyChanging("Model");
                    ValueTable["Model"] = value;
                    OnValueChanged("Model", value, m);
                    OnPropertyChanged("Model");
                    RefreshTitle();
                }
            }
        }

        void value_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            OnModelValueChanged(e);
        }
        /// <summary>
        /// Model屬性值發生改后，向CommandManager增加操作記錄
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnModelValueChanged(ValueChangedEventArgs e)
        {
            CommandManager.AddCommand(new QPP.Memento.PropertyChangedCommand(Model, e));
        }

        void Model_StateChanged(object sender, EventArgs e)
        {
            IsCreating = ((TModel)sender).DataState == DataState.Created;
            RefreshTitle();
        }

        protected override void RefreshTitle()
        {
            Title = RuntimeContext.Service.L10N.GetText("Title." + Metadata.Module.TypeName);
            if (Model == null) return;
            if (Model.DataState == DataState.Created)
                Title += " - " + RuntimeContext.Service.L10N.GetText("Create") + " *";
            else
            {
                string modelTitle = GetCaption();
                if (Model.DataState == DataState.Modified)
                    Title += " - " + modelTitle + " *";
                else
                    Title += " - " + modelTitle;
            }
        }

        #region Save

        /// <summary>
        /// 保存命令
        /// </summary>
        [RelayCommand(RelayCommands.Save)]
        public ICommand SaveCommand
        {
            get { return CommandContext.GetCommand(() => SaveCommand, () => ExecuteSave(), CanExecuteSave); }
        }

        /// <summary>
        /// 驗證并保存Model
        /// </summary>
        /// <returns></returns>
        protected virtual bool ExecuteSave()
        {
            try
            {
                using (new StatusBusy(RuntimeContext.Service.L10N.GetText("Status.Saving"), ViewStatus,
                    RuntimeContext.Service.L10N.GetText("Status.Saved")))
                {
                    if (IsValid())
                    {
                        bool isNew = Model.DataState == DataState.Created;
                        if (isNew)
                            OnAdd().CopyTo(Model);
                        else
                            OnUpdate().CopyTo(Model);
                        var msg = new ModifiedMessage<TModel>(this, Model, isNew);
                        OnSaved(msg);
                        return true;
                    }
                }
            }
            catch (QPP.Validation.ValidationException exc)
            {
                ErrorMessage = RuntimeContext.Service.ExceptionHandler.GetErrorInfo(exc);
            }
            return false;
        }
        /// <summary>
        /// 保存成功后
        /// </summary>
        /// <param name="message"></param>
        protected virtual void OnSaved(ModifiedMessage<TModel> message)
        {
            RuntimeContext.Service.Messenger.Send(message);
            Model.ResetState();
            RefreshTitle();
            AddRecent(message);
            RuntimeContext.Service.GetObject<IDialog>().Show(new DialogMessage(Source, RuntimeContext.Service.L10N.GetText("Msg.SaveSuccessfully")));
        }

        /// <summary>
        /// 新增最近修改記錄
        /// </summary>
        /// <param name="message"></param>
        protected virtual void AddRecent(ModifiedMessage<TModel> message)
        {
            Action run = new Action(() =>
            {
                try
                {
                    var data = OnModelSerializing();
                    var uri = Metadata.Uri + "?data=" + data;
                    var hashCode = message.Content.GetHashCode().ToString();
                    RuntimeContext.Service.GetObject<INavigator>().AddModified(Title, hashCode, uri, Metadata.TypeName, ViewType.Wpf);
                }
                catch (Exception exc)
                {
                    RuntimeContext.Service.Logger.Error(exc);
                }
            });
            run.BeginInvoke(new AsyncCallback(e => { }), null);
        }

        protected virtual bool CanExecuteSave()
        {
            if (Model == null) return false;
            return (Model.DataState == DataState.Created && CanCreate)
                || (Model.DataState != DataState.Created && CanUpdate);
        }

        /// <summary>
        /// Do update to database
        /// </summary>
        protected abstract TModel OnUpdate();
        /// <summary>
        /// Do insert into database
        /// </summary>
        /// <returns></returns>
        protected abstract TModel OnAdd();

        /// <summary>
        /// 驗證Model，調用Model.Validate()進行驗證
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValid()
        {
            Model.Validate();
            ErrorMessage = Model.Error;
            return ErrorMessage.IsNullOrEmpty();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual bool CanUpdate
        {
            get { return GetPermission(CommandCode.Update); }
        }

        #endregion

        #region Create

        /// <summary>
        /// 新建命令
        /// </summary>
        [RelayCommand(RelayCommands.Create)]
        public ICommand CreateCommand
        {
            get { return CommandContext.GetCommand(() => CreateCommand, ExecuteCreate, CanExecuteCreate); }
        }

        void ExecuteCreate()
        {
            if (Model != null && Model.DataState != DataState.Normal)
            {
                var r = SaveConfirm();
                if (r == DialogResult.Yes && !ExecuteSave())
                    return;
                if (r == DialogResult.Cancel)
                    return;
            }
            OnCreate();
        }

        bool CanExecuteCreate()
        {
            return CanCreate && Model != null && Model.DataState != DataState.Created;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnCreate()
        {
            Model = new TModel();
            RefreshTitle();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual bool CanCreate
        {
            get { return GetPermission(CommandCode.Create); }
        }

        #endregion

        #region Copy

        /// <summary>
        /// 複製Model命令
        /// </summary>
        [RelayCommand(RelayCommands.Copy)]
        public ICommand CopyCommand
        {
            get { return CommandContext.GetCommand(() => CopyCommand, ExecuteCopy, CanExecuteCopy); }
        }

        bool CanExecuteCopy()
        {
            return CanCreate && Model != null && Model.DataState != DataState.Created;
        }

        void ExecuteCopy()
        {
            if (Model.DataState != DataState.Normal)
            {
                var r = SaveConfirm();
                if (r == DialogResult.Yes && !ExecuteSave())
                    return;
                if (r == DialogResult.Cancel)
                    return;
            }
            OnCopy();
        }

        protected virtual void OnCopy()
        {
            TModel newModel = new TModel();
            Model = CopyModel(newModel);
            RefreshTitle();
        }

        /// <summary>
        /// 重寫此方法可以Copy并修改Model的初始值
        /// </summary>
        protected virtual TModel CopyModel(TModel model)
        {
            Model.CopyTo(model);
            return model;
        }

        #endregion

        #region Undo/Redo Command

        /// <summary>
        /// 撤銷
        /// </summary>
        [RelayCommand(RelayCommands.Undo)]
        public ICommand UndoCommand
        {
            get
            {
                return CommandContext.GetCommand(() => UndoCommand, () =>
                {
                    OnUndo();
                    RefreshTitle();
                }, CanExecuteUndo);
            }
        }

        /// <summary>
        /// 重做
        /// </summary>
        [RelayCommand(RelayCommands.Redo)]
        public ICommand RedoCommand
        {
            get
            {
                return CommandContext.GetCommand(() => RedoCommand, () =>
                {
                    OnRedo();
                    RefreshTitle();
                }, CanExecuteRedo);
            }
        }

        protected virtual void OnUndo()
        {
            CommandManager.Undo();
        }

        protected virtual bool CanExecuteUndo()
        {
            return CommandManager.CanUndo;
        }

        protected virtual void OnRedo()
        {
            CommandManager.Redo();
        }

        protected virtual bool CanExecuteRedo()
        {
            return CommandManager.CanRedo;
        }

        #endregion

        /// <summary>
        /// 打開列表命令
        /// </summary>
        [RelayCommand(RelayCommands.OpenSearch)]
        public ICommand OpenSearchCommand
        {
            get { return CommandContext.GetCommand(() => OpenSearchCommand, OpenSearch); }
        }

        /// <summary>
        /// 重寫此方法，以實現從編輯頁面跳到列表查詢頁面
        /// </summary>
        protected virtual void OpenSearch() { }


        /// <summary>
        /// 保存命令
        /// </summary>
        [RelayCommand(RelayCommands.Refresh)]
        public ICommand RefreshCommand
        {
            get { return CommandContext.GetCommand(() => RefreshCommand, ExecuteRefresh, CanExecuteRefresh); }
        }

        protected virtual void ExecuteRefresh()
        {
            using (new StatusBusy(RuntimeContext.Service.L10N.GetText("Status.Loading"), ViewStatus))
            {
                var data = OnModelSerializing();
                Model = OnModelDeserializing(data);
            }
        }

        protected virtual bool CanExecuteRefresh()
        {
            return Model != null && Model.DataState != DataState.Created;
        }

        protected override void OnDeserializing(SerializationInfo info)
        {
            var data = info.GetString("Model");
            if (data.IsNotEmpty())
                Model = OnModelDeserializing(data);
            if (Model == null)
                OnCreate();
        }

        /// <summary>
        /// 重寫此方法，實現恢復Model
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract TModel OnModelDeserializing(string data);

        /// <summary>
        /// 重寫此方法，實現保存Model信息，用於恢復
        /// </summary>
        /// <returns></returns>
        protected abstract string OnModelSerializing();

        protected override void OnSerializing(SerializationInfo info)
        {
            if (Model != null && Model.DataState != DataState.Created)
                info.AddValue("Model", OnModelSerializing());
        }

        public override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (Model != null && Model.DataState != DataState.Normal)
            {
                RuntimeContainers.Current.Show(this);
                var r = SaveConfirm();
                if (r == DialogResult.Yes && !ExecuteSave())
                    e.Cancel = true;
                if (r == DialogResult.Cancel)
                    e.Cancel = true;
            }
            base.OnClosing(e);
        }

        /// <summary>
        /// 获取模型的标题，显示在视图标题中
        /// </summary>
        protected virtual string GetCaption()
        {
            foreach (var p in typeof(TModel).GetProperties())
                if (Attribute.IsDefined(p, typeof(CaptionAttribute)))
                    return p.GetValue(Model, null).ToSafeString();
            return RuntimeContext.Service.L10N.GetText("Edit");
        }

        DialogResult SaveConfirm()
        {
            var title = "";
            if (Model.DataState == DataState.Created)
                title = " \"" + RuntimeContext.Service.L10N.GetText("Create") + " - "
                    + RuntimeContext.Service.L10N.GetText("Title." + Metadata.Module.TypeName) + "\" ";
            else
                title = " \"" + RuntimeContext.Service.L10N.GetText("Title."
                    + Metadata.Module.TypeName) + " - " + GetCaption() + "\" ";
            DialogResult result = DialogResult.None;
            RuntimeContext.Service.GetObject<IDialog>().Show(new DialogMessage()
            {
                Sender = Source,
                Button = DialogButton.YesNoCancel,
                Caption = RuntimeContext.Service.L10N.GetText("Title.SaveModification"),
                Content = RuntimeContext.Service.L10N.GetText("Msg.SaveModificationConfirm").FormatArgs(title),
                DefaultResult = DialogResult.Yes,
                Icon = DialogImage.Question,
                Callback = (r) => { result = r; }
            });
            return result;
        }

        public override void SetArgs(DataArgs args)
        {
            if ("create".CIEquals(args.Get<string>("action")))
                ExecuteCreate();
            var m = args.Get<TModel>("model");
            if (m != null)
                Model = m;
            var data = args.Get<string>("data");
            if (data.IsNotEmpty())
                Model = OnModelDeserializing(data);
            base.SetArgs(args);
        }
    }
}
