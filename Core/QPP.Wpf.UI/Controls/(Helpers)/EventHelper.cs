using QPP.Command;
using QPP.Wpf.UI.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace QPP.Wpf.UI.Controls
{
    public class EventHelper : DependencyObject
    {
        public static readonly DependencyProperty EventToCommandProperty;

        static EventHelper()
        {
            EventToCommandProperty = DependencyProperty.RegisterAttached("EventToCommand", typeof(ICollection), typeof(EventHelper), new PropertyMetadata((x, y) =>
            {
                var collection = y.NewValue as ICollection;
                if (collection != null)
                {
                    foreach (EventToCommand action in collection)
                    {
                        var trigger = new System.Windows.Interactivity.EventTrigger(action.EventName);
                        trigger.Actions.Add(action);
                        System.Windows.Interactivity.Interaction.GetTriggers(x).Add(trigger);
                    }
                }
            }));
        }

        public static void SetEventToCommand(DependencyObject obj, ICollection value)
        {
            obj.SetValue(EventToCommandProperty, value);
        }

        public static ICollection GetEventHandler(DependencyObject obj)
        {
            return (ICollection)obj.GetValue(EventToCommandProperty);
        }
    }
}
