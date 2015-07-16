﻿using QPP.Wpf.UI.Converters;
using System;
using System.Windows;
using System.Windows.Markup;

namespace QPP.Wpf.UI.Markup
{

    /// <summary>
    /// Implements a markup extension that allows instances of <see cref="BooleanToVisibilityConverter"/> to be easily created.
    /// </summary>
    /// <remarks>
    /// This markup extension allows instance of <see cref="BooleanToVisibilityConverter"/> to be easily created inline in a XAML binding.
    /// See the example below.
    /// </remarks>
    /// <example>
    /// The following shows how to use the <c>BooleanToVisibilityConverterExtension</c> to display a <c>TextBox</c> only if the bound property is
    /// <see langword="true"/>:
    /// <code lang="xml">
    /// <![CDATA[
    /// <TextBox Visibility="{Binding ShowTheTextBox, Converter={BooleanToVisibilityConverter}}"/>
    /// ]]>
    /// </code>
    /// </example>
    public sealed class BoolToVisibilityConverterExtension : MarkupExtension
    {
        private bool isReversed;
        private bool useHidden;

        /// <summary>
        /// Initializes a new instance of the BooleanToVisibilityConverterExtension class.
        /// </summary>
        public BoolToVisibilityConverterExtension()
        {
        }

        /// <summary>
        /// Initializes a new instance of the BooleanToVisibilityConverterExtension class.
        /// </summary>
        /// <param name="isReversed">
        /// Whether the return values should be reversed.
        /// </param>
        /// <param name="useHidden">
        /// Whether <see cref="Visibility.Hidden"/> should be used instead of <see cref="Visibility.Collapsed"/>.
        /// </param>
        public BoolToVisibilityConverterExtension(bool isReversed, bool useHidden)
        {
            this.isReversed = isReversed;
            this.useHidden = useHidden;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the return values for the <see cref="BooleanToVisibilityConverter"/> should be reversed.
        /// </summary>
        [ConstructorArgument("isReversed")]
        public bool IsReversed
        {
            get { return this.isReversed; }
            set { this.isReversed = value; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="BooleanToVisibilityConverter"/> should return <see cref="Visibility.Hidden"/> instead
        /// of <see cref="Visibility.Collapsed"/>.
        /// </summary>
        [ConstructorArgument("useHidden")]
        public bool UseHidden
        {
            get { return this.useHidden; }
            set { this.useHidden = value; }
        }

        /// <summary>
        /// Provides an instance of <see cref="BooleanToVisibilityConverter"/> based on <see cref="IsReversed"/> and <see cref="UseHidden"/>.
        /// </summary>
        /// <param name="serviceProvider">
        /// An object that can provide services.
        /// </param>
        /// <returns>
        /// The instance of <see cref="BooleanToVisibilityConverter"/>.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new BoolToVisibilityConverter(this.isReversed, this.useHidden);
        }
    }
}