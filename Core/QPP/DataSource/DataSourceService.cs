using QPP.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.DataSource
{
    public class DataSourceService : IDataSourceService
    {
        Dictionary<string, DataSourceProvider> m_DataSourceProviders = new Dictionary<string, DataSourceProvider>();

        public IList<ValueTextEntry> GetDataSource(string name)
        {
            var key = name.ToLower();
            if (m_DataSourceProviders.ContainsKey(key))
                return m_DataSourceProviders[key].Invoke();
            throw new DataSourceException("DataSource with name:{0} not found.".FormatArgs(name));
        }

        public void Register(string name, DataSourceProvider provider)
        {
            var key = name.ToLower();
            m_DataSourceProviders.Add(key, provider);
        }
    }
}
