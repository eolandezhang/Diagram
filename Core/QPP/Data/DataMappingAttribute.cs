using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Data
{
    /// <summary>
    /// 標記包含XXX.hbm.xml資源文件的程序集,讓NHibernate自動加載
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class DataMappingAttribute : Attribute
    {
    }
}
