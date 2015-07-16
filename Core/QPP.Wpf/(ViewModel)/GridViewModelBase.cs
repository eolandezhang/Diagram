using QPP.Collections;
using QPP.Command;
using QPP.ComponentModel;
using QPP.Dialog;
using QPP.Modularity;
using QPP.Navigation;
using QPP.Wpf.Command;
using QPP.Wpf.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace QPP.Wpf
{
    public abstract class GridViewModelBase<TModel> : ViewModelBase
        where TModel : IDataModel
    {
        protected override void Initialize()
        {
            base.Initialize();
            IsSearchOnLoad = true;
            FilterCriteria = new FilterCriteria();
            SelectedModels = new BindingCollection<TModel>();
        }


        /// <summary>
        /// 最終用於查詢的字串
        /// </summary>
        protected virtual string QueryString { get; set; }

        protected override void RefreshTitle()
        {
            Title = RuntimeContext.Service.L10N.GetText("Title." + Metadata.Module.TypeName);
        }

        /// <summary>
        /// 当前选中的模型
        /// </summary>
        public TModel SelectedModel
        {
            get { return Get<TModel>("SelectedModel"); }
            set { Set("SelectedModel", value); }
        }

        /// <summary>
        /// 当前选中的模型列表
        /// </summary>
        public BindingCollection<TModel> SelectedModels { get; set; }

        /// <summary>
        /// 查询标准
        /// </summary>
        public virtual FilterCriteria FilterCriteria
        {
            get { return Get<FilterCriteria>("FilterCriteria"); }
            set { Set("FilterCriteria", value); }
        }

        protected virtual void OpenDetails(TModel model)
        {

        }

        #region QueryCommand

        [RelayCommand]
        public ICommand QueryCommand
        {
            get { return CommandContext.GetCommand<object>(() => QueryCommand, ExecuteQuery, CanExecuteQuery); }
        }

        protected virtual void ExecuteQuery(object query)
        {
            QueryString = query.ToSafeString();
            OnSearch();
        }

        protected virtual bool CanExecuteQuery(object query)
        {
            return CanRead;
        }

        #endregion

        #region Load

        public bool IsSearchOnLoad
        {
            get;
            set;
        }

        protected override void OnLoad()
        {
            if (CanRead && IsSearchOnLoad)
                OnSearch();
            base.OnLoad();
        }

        #endregion

        #region ResetFilter

        [RelayCommand(RelayCommands.ResetFilter)]
        public ICommand ResetFilterCommand
        {
            get { return CommandContext.GetCommand(() => ResetFilterCommand, ExecuteResetFilter); }
        }

        protected virtual void ExecuteResetFilter()
        {
            FilterCriteria.Reset();
        }

        #endregion

        #region Create

        [RelayCommand(RelayCommands.Create)]
        public ICommand CreateCommand
        {
            get { return CommandContext.GetCommand(() => CreateCommand, ExecuteCreate, CanExecueteCreate); }
        }

        protected virtual bool CanExecueteCreate()
        {
            return CanCreate;
        }

        protected virtual void ExecuteCreate()
        {
            using (new StatusBusy(RuntimeContext.Service.L10N.GetText("Status.Processing"), ViewStatus))
                OnCreate();
        }

        /// <summary>
        /// 能否创建
        /// </summary>
        protected virtual bool CanCreate
        {
            get { return GetPermission(CommandCode.Create); }
        }

        /// <summary>
        /// CreateCommand触发时调用
        /// </summary>
        protected virtual void OnCreate()
        {
            OpenDetails(default(TModel));
        }

        #endregion

        #region Edit

        [RelayCommand(RelayCommands.Edit)]
        public ICommand EditCommand
        {
            get { return CommandContext.GetCommand<object>(() => EditCommand, ExecuteEdit, CanExecuteEdit); }
        }

        protected virtual bool CanExecuteEdit(object o)
        {
            return CanEdit;
        }

        protected virtual void ExecuteEdit(object o)
        {
            using (new StatusBusy(RuntimeContext.Service.L10N.GetText("Status.Loading"), ViewStatus))
                OnEdit(o);
        }

        /// <summary>
        /// 能否编辑
        /// </summary>
        protected virtual bool CanEdit
        {
            get { return GetPermission(CommandCode.Update); }
        }

        /// <summary>
        /// EditCommand触发时调用
        /// </summary>
        protected virtual void OnEdit(object o)
        {
            var model = o.ConvertTo<TModel>(default(TModel));
            if (model == null) return;
            var m = (TModel)model.Clone();
            m.ResetState();
            OpenDetails(m);
        }

        #endregion

        #region Search

        [RelayCommand(RelayCommands.Search)]
        public ICommand SearchCommand
        {
            get { return CommandContext.GetCommand(() => SearchCommand, ExecuteSearch, CanExecuteSearch); }
        }

        protected virtual void ExecuteSearch()
        {
            QueryString = FilterCriteria.ToCriteriaString();
            OnSearch();
        }

        protected virtual bool CanExecuteSearch()
        {
            return CanRead;
        }

        /// <summary>
        /// 能否读取
        /// </summary>
        public virtual bool CanRead
        {
            get { return GetPermission(CommandCode.Read); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnSearch()
        {
            SelectedModels.Clear();
        }

        #endregion

        #region Delete

        [RelayCommand(RelayCommands.Delete)]
        public ICommand DeleteCommand
        {
            get { return CommandContext.GetCommand<object>(() => DeleteCommand, ExecuteDelete, CanExecuteDelete); }
        }

        protected virtual void ExecuteDelete(object obj)
        {
            var model = obj.ConvertTo<TModel>(default(TModel));
            if (model == null) return;
            RuntimeContext.Service.GetObject<IDialog>().Show(new DialogMessage()
            {
                Sender = Source,
                Button = DialogButton.OKCancel,
                Caption = RuntimeContext.Service.L10N.GetText("Title.Delete"),
                Content = RuntimeContext.Service.L10N.GetText("Msg.DeleteConfirm"),
                DefaultResult = DialogResult.Cancel,
                Icon = DialogImage.Question,
                Callback = (e) =>
                {
                    if (e == DialogResult.OK)
                    {
                        using (new StatusBusy(RuntimeContext.Service.L10N.GetText("Status.Saving"),
                            ViewStatus, RuntimeContext.Service.L10N.GetText("Status.Saved")))
                        {
                            Delete(model);
                            OnDeleted(model);
                        }
                        RuntimeContext.Service.GetObject<IDialog>().Show(new DialogMessage(Source,
                            RuntimeContext.Service.L10N.GetText("Msg.DeleteSuccessfully").FormatArgs(1)));
                    }
                }
            });
        }

        protected virtual bool CanExecuteDelete(object obj)
        {
            return CanDelete;
        }

        protected abstract void Delete(TModel model);

        protected virtual void OnDeleted(TModel model)
        {
            if (SelectedModels.Count != 0)
                SelectedModels.Remove(model);
            RemoveRecent(model);
        }

        protected virtual void RemoveRecent(TModel model)
        {
            var hashCode = model.GetHashCode().ToString();
            RuntimeContext.Service.GetObject<INavigator>().RemoveModified(hashCode, Metadata.TypeName, ViewType.Wpf);
        }

        protected virtual bool CanDelete
        {
            get { return GetPermission(CommandCode.Delete); }
        }

        #endregion

        #region DeleteSelected

        [RelayCommand(RelayCommands.DeleteSelected)]
        public ICommand DeleteSelectedCommand
        {
            get { return CommandContext.GetCommand(() => DeleteSelectedCommand, ExecuteDeleteSelected, CanExecuteDeleteSelected); }
        }

        protected virtual void ExecuteDeleteSelected()
        {
            if (SelectedModels.Count == 0) return;
            int count = SelectedModels.Count;
            RuntimeContext.Service.GetObject<IDialog>().Show(new DialogMessage()
            {
                Sender = Source,
                Button = DialogButton.OKCancel,
                Caption = RuntimeContext.Service.L10N.GetText("Title.Delete"),
                Content = RuntimeContext.Service.L10N.GetText("Msg.DeleteSelectedConfirm"),
                DefaultResult = DialogResult.Cancel,
                Icon = DialogImage.Question,
                Callback = (e) =>
                {
                    if (e == DialogResult.OK)
                    {
                        using (new StatusBusy(RuntimeContext.Service.L10N.GetText("Status.Saving"),
                            ViewStatus, RuntimeContext.Service.L10N.GetText("Status.Saved")))
                        {
                            DeleteSelected();
                            OnSelectedDeleted();
                        }
                        RuntimeContext.Service.GetObject<IDialog>().Show(new DialogMessage(Source,
                            RuntimeContext.Service.L10N.GetText("Msg.DeleteSuccessfully").FormatArgs(count)));
                    }
                }
            });
        }

        protected virtual bool CanExecuteDeleteSelected()
        {
            return CanDelete && SelectedModels.Count > 0;
        }

        protected virtual void DeleteSelected()
        {
            foreach (var m in SelectedModels)
                Delete(m);
        }

        protected virtual void OnSelectedDeleted()
        {
            SelectedModels.Clear();
        }

        [RelayCommand(RelayCommands.Export)]
        public ICommand ExportCommand
        {
            get { return CommandContext.GetCommand<object>(() => ExportCommand, ExecuteExport, CanExecuteExport); }
        }

        protected virtual void ExecuteExport(object columns)
        {
        }

        protected virtual bool CanExecuteExport(object columns)
        {
            return GetPermission(CommandCode.Export);
        }

        #endregion
    }
}
