using QPP.Collections;
using QPP.Command;
using QPP.Wpf.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace QPP.Wpf.Converters
{
    /// <summary>
    /// 把CommandContext轉換成MenuItem集合。
    /// <para/>
    /// 根據<see cref="CommandContext"/>.Commands中，類型為<see cref="RelayCommandModel"/>，
    /// Usage等於<see cref="CommandUsage"/>.ContextMenu的命令模型轉為成<see cref="MenuItem"/>
    /// </summary>
    public class CommandToMenuItemConverter : IValueConverter
    {
        public string Target { get; set; }

        public CommandToMenuItemConverter() { }

        public CommandToMenuItemConverter(string target)
        {
            Target = target;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var ctx = value as CommandContext;
            if (ctx == null) return null;
            BindingCollection<object> result = new BindingCollection<object>();
            ctx.PropertyChanged += (s, e) => result.Load(Load(ctx));
            result.Load(Load(ctx));
            return result;
        }

        List<object> Load(CommandContext ctx)
        {
            var result = new List<object>();
            foreach (var cmd in ctx.Commands.OfType<RelayCommandModel>()
                .Where(p => (p.Usage & CommandUsage.ContextMenu) == CommandUsage.ContextMenu && p.Target.CIEquals(Target))
                .OrderBy(p => p.VisibleIndex))
            {
                if (result.Count > 0 && cmd.BeginGroup)
                    result.Add(new Separator());//BeginGroup為True時，加入Separator
                var item = new MenuItem();
                item.Header = RuntimeContext.Service.L10N.GetText("Cmd." + cmd.Name);
                item.Command = (System.Windows.Input.ICommand)cmd;
                result.Add(item);
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
