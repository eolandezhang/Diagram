using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QPP.ComponentModel
{
    public interface IDataModel : IObservableObject, IStatefulObject, IDataErrorInfo
    {
    }
}
