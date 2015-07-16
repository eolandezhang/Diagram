using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Api
{
    public class ApiContainer : IApiContainer
    {
        static Dictionary<string, IWebApiService> services = new Dictionary<string, IWebApiService>();

        public IWebApiService GetService(string name = "default")
        {
            if (services.ContainsKey(name))
                return services[name];
            return null;
        }

        public void Register(string name, IWebApiService service)
        {
            services[name] = service;
        }

        public IApiContext Context { get; set; }
    }
}
