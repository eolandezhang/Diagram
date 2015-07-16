using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.ComponentModel.DataAnnotations
{
    [AttributeUsageAttribute(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class DeleteMarkAttribute:Attribute
    {
        public DeleteMarkAttribute()
        {
            Mark = true;
        }
        public DeleteMarkAttribute(object mark)
        {
            Mark = mark;
        }
        public object Mark { get; set; }
    }
}
