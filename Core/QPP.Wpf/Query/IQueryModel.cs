using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Query
{
    public interface IQueryModel
    {
        string Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string Value { get; set; }
        string Uri { get; set; }
        bool IsShared { get; set; }
        string CreateBy { get; set; }
        DateTime? CreateDate { get; set; }
        string UpdateBy { get; set; }
        DateTime? UpdateDate { get; set; }
    }
}
