using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Query
{
    public interface IQueryService
    {
        IQueryModel CreateModel();
        IList<IQueryModel> GetQueries(string owner, string uri);
        void Save(IQueryModel model);
        void Delete(IQueryModel model);
    }
}
