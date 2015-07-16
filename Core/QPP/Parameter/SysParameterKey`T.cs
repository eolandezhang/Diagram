using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Parameter
{
    public abstract class SysParameterKey<T> : ISysParameterKey where T : ISysParameterValue
    {
        string id;
        protected SysParameterKey()
        {
            id = Security.Cryptography.Md5(GetType().FullName);
        }
        public string Id { get { return id; } }
        public abstract string Name { get; }
        public abstract string Category { get; }
        public abstract string Description { get; }
        public abstract T DefaultValue { get; }
    }
}
