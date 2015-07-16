using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq.Expressions;
using QPP.ComponentModel;
using QPP.Wpf.UI.Commands;

namespace QPP.Wpf.UI.Controls.FilterControl
{
    public class FilterControl : ListView
    {
        private ObservableCollection<IFilterNode> Nodes { get; set; }

        public string Value
        {
            get { return FilterCriteriaHelper.GetCriteria(Nodes); }//FilterCriteria.GetCriteria(Nodes); }
        }

        private ObservableCollection<FilterColumn> filterColumns;

        [Browsable(false)]
        public ObservableCollection<FilterColumn> FilterColumns
        {
            get
            {
                if (filterColumns == null)
                    filterColumns = new ObservableCollection<FilterColumn>();
                return filterColumns;
            }
            set { filterColumns = value; }
        }

        public FilterContentTemplateSelector FilterContentTemplateSelector { get; set; }

        public ObservableCollection<string> RelationCollection { get; set; }

        /// <summary>
        /// 为栏位下拉框提供 数据源
        /// </summary>
        public ObservableCollection<ValueTextEntry> FieldNameCollection { get; set; }

        // StringActionCollection、NumericOrDateTimeActionCollection和BooleanActionCollection为操作下拉框提供 数据源
        public ObservableCollection<Action> StringActionCollection { get; set; }
        public ObservableCollection<Action> NumericOrDateTimeActionCollection { get; set; }
        public ObservableCollection<Action> BooleanActionCollection { get; set; }

        #region private fields

        Dictionary<string, TypeCode> fieldNameToType = new Dictionary<string, TypeCode>();

        #endregion

        #region Constructors and Overrides

        static FilterControl()
        {
            var thisType = typeof(FilterControl);
            DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));            
        }

        public FilterControl()
        {
            filterColumns = new ObservableCollection<FilterColumn>();
            Nodes = new ObservableCollection<IFilterNode>();
        }

        public override void OnApplyTemplate()
        {
            RegisterColumns();
            InitFieldNameCollection();
            InitActionCollection();
            InitRelationCollection();
            InitFilterContentTemplateSelector();
            Nodes.Add(new FilterNode() { IsGroup = true });
            ItemsSource = Nodes;
            base.OnApplyTemplate();
        }
        #endregion
        
        #region 初始化 属性、字段方法
        void RegisterColumns()
        {
            foreach (var column in FilterColumns)
            {
                if (!fieldNameToType.ContainsKey(column.FieldName))
                    fieldNameToType.Add(column.FieldName, column.ColumnType);
            }
        }

        void InitRelationCollection()
        {
            RelationCollection = new ObservableCollection<string>();
            RelationCollection.Add("And");
            RelationCollection.Add("Or");
        }

        void InitFieldNameCollection()
        {
            FieldNameCollection = new ObservableCollection<ValueTextEntry>();
            foreach (var column in FilterColumns)
            {
                FieldNameCollection.Add(new ValueTextEntry(column.FieldName, column.Caption ?? column.FieldName));
            }
        }

        void InitFilterContentTemplateSelector()
        {
            if (Util.IsDesignMode) return;
            FilterContentTemplateSelector = new FilterContentTemplateSelector();
            var f = this.Template.Resources;
            if (this.Template.Resources.Values != null)
            {
                foreach (FilterDataTemplate d in f.Values)
                {
                    if (d != null)
                        FilterContentTemplateSelector.DataTemplateCollection.Add(d);
                }
            }
        }

        void InitActionCollection()
        {

            StringActionCollection = new ObservableCollection<Action>();
            StringActionCollection.Add(new Action() { Name = "Is Null", Value = ActionType.IsNull });
            StringActionCollection.Add(new Action() { Name = "Not Null", Value = ActionType.NotNull });
            StringActionCollection.Add(new Action() { Name = "= Equal", Value = ActionType.Equal });
            StringActionCollection.Add(new Action() { Name = "!= Not Equal", Value = ActionType.NotEqual });
            StringActionCollection.Add(new Action() { Name = "< Lesser", Value = ActionType.Lesser });
            StringActionCollection.Add(new Action() { Name = "<= Lesser Or Equal", Value = ActionType.LesserOrEqual });
            StringActionCollection.Add(new Action() { Name = "> Greater", Value = ActionType.Greater });
            StringActionCollection.Add(new Action() { Name = ">= Greater Or Equal", Value = ActionType.GreaterOrEqual });
            StringActionCollection.Add(new Action() { Name = "Between", Value = ActionType.Between });
            StringActionCollection.Add(new Action() { Name = "Not Between", Value = ActionType.NotBetween });
            StringActionCollection.Add(new Action() { Name = "Begin With", Value = ActionType.BeginWith });
            StringActionCollection.Add(new Action() { Name = "End With", Value = ActionType.EndWith });
            StringActionCollection.Add(new Action() { Name = "Contain", Value = ActionType.Contain });
            StringActionCollection.Add(new Action() { Name = "Not Contain", Value = ActionType.NotContain });

            NumericOrDateTimeActionCollection = new ObservableCollection<Action>();
            NumericOrDateTimeActionCollection.Add(new Action() { Name = "Is Null", Value = ActionType.IsNull });
            NumericOrDateTimeActionCollection.Add(new Action() { Name = "Not Null", Value = ActionType.NotNull });
            NumericOrDateTimeActionCollection.Add(new Action() { Name = "= Equal", Value = ActionType.Equal });
            NumericOrDateTimeActionCollection.Add(new Action() { Name = "!= Not Equal", Value = ActionType.NotEqual });
            NumericOrDateTimeActionCollection.Add(new Action() { Name = "< Lesser", Value = ActionType.Lesser });
            NumericOrDateTimeActionCollection.Add(new Action() { Name = "<= Lesser Or Equal", Value = ActionType.LesserOrEqual });
            NumericOrDateTimeActionCollection.Add(new Action() { Name = "> Greater", Value = ActionType.Greater });
            NumericOrDateTimeActionCollection.Add(new Action() { Name = ">= Greater Or Equal", Value = ActionType.GreaterOrEqual });
            NumericOrDateTimeActionCollection.Add(new Action() { Name = "Between", Value = ActionType.Between });
            NumericOrDateTimeActionCollection.Add(new Action() { Name = "Not Between", Value = ActionType.NotBetween });

            BooleanActionCollection = new ObservableCollection<Action>();
            BooleanActionCollection.Add(new Action() { Name = "IsTrue", Value = ActionType.IsTrue });
            BooleanActionCollection.Add(new Action() { Name = "IsFalse", Value = ActionType.IsFalse });

        }

        #endregion

        #region  event handlers

        void node_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var node = sender as FilterNode;
            if (e.PropertyName == "FieldName" && node.FieldName != null)
            {
                node.FilterDataTemplate = FilterColumns.FirstOrDefault(p => p.FieldName == node.FieldName).FilterDataTemplate;
                node.Type = fieldNameToType[node.FieldName];
                ResetValueByFieldName(node);
            }
            else if (e.PropertyName == "Action" && node.FieldName != null)// && node.Action!=null)
            {
                node.FilterDataTemplate = FilterColumns.FirstOrDefault(p => p.FieldName == node.FieldName).FilterDataTemplate;
                node.Type = fieldNameToType[node.FieldName];
                ResetValueByAction(node);
            }
        }

        void ResetValueByFieldName(IFilterNode node)
        {
            if (node.Type == TypeCode.Boolean)
            {
                node.Value = true;
                node.Action = ActionType.IsTrue;
            }
            else
            {
                node.Value = null;
                node.Scope.From = null;
                node.Scope.To = null;
                node.Action = ActionType.Equal;
            }
        }

        void ResetValueByAction(IFilterNode node)
        {
            if (node.Type == TypeCode.Boolean)
            {
                node.Value = true;
            }
            else
            {
                node.Value = null;
                node.Scope.From = null;
                node.Scope.To = null;
            }
        }

        #endregion

        #region RemoveComand


        public ICommand RemoveComand
        {
            get { return new RelayCommand(RemoveAction); }
        }

        private void RemoveAction(object obj)
         {
            try
            {
                var node = obj as FilterNode;
                if (node != null && node.Parent != null)
                {
                    var parent = node.Parent;
                    parent.Children.Remove(node);
                    Nodes.Remove(node);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToSafeString());
            }
        }
        #endregion

        #region AddComand
        public ICommand AddComand
        {
            get { return new RelayCommand(AddAction); }
        }

        int SetInsertingIndex(IFilterNode node, ref  int index)
        {
            foreach (IFilterNode child in node.Children)
            {
                index++;
                if (child.Children.Count > 0)
                    SetInsertingIndex(child, ref index);
            }
            return index;
        }

        private void AddAction(object obj)
        {
            try
            {
                var node = obj as FilterNode;
                if (node != null)
                {
                    int index = Nodes.IndexOf(node);
                    SetInsertingIndex(node, ref index);

                    var child = new FilterNode() { Name = "New Filter" };
                    child.PropertyChanged += node_PropertyChanged;
                    node.Children.Add(child);
                    child.Parent = node;

                    Nodes.Insert(++index, child);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToSafeString());
            }
        }
        #endregion

        #region  RemoveChildrenComand

        public ICommand RemoveChildrenComand
        {
            get { return new RelayCommand(RemoveChildrenAction, canRemoveExcute); }
        }

        private bool canRemoveExcute(object obj)
        {
            var node = obj as FilterNode;
            return (node != null && node.Children != null && node.Children.Count > 0);
        }

        private void RemoveChildrenAction(object obj)
        {
            try
            {
                var node = obj as FilterNode;
                if (node != null && node.Children.Count > 0)
                {
                    foreach (var child in node.Children.ToList())
                        RemoveChildrenAction(child);
                }
                RemoveAction(node);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToSafeString());
            }
        }

        #endregion

        #region  RemoveGroupComand

        public ICommand RemoveGroupComand
        {
            get { return new RelayCommand(RemoveGroupAction); }
        }

        private void RemoveGroupAction(object obj)
        {
            try
            {
                RemoveChildrenAction(obj);
                RemoveAction(obj);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToSafeString());
            }
        }
        #endregion

        #region AddRowComand

        public ICommand AddRowComand
        {
            get { return new RelayCommand(AddRowAction); }
        }

        private void AddRowAction(object obj)
        {
            try
            {
                var node = obj as FilterNode;
                if (node != null)
                {
                    int index = Nodes.IndexOf(node);
                    SetInsertingIndex(node, ref index);

                    FilterNode child = new FilterNode() { Name = "New Filter", IsGroup = true };
                    child.PropertyChanged += node_PropertyChanged;
                    node.Children.Add(child);
                    child.Parent = node;

                    Nodes.Insert(++index, child);

                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToSafeString());
            }
        }
        #endregion
    }
}
