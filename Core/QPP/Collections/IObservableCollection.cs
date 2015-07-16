using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Collections
{
    public interface IObservableCollection
    {
        IList Deleted { get; }
        IList Created { get; }
        IList Updated { get; }

        void ResumePropertyChanged();
        void SuppressPropertyChanged();
    }
}
