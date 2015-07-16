using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Api
{
    public interface IApiContainer
    {
        IWebApiService GetService(string name = "default");

        void Register(string name, IWebApiService service);

        IApiContext Context { get; }
    }
}
