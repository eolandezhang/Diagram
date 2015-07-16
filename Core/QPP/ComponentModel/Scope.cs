using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QPP.ComponentModel
{
    /// <summary>
    /// 值範圍
    /// </summary>
    public class Scope : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
                PropertyChanging.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        object _from;
        public object From
        {
            get { return _from; }
            set
            {
                if (!object.Equals(_from, value))
                {
                    OnPropertyChanging("From");
                    _from = value;
                    OnPropertyChanged("From");
                }
            }
        }

        object _to;
        public object To
        {
            get { return _to; }
            set
            {
                if (!object.Equals(_to, value))
                {
                    OnPropertyChanging("To");
                    _to = value;
                    OnPropertyChanged("To");
                }
            }
        }
    }
}
