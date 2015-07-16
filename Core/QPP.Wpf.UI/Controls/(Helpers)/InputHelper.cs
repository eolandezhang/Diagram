using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace QPP.Wpf.UI.Controls
{
    public class InputHelper : DependencyObject
    {
        public static readonly DependencyProperty InputBindingsProperty;
        static InputHelper()
        {
            InputBindingsProperty = DependencyProperty.RegisterAttached("InputBindings", typeof(ICollection), typeof(InputHelper), new FrameworkPropertyMetadata((d, e) =>
            {
                var element = d as UIElement;
                if (element != null)
                {
                    element.InputBindings.Clear();
                    element.InputBindings.AddRange(e.NewValue as ICollection);
                    if (e.NewValue is INotifyCollectionChanged)
                    {
                        ((INotifyCollectionChanged)e.NewValue).CollectionChanged += (x, y) =>
                        {
                            if (y.Action == NotifyCollectionChangedAction.Add || y.Action == NotifyCollectionChangedAction.Replace)
                            {
                                element.InputBindings.AddRange(y.NewItems as ICollection);
                            }
                            if (y.Action == NotifyCollectionChangedAction.Remove || y.Action == NotifyCollectionChangedAction.Replace)
                            {
                                foreach (InputBinding b in y.NewItems as ICollection)
                                    element.InputBindings.Remove(b);
                            }
                            if (y.Action == NotifyCollectionChangedAction.Reset)
                            {
                                element.InputBindings.Clear();
                                element.InputBindings.AddRange(x as ICollection);
                            }
                        };
                    }
                }
            }));
        }

        public static void SetInputBindings(DependencyObject obj, ICollection value)
        {
            obj.SetValue(InputBindingsProperty, value);
        }

        public static ICollection GetInputBindings(DependencyObject obj)
        {
            return (ICollection)obj.GetValue(InputBindingsProperty);
        }
    }
}
