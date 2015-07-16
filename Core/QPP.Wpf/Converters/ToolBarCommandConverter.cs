using QPP.Collections;
using QPP.Command;
using QPP.Wpf.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace QPP.Wpf.Converters
{
    /// <summary>
    /// 把CommandContext轉換成Object集合。用于工具條綁定。
    /// <para/>
    /// 根據<see cref="CommandContext"/>.Commands中，類型為<see cref="RelayCommandModel"/>，
    /// Usage等於<see cref="CommandUsage"/>.ToolBar的命令模型轉為成<see cref="Object"/>
    /// </summary>
    public class ToolBarCommandConverter : IValueConverter
    {
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
            foreach (var model in ctx.Commands.OfType<RelayCommandModel>()
                .Where(p => (p.Usage & CommandUsage.ToolBar) == CommandUsage.ToolBar)
                .OrderBy(p => p.VisibleIndex))
            {
                if (result.Count > 0 && model.BeginGroup)
                    result.Add("-");//BeginGroup為True時，增加分割符號，配合SeparatorTemplateSelector生成Separator
                var cmd = new RelayCommandModel();
                cmd.Icon = model.Icon;
                cmd.Command = model.Command;
                RuntimeContext.Service.L10N.PropertyChanged += (s, e) =>
                {
                    cmd.Text = RuntimeContext.Service.L10N.GetText("Cmd." + model.Name);
                    cmd.ToolTip = RuntimeContext.Service.L10N.GetText("Cmd.ToolTip." + model.Name);
                };
                cmd.Text = RuntimeContext.Service.L10N.GetText("Cmd." + model.Name);
                cmd.ToolTip = RuntimeContext.Service.L10N.GetText("Cmd.ToolTip." + model.Name);
                result.Add(cmd);
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
