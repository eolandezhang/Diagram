using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace QPP.Wpf.UI.Controls.FilterControl
{
    public class FilterValueControl : ContentControl
    {
        public static DependencyProperty ValueContentProperty;
        public static DependencyProperty FilterContentTplSelectorProperty;

        public object ValueContent
        {
            get { return GetValue(ValueContentProperty); }
            set { SetValue(ValueContentProperty, value); }
        }


        private FilterNode _Node;
        public FilterNode Node
        {
            get { return _Node; }

            internal set
            {
                if (_Node != value)
                {
                    if (_Node != null)
                    {
                        
                        _Node.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(Node_PropertyChanged);
                    }
                    _Node = value;
                    if (_Node != null)
                    {

                        _Node.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Node_PropertyChanged);
                    }
                    
                }
            }
        }

        public FilterContentTemplateSelector FilterContentTplSelector
        {
            get { return (FilterContentTemplateSelector)GetValue(FilterContentTplSelectorProperty); }
            set { SetValue(FilterContentTplSelectorProperty, value); }
        }
        
        static FilterValueControl()
        {
            var thisType = typeof(FilterValueControl);
            ValueContentProperty = DependencyProperty.Register("ValueContent", typeof(object), thisType, new PropertyMetadata(null, ValueContent_PropertyChangedCallback));
            FilterContentTplSelectorProperty = DependencyProperty.Register("FilterContentTplSelector", typeof(FilterContentTemplateSelector), thisType, new PropertyMetadata(null, ContentTplSelector_PropertyChangedCallback)); 
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ContentTemplateSelector = FilterContentTplSelector;
        }


        static void ValueContent_PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                (d as FilterValueControl).Node = e.NewValue as FilterNode;
            }
        }

        static void ContentTplSelector_PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as FilterValueControl;
            if (me.ContentTemplateSelector == null)
                me.ContentTemplateSelector = e.NewValue as DataTemplateSelector;
        }

        private void Node_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FieldName" || e.PropertyName == "Action")
            {
                this.ContentTemplateSelector = null;
                this.ContentTemplateSelector = FilterContentTplSelector;
            }
        }
    }
}
