using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class DataSourceAttribute : Attribute
    {
        public string Name { get; set; }

        public DataSourceAttribute(string name)
        {
            Name = name;
        }
        public DataSourceAttribute()
        {
        }
    }
}
