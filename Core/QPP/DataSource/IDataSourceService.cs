using QPP.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.DataSource
{
    public interface IDataSourceService
    {
        IList<ValueTextEntry> GetDataSource(string name);
        void Register(string name, DataSourceProvider provider);
    }
}
