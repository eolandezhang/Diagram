using QPP.Wpf.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace QPP.Wpf.Markup
{
    public class CommandToMenuItemConverterExtension : MarkupExtension
    {
        public string Target { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new CommandToMenuItemConverter(Target);
        }
    }
}
