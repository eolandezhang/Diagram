using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.AddIn
{
    public class AddInToken
    {
        public Type Type { get; set; }

        public T Activate<T>()
        {
            return (T)Activator.CreateInstance(Type);
        }
    }
}
