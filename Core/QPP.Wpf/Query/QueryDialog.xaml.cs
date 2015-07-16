using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QPP.Wpf.UI.Controls.Metro;
using QPP.Wpf.Command;

namespace QPP.Wpf.Query
{
    /// <summary>
    /// UrgeTaskDialog.xaml 的互動邏輯
    /// </summary>
    public partial class QueryDialog : MetroWindow
    {
        public delegate void QueryHandler(string queryString);

        IList<IQueryModel> QueryList;
        QueryHandler queryHandler;

        public string Uri { get; set; }

        public static ICommand FilterCommand { get; set; }

        public static ICommand EditCommand { get; set; }

        public static ICommand DeleteCommand { get; set; }

        static QueryDialog()
        {
            FilterCommand = new RelayCommand<QueryDialog>((d) => { });
            EditCommand = new RelayCommand<DataGrid>((d) => { });
            DeleteCommand = new RelayCommand<DataGrid>((d) => { });
        }

        public QueryDialog(QueryHandler handler)
        {
            InitializeComponent();
            queryHandler = handler;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            var value = grid.SelectedValue as IQueryModel;
            if (value != null)
            {
                queryHandler(value.Value);
                Close();
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var manager = RuntimeContext.Service.GetObject<IQueryService>();
            QueryList = manager.GetQueries(RuntimeContext.AppContext.User.Id, Uri);
            grid.ItemsSource = QueryList;
        }

        void FilterQuery()
        {
            if (txt.Text.IsNullOrEmpty())
                grid.ItemsSource = QueryList;
            else
            {
                var query = txt.Text.ToLower();
                var result = QueryList.Where(item =>
                {
                    var name = item.Name.ToSafeString().ToLower();
                    var description = item.Description.ToSafeString().ToLower();
                    return name.Contains(query)
                        || (name.IsNotEmpty() && query.Contains(name))
                        || (description.IsNotEmpty() && query.Contains(description));
                });
                grid.ItemsSource = result;
            }
        }

        private void txt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FilterQuery();
            }
        }

        private void grid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseDoubleClick += (x, y) => { FilterQuery(); };
        }
    }
}
