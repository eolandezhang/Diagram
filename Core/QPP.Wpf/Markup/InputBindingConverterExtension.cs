﻿using QPP.DataSource;
using QPP.Wpf.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace QPP.Wpf.Markup
{
    public class InputBindingConverterExtension : MarkupExtension
    {
        static InputBindingConverter converer = new InputBindingConverter();

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return converer;
        }
    }
}