using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.ServiceLocation
{
    public interface IServiceLocator
    {
        bool Contains(string name);
        bool Contains<T>();
        T GetObject<T>(string name);
        T GetObject<T>();
        IDictionary<string, T> GetObjects<T>();
    }
}
