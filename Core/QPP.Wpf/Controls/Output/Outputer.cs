using QPP.Command;
using QPP.ComponentModel;
using QPP.Messaging;
using QPP.Navigation;
using QPP.Runtime;
using QPP.Wpf;
using QPP.Wpf.Command;
using QPP.Wpf.Diagnostic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Controls.Output
{
    [QPP.Modularity.Presenter(Uri="/QPP.Wpf;component/Controls/Output/OutputBox.xaml")]
    public class Outputer : ViewModelBase
    {
        public static Outputer Default = new Outputer();

        public string Output
        {
            get { return Get<string>("Output"); }
            set { Set("Output", value); }
        }

        [RelayCommand]
        public ICommand ClearCommand
        {
            get { return CommandContext.GetCommand(() => ClearCommand, () => { Output = ""; }); }
        }

        public void Write(string message)
        {
            Output += message;
            if (Output.Length > 3000)
            {
                var index = Output.IndexOf("\r\n", 1000);
                if (index > 0)
                    Output = Output.Substring(index);
            }
        }
    }
}
