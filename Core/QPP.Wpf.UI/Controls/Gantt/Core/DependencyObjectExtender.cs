using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace QPP.Wpf.UI.Controls.Gantt.Core
{
    public static class DependencyObjectExtender
    {
        public static List<DependencyObject> GetAllChildren(this DependencyObject instance)
        {
            return GetAllChildren(instance, true);
        }
        public static List<DependencyObject> GetAllChildren(this DependencyObject instance, bool Recursive)
        {
            int count = VisualTreeHelper.GetChildrenCount(instance);
            List<DependencyObject> result = new List<DependencyObject>(count);

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(instance, i);
                    result.Add(child);

                    if (Recursive)
                        result.AddRange(child.GetAllChildren(Recursive));
                }
            }

            return result;
        }
    }
}
