using QPP.Collections;
using QPP.Command;
using QPP.ComponentModel;
using QPP.Runtime.Serialization;
using QPP.Wpf.Command;
using QPP.Wpf.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace QPP.Wpf
{
    /// <summary>
    /// 分頁数据列表视图模型
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public abstract class PagingGridViewModel<TModel> : GridViewModelBase<TModel>
        where TModel : DataModel, new()
    {
        protected virtual bool IsSavingPageSize
        {
            get { return true; }
        }

        protected string DESC<T>(Expression<Func<TModel, T>> expr)
        {
            if (expr.Body is MemberExpression)
            {
                MemberExpression memberExpression = (MemberExpression)expr.Body;
                return memberExpression.Member.Name + " DESC";
            }
            return null;
        }

        protected string ASC<T>(Expression<Func<TModel, T>> expr)
        {
            if (expr.Body is MemberExpression)
            {
                MemberExpression memberExpression = (MemberExpression)expr.Body;
                return memberExpression.Member.Name + " ASC";
            }
            return null;
        }

        /// <summary>
        /// 模型集合
        /// </summary>
        public DataModelCollection<TModel> ModelCollection
        {
            get { return Get<DataModelCollection<TModel>>("ModelCollection", () => new DataModelCollection<TModel>()); }
        }

        protected override void Initialize()
        {
            base.Initialize();
            RegisterMessage();
            CurrentPage = 1;
            if (IsSavingPageSize)
                PageSize = Settings.Default.DataGridPageSize;
            ModelCollection.ListChanged += new System.ComponentModel.ListChangedEventHandler(ModelCollection_ListChanged);
        }

        protected virtual void RegisterMessage()
        {
            if (QPP.Wpf.UI.Util.IsDesignMode) return;
            //註冊修改通知,無需令牌,任何修改都刷新列表
            RuntimeContext.Service.Messenger.Register<ModifiedMessage<TModel>>(this, (e) =>
            {
                if (e.IsCreated)
                {
                    ModelCollection.Insert(0, e.Content);
                    Total += 1;
                }
                else
                {
                    var m = ModelCollection.FirstOrDefault(p => EntityId.GetId(p) == EntityId.GetId(e.Content));
                    if (m != null)
                        e.Content.CopyTo(m);
                }
            });
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (!IsLoaded) return;//未加載之前,不觸發
            if (propertyName == "PageSize")
            {
                if (IsSavingPageSize)
                    Settings.Default.DataGridPageSize = PageSize;
                if (PageSize * (CurrentPage - 1) > Total)
                    CurrentPage = 1;
                else
                    FetchData();
            }
            else if (propertyName == "CurrentPage")
                FetchData();
        }

        void ModelCollection_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.PropertyDescriptorChanged)
                FetchData();
        }

        protected override void OnSearch()
        {
            base.OnSearch();
            IList<TModel> data = new List<TModel>();
            RunAsync(RuntimeContext.Service.L10N.GetText("Status.Loading"), () =>
            {
                CurrentPage = 1;
                data = FetchData(PageSize * (CurrentPage - 1), PageSize, ModelCollection.Sort);
                Total = FetchCount();
            }, () =>
            {
                ModelCollection.Load(data);
            });
        }

        protected abstract int FetchCount();

        void FetchData()
        {
            IList<TModel> data = new List<TModel>();
            RunAsync(RuntimeContext.Service.L10N.GetText("Status.Loading"), () =>
            {
                data = FetchData(PageSize * (CurrentPage - 1), PageSize, ModelCollection.Sort);
            }, () =>
            {
                ModelCollection.Load(data);
            });
        }

        protected virtual Criteria<TModel> GetQuery()
        {
            return QPP.Criteria.Create<TModel>(QueryString);
        }

        protected abstract IList<TModel> FetchData(int skipRows, int maxRows, string sort);

        protected override void OnDeleted(TModel model)
        {
            base.OnDeleted(model);
            ModelCollection.Remove(model);
            Total -= 1;
        }

        protected override void OnSelectedDeleted()
        {
            Total -= SelectedModels.Count;
            foreach (var m in SelectedModels)
                ModelCollection.Remove(m);
            base.OnSelectedDeleted();
        }

        public int Total
        {
            get { return Get<int>("Total"); }
            set { Set("Total", value); }
        }

        public int CurrentPage
        {
            get { return Get<int>("CurrentPage"); }
            set { Set("CurrentPage", value); }
        }

        public int PageSize
        {
            get { return Get<int>("PageSize"); }
            set { Set("PageSize", value); }
        }

        [RelayCommand(RelayCommands.NextPage)]
        public ICommand NextPageCommand
        {
            get { return CommandContext.GetCommand(() => NextPageCommand, ExecuteNextPage, CanExecuteNextPage); }
        }

        protected virtual void ExecuteNextPage()
        {
            CurrentPage++;
        }

        protected virtual bool CanExecuteNextPage()
        {
            return PageSize * CurrentPage < Total;
        }

        [RelayCommand(RelayCommands.PreviousPage)]
        public ICommand PreviousPageCommand
        {
            get { return CommandContext.GetCommand(() => PreviousPageCommand, ExecutePreviousPage, CanExecutePreviousPage); }
        }

        protected virtual void ExecutePreviousPage()
        {
            CurrentPage--;
        }

        protected virtual bool CanExecutePreviousPage()
        {
            return CurrentPage > 1;
        }

        protected override void ExecuteExport(object columns)
        {
            var data = FetchData(0, -1, ModelCollection.Sort);
            Utils.ExportHelper.Export(columns, data);
        }

        protected override void OnSerializing(SerializationInfo info)
        {
            info.AddValue("Query", FilterCriteria.ToQueryString());
        }

        protected override void OnDeserializing(SerializationInfo info)
        {
            var data = info.GetString("Query");
            FilterCriteria.LoadQuery(data);
            QueryString = FilterCriteria.ToCriteriaString();
            OnSearch();
        }
    }
}
