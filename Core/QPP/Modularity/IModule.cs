using QPP.Metadata;
using QPP.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Modularity
{
    public interface IModule : ILoadableItem
    {
        ModuleMetadata Metadata { get; }
    }
}
