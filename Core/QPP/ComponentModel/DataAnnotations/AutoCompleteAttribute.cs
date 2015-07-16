using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AutoCompleteAttribute : Attribute
    {
        public string Name { get; set; }

        public AutoCompleteAttribute(string name)
        {
            Name = name;
        }
    }
}
