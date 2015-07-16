using QPP.Command;
using QPP.Wpf.Command;
using QPP.Wpf.UI.Actions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;

namespace QPP.Wpf.Converter
{
    /// <summary>
    /// 把CommandContext轉換成EventToCommand集合。
    /// <para/>
    /// 根據<see cref="CommandContext"/>.Commands中，類型為
    /// <see cref="EventCommandModel"/>的命令模型轉為成EventToCommand
    /// </summary>
    public class EventToCommandConverter : IValueConverter
    {
        public EventToCommandConverter()
        {
        }

        public EventToCommandConverter(string target)
        {
            Target = target;
        }

        public string Target { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var ctx = value as CommandContext;
            if (ctx == null) return null;
            var result = new Collection<EventToCommand>();
            foreach (var cmd in ctx.Commands.OfType<EventCommandModel>()
                .Where(p => p.EventName.IsNotEmpty() && p.Target.CIEquals(Target)))
            {
                EventToCommand e = new EventToCommand();
                e.Command = (System.Windows.Input.ICommand)cmd;
                e.EventName = cmd.EventName;
                e.PassEventArgsToCommand = true;
                result.Add(e);
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
