using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Api
{
    public class ApiContext : IApiContext
    {
        public string Database { get; set; }

        public string CompanyId { get; set; }

        public string Simulator { get; set; }
    }
}
