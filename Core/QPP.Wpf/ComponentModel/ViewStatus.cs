using QPP.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace QPP.Wpf.ComponentModel
{
    public class ViewStatus : StatefulObject
    {
        /// <summary>
        /// 是否繁忙
        /// </summary>
        public bool IsBusy
        {
            get { return Get<bool>("IsBusy"); }
            set { Set("IsBusy", value); }
        }

        public Cursor Cursor
        {
            get { return Get<Cursor>("Cursor"); }
            set { Set("Cursor", value); }
        }

        public string StatusText
        {
            get { return Get<string>("StatusText"); }
            set { Set("StatusText", value); }
        }
    }
}
