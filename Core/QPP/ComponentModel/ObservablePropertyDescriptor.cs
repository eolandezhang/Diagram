
using System.Collections;
namespace QPP.ComponentModel
{
    public class ObservablePropertyDescriptor
    {
        public static T GetValue<T>(ObservableObject obj, string propertyName)
        {
            return obj.Get<T>(propertyName);
        }

        public static void SetValue<T>(ObservableObject obj, string propertyName, T value)
        {
            obj.Set<T>(propertyName, value);
        }

        public static void CopyTo(ObservableObject source, ObservableObject target)
        {
            foreach (DictionaryEntry item in source.ValueTable)
            {
                target.ValueTable[item.Key] = item.Value;
            }
        }
    }
}
