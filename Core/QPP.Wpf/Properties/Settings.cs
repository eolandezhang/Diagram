using QPP.Wpf.Localization;
using QPP.Wpf.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace QPP.Wpf.Properties
{
    public partial class Settings
    {
        protected override void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Accent")
            {
                var currentAccent = ThemeManager.DefaultAccents.First(x => x.Name == Accent);
                ThemeManager.ChangeTheme(Application.Current, currentAccent, QPP.Wpf.UI.Theme.Light);
            }
            else if (e.PropertyName == "Langauge")
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Langauge);
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Langauge);
                L10N.Default.CultureCode = Langauge;
            }
            base.OnPropertyChanged(sender, e);
        }
    }
}
