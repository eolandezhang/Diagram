using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Api
{
    public interface IApiContextProvider
    {
        IApiContext ApiContext { get; }
    }
}
