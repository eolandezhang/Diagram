using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Metadata
{
    public interface IMetadataDataProvider
    {
        PresenterMetadata GetMetadata(string id);
    }
}
