using QPP.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Metadata
{
    public interface IMetadataDescriptor
    {
        PresenterMetadata GetMetadata(Type type);
    }
}
