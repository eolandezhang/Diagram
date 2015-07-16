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
using QPP.Messaging;

namespace QPP.Wpf.Dialogs
{
    /// <summary>
    /// UrgeTaskDialog.xaml 的互動邏輯
    /// </summary>
    public partial class ExportResultDialog : MetroWindow
    {
        private string m_File = null;

        public ExportResultDialog(string file)
        {
            InitializeComponent();
            this.m_File = file;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (openDirectory.IsChecked == true)
            {
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
                psi.Arguments = " /select," + m_File;
                System.Diagnostics.Process.Start(psi);
            }
            if (openFile.IsChecked == true)
            {
                System.Diagnostics.Process.Start(m_File);
            }
            this.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
