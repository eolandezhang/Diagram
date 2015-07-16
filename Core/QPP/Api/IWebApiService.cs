using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Api
{
    public interface IWebApiService
    {
        string BaseAddress { get; set; }
        int Timeout { get; set; }
        string Database { get; set; }

        T Get<T>(string url);
        T Post<T>(string url, object data);
        void Put(string url, object obj);
        void Delete(string url);
    }
}
