using QPP.Wpf.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace QPP.Wpf.Markup
{
    public class HasCommmandExtension : MarkupExtension
    {
        public string PropertyName { get; set; }

        public HasCommmandExtension() { }

        public HasCommmandExtension(string propertyName)
        {
            PropertyName = propertyName;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new HasCommandConverter(PropertyName);
        }
    }
}
