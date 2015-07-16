using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Parameter
{
    public interface ISysParameterKey
    {
        string Id { get; }
        string Name { get; }
        string Category { get; }
        string Description { get; }
    }
}
