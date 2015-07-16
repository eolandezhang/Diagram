using QPP.Wpf.UI.Controls.FilterControl;
using QPP.Wpf.UI.Controls.Metro;
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

namespace QPP.Wpf.Query
{
    /// <summary>
    /// Interaction logic for DynamicQueryDialog.xaml
    /// </summary>
    public partial class DynamicQueryDialog : MetroWindow
    {
        public DynamicQueryDialog()
        {
            InitializeComponent();
        }

        public ICommand QueryCommand { get; set; }

        public void SetColumns(IEnumerable<FilterColumn> columns)
        {
            foreach (var c in columns)
                filter.FilterColumns.Add(c);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            QueryCommand.Execute(filter.Value);
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var service = RuntimeContext.Service.GetObject<IQueryService>();
            var model = service.CreateModel();
            model.Value = filter.Value;
            model.Uri = BaseUriHelper.GetBaseUri(this) + Name;
            //var model = new SysQueryDomain();
            //model.Value = filter.Value;
            //model.Id = Guid.NewGuid().ToString("N");
            //model.PageUri = BaseUriHelper.GetBaseUri(this) + Name;
            //Hide();
            //QueryEditDialog.ShowDialog(model, Owner);
            //ShowDialog();
        }
    }
}
