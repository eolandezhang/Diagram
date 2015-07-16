using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.ComponentModel
{
    /// <summary>
    /// Notifies clients that a property value is Changed.
    /// </summary>
    public interface INotifyValueChanged
    {
        event EventHandler<ValueChangedEventArgs> ValueChanged;
    }
}
