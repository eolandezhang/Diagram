using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using QPP.Wpf.UI.Models;
using System.Collections.ObjectModel;
using QPP.Filtering;
using QPP.Wpf.UI.Controls.FilterControl;
using QPP.Wpf.ComponentModel;

namespace QPP.Wpf.Query
{
    [TemplatePart(Name = "saveButton")]
    [TemplatePart(Name = "searchButton")]
    public class QueryFormCommands : Control
    {
        public static DependencyProperty QueryCommandProperty;
        public static DependencyProperty FilterCriteriaProperty;
        public static DependencyProperty CanQueryProperty;

        static QueryFormCommands()
        {
            var thisType = typeof(QueryFormCommands);
            DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));
            QueryCommandProperty = DependencyProperty.Register("QueryCommand", typeof(ICommand), thisType, new PropertyMetadata(null));
            FilterCriteriaProperty = DependencyProperty.Register("FilterCriteria", typeof(FilterCriteria), thisType, new PropertyMetadata(null));
            CanQueryProperty = DependencyProperty.Register("CanQuery", typeof(bool), thisType, new PropertyMetadata(null));
        }

        ObservableCollection<FilterColumn> filterColumns;

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

        public ObservableCollection<FieldExpression> Expression { get; set; }

        public FilterCriteria FilterCriteria
        {
            get { return (FilterCriteria)GetValue(FilterCriteriaProperty); }
            set { SetValue(FilterCriteriaProperty, value); }
        }

        public bool CanQuery
        {
            get { return (bool)GetValue(CanQueryProperty); }
            set { SetValue(CanQueryProperty, value); }
        }

        public ICommand QueryCommand
        {
            get { return (ICommand)GetValue(QueryCommandProperty); }
            set { SetValue(QueryCommandProperty, value); }
        }

        public QueryFormCommands()
        {
            if (!QPP.Wpf.UI.Util.IsDesignMode)
                Loaded += new RoutedEventHandler(QueryFormCommands_Loaded);
        }

        void QueryFormCommands_Loaded(object sender, RoutedEventArgs e)
        {
            m_SaveButton.IsEnabled = CanQuery;
            m_SearchButton.IsEnabled = CanQuery;
            m_FindButton.IsEnabled = CanQuery && FilterColumns.Count > 0;
        }

        Button m_SaveButton;
        Button m_SearchButton;
        Button m_FindButton;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_SaveButton = GetTemplateChild("saveButton") as Button;
            m_SaveButton.Click += btnSave_Click;
            m_SearchButton = GetTemplateChild("searchButton") as Button;
            m_SearchButton.Click += btnSearch_Click;
            m_FindButton = GetTemplateChild("findButton") as Button;
            m_FindButton.Click += btnFind_Click;
        }

        void btnFind_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new DynamicQueryDialog();
            dialog.SetColumns(FilterColumns);
            dialog.Owner = Window.GetWindow(this);
            dialog.QueryCommand = QueryCommand;
            dialog.ShowDialog();
        }

        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var service = RuntimeContext.Service.GetObject<IQueryService>();
            var model = service.CreateModel();
            model.Value = FilterCriteria.ToCriteriaString();
            model.Id = Guid.NewGuid().ToString("N");
            model.Uri = BaseUriHelper.GetBaseUri(this) + Name;
            //var dialog = new QueryEditDialog();
            //QueryEditDialog.ShowDialog(model, Window.GetWindow(this));
        }

        void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            QueryDialog dialog = new QueryDialog(DoQuery);
            dialog.Uri = BaseUriHelper.GetBaseUri(this) + Name;
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        void DoQuery(string query)
        {
            QueryCommand.Execute(query);
        }
    }
}
