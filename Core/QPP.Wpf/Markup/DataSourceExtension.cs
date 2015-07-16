using QPP.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace QPP.Wpf.Markup
{
    public class DataSourceExtension : MarkupExtension
    {
        public string Type { get; set; }

        public DataSourceExtension() { }

        public DataSourceExtension(string type)
        {
            Type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (QPP.Wpf.UI.Util.IsDesignMode) return Type;
            return RuntimeContext.Service.GetObject<IDataSourceService>().GetDataSource(Type);
        }
    }
}
