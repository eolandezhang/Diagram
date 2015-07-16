using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using QPP.ServiceLocation;
using QPP.Security;

namespace QPP.Wpf.Converters
{
    public class UserIdToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var user = RuntimeContext.Service.GetObject<IUserService>().Users.FirstOrDefault(p => p.Id.CIEquals(value.ToSafeString()));
            if (user != null)
                return user.UserName;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
