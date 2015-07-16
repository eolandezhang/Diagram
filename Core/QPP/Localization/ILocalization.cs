using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QPP.Localization
{
    public interface ILocalization : INotifyPropertyChanged
    {
        string GetText(string key);
    }
}
