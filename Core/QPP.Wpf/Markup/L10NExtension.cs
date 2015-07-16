using QPP.Wpf.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace QPP.Wpf.Markup
{
    public class L10NExtension : MarkupExtension
    {
        public L10NExtension() { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return L10N.Default;
        }
    }
}
