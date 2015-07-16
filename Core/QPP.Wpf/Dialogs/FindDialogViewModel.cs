using QPP.Collections;
using QPP.Command;
using QPP.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace QPP.Wpf.Dialogs
{
    public class FindDialogViewModel<TModel> : ViewModelBase
        where TModel : class, IDataModel, new()
    {
        public WindowModel WindowModel
        {
            get { return Get<WindowModel>("WindowModel"); }
            set { Set("WindowModel", value); }
        }

        /// <summary>
        /// 指定清單方塊的選取行為。
        /// </summary>
        public SelectionMode SelectionMode
        {
            get { return Get<SelectionMode>("SelectionMode"); }
            set { Set("SelectionMode", value); }
        }

        /// <summary>
        /// 用于对话框是否返回结果集
        /// </summary>
        public bool DialogResult
        {
            get { return Get<bool>("DialogResult"); }
            set { Set("DialogResult", value); }
        }

        /// <summary>
        /// 未选中集合中的當前项
        /// </summary>
        public TModel CurrentUnelected
        {
            get { return Get<TModel>("SelectedModel"); }
            set { Set("SelectedModel", value); }
        }

        /// <summary>
        /// 选中集合中的當前项
        /// </summary>
        public TModel CurrentSelected
        {
            get { return Get<TModel>("ConfirmSelectedModel"); }
            set { Set("ConfirmSelectedModel", value); }
        }

        /// <summary>
        /// 存储选中项数据集合
        /// </summary>
        public ObservableCollection<TModel> SelectedItems { get; set; }

        /// <summary>
        /// 存储未选中数据集合
        /// </summary>
        public DataModelCollection<TModel> UnselectedItems { get; set; }

        /// <summary>
        /// 用于保存初始化数据,便于查询.
        /// </summary>
        public DataModelCollection<TModel> SourceCollection { get; set; }

        public List<object> SelectedValues { get; set; }

        /// <summary>
        /// 查询条件
        /// </summary>
        public string QueryString
        {
            get { return Get<string>("QueryString"); }
            set { Set("QueryString", value); }
        }

        public FindDialogViewModel()
        {
            WindowModel = new WindowModel();
            SelectedItems = new ObservableCollection<TModel>();
            UnselectedItems = new DataModelCollection<TModel>();
            SourceCollection = new DataModelCollection<TModel>();
            SelectedValues = new List<object>();
            SelectedItems.CollectionChanged += (o, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    var temp = e.NewItems[0] as TModel;
                    if (temp != null)
                        UnselectedItems.Remove(temp);
                }
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    var a = e.OldItems[0] as TModel;
                    if (a != null)
                        UnselectedItems.Add(a);
                }
            };
        }

        public ICommand SearchCommand
        {
            get { return CommandContext.GetCommand(() => SearchCommand, SearchAction); }
        }

        protected virtual void SearchAction()
        {

        }

        public ICommand ApplyCommand
        {
            get { return CommandContext.GetCommand(() => ApplyCommand, ApplyAction); }
        }

        protected virtual void ApplyAction()
        {
            DialogResult = true;
            WindowModel.Close();
        }

        public ICommand CancelCommand
        {
            get { return CommandContext.GetCommand(() => CancelCommand, CancelAction); }
        }

        protected virtual void CancelAction()
        {
            DialogResult = false;
            WindowModel.Close();
        }

        public ICommand ResetFilterCommand
        {
            get { return CommandContext.GetCommand(() => ResetFilterCommand, ResetFilterAction); }
        }

        protected virtual void ResetFilterAction()
        {
            QueryString = "";
        }

        public ICommand SelecteCommand
        {
            get { return CommandContext.GetCommand(() => SelecteCommand, SelecteAction); }
        }

        protected virtual void SelecteAction()
        {
            if (CurrentUnelected != null)
            {
                SelectedItems.Add(CurrentUnelected);
            }
        }

        public ICommand UnelecteCommand
        {
            get { return CommandContext.GetCommand(() => UnelecteCommand, UnselecteAction); }
        }

        protected virtual void UnselecteAction()
        {
            if (CurrentSelected != null)
            {
                SelectedItems.Remove(CurrentSelected);
            }
        }

        public ICommand SelecteAllCommand
        {
            get { return CommandContext.GetCommand(() => SelecteAllCommand, SelecteAllAction); }
        }

        protected virtual void SelecteAllAction()
        {
            var temp = new DataModelCollection<TModel>();
            temp.Load(UnselectedItems);
            foreach (var item in temp)
            {
                if (!SelectedItems.Contains(item))
                    SelectedItems.Add(item);
            }
        }

        public ICommand UnselecteAllCommand
        {
            get { return CommandContext.GetCommand(() => UnselecteAllCommand, UnselecteAllAction); }
        }

        protected virtual void UnselecteAllAction()
        {
            foreach (var item in SelectedItems)
            {
                UnselectedItems.Add(item);
            }
            SelectedItems.Clear();
        }
    }
}
