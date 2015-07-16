using QPP.Collections;
using QPP.ComponentModel;
using QPP.Wpf.UI;

namespace QPP.Wpf
{
    /// <summary>
    /// 数据列表视图模型
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public abstract class GridViewModel<TModel> : GridViewModelBase<TModel>
        where TModel : DataModel, new()
    {
        protected override void Initialize()
        {
            base.Initialize();
            RegisterMessage();
        }

        protected virtual void RegisterMessage()
        {
            if (Util.IsDesignMode) return;
            //註冊修改通知,無需令牌,任何修改都刷新列表
            RuntimeContext.Service.Messenger.Register<ModifiedMessage<TModel>>(this, (e) =>
            {
                OnSearch();
            });
        }

        /// <summary>
        /// 模型集合
        /// </summary>
        public DataModelCollection<TModel> ModelCollection
        {
            get { return Get<DataModelCollection<TModel>>("ModelCollection", () => new DataModelCollection<TModel>()); }
        }

        protected override void OnDeleted(TModel model)
        {
            base.OnDeleted(model);
            ModelCollection.Remove(model);
        }

        protected override void OnSelectedDeleted()
        {
            foreach (var m in SelectedModels)
                ModelCollection.Remove(m);
            base.OnSelectedDeleted();
        }
    }
}
