using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Validation
{
    public interface IDataValidator
    {
        string Error { get; }
        string GetError(string columnName);
        void Validate(object obj, string propertyName);
        void Validate(object obj);
    }
}
