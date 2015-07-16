using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Modularity
{
    public class ModuleCatalogAttribute: Attribute
    {
        public Type AppType { get; set; }

        public ModuleCatalogAttribute() { }

        public ModuleCatalogAttribute(Type type)
        {
            AppType = type;
        }
    }
}
