using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Localization
{
    public interface IResourceModel
    {
        string CultureCode { get; set; }
        string Name { get; set; }
        string Value { get; set; }
    }
}
