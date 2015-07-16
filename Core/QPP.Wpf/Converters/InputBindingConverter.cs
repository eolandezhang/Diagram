using QPP.Collections;
using QPP.Command;
using QPP.Wpf.Command;
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
    /// 把CommandContext轉換成InputBinding集合。
    /// <para/>
    /// 根據<see cref="CommandContext"/>.Commands中，類型為<see cref="RelayCommandModel"/>，
    /// Usage等於<see cref="CommandUsage"/>.KeyBinding的命令模型轉為成<see cref="InputBinding"/>
    /// </summary>
    public class InputBindingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var ctx = value as CommandContext;
            if (ctx == null) return null;
            var result = new BindingCollection<InputBinding>();
            ctx.PropertyChanged += (s, e) => result.Load(Load(ctx));
            result.Load(Load(ctx));
            return result;
        }

        List<InputBinding> Load(CommandContext ctx)
        {
            var result = new List<InputBinding>();
            foreach (var cmd in ctx.Commands.OfType<RelayCommandModel>()
                .Where(p => (p.Usage & CommandUsage.KeyBinding) == CommandUsage.KeyBinding))
            {
                foreach (InputGesture g in cmd.InputGestures)
                {
                    InputBinding binding = new InputBinding((System.Windows.Input.ICommand)cmd, g);
                    result.Add(binding);
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
