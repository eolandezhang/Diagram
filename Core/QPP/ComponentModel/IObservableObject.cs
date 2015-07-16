using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QPP.ComponentModel
{
    public interface IObservableObject : INotifyPropertyChanged, INotifyPropertyChanging, INotifyValueChanged, ICloneable
    {
        object this[string propertyName] { get;set;}
    }
}
