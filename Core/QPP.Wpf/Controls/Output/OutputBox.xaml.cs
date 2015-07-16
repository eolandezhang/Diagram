using QPP.Wpf.Layout;
using System.Windows.Controls;

namespace QPP.Wpf.Controls.Output
{
    public partial class OutputBox : DockingAnchorable
    {
        public OutputBox()
        {
            InitializeComponent();
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = sender as TextBox;
            txt.ScrollToEnd();
        }
    }
}
