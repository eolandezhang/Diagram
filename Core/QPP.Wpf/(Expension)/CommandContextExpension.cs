using QPP.Command;
using QPP.Wpf.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QPP.Wpf
{
    public static class CommandContextExpension
    {
        /// <summary>
        /// 注册RelayCommand
        /// </summary>
        public static ICommand GetCommand(this ICommandContext commandService, RelayCommandAttribute attribute,
           Action action, Func<bool> canExecute = null)
        {
            if (commandService.Commands.ContainsName(attribute.Name))
                return commandService.Commands[attribute.Name].Command;
            var cmd = new RelayCommand(() =>
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception exc)
                {
                    RuntimeContext.Service.ExceptionHandler.Handle(exc);
                }
            }, canExecute);
            var commandModel = new RelayCommandModel(attribute, cmd);
            commandService.Register(commandModel);
            return cmd;
        }

        /// <summary>
        /// 注册RelayCommand&lt;T&gt;
        /// </summary>
        public static ICommand GetCommand<T>(this ICommandContext commandService, RelayCommandAttribute attribute,
            Action<T> action, Func<T, bool> canExecute = null)
        {
            if (commandService.Commands.ContainsName(attribute.Name))
                return commandService.Commands[attribute.Name].Command;
            var cmd = new RelayCommand<T>((e) =>
            {
                try
                {
                    action.Invoke(e);
                }
                catch (Exception exc)
                {
                    RuntimeContext.Service.ExceptionHandler.Handle(exc);
                }
            }, canExecute);
            var commandModel = new RelayCommandModel(attribute, cmd);
            commandService.Register(commandModel);
            return cmd;
        }


        /// <summary>
        /// 注册事件命令
        /// </summary>
        public static ICommand GetCommand(this ICommandContext commandService, EventCommandAttribute attribute, Action action)
        {
            if (commandService.Commands.ContainsName(attribute.Name))
                return commandService.Commands[attribute.Name].Command;
            var cmd = new RelayCommand(() =>
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception exc)
                {
                    RuntimeContext.Service.ExceptionHandler.Handle(exc);
                }
            });
            var commandModel = new EventCommandModel(attribute, cmd);
            commandService.Register(commandModel);
            return cmd;
        }

        /// <summary>
        /// 註冊事件命令
        /// </summary>
        public static ICommand GetCommand<T>(this ICommandContext commandService, EventCommandAttribute attribute, Action<T> action)
        {
            if (commandService.Commands.ContainsName(attribute.Name))
                return commandService.Commands[attribute.Name].Command;
            var cmd = new RelayCommand<T>((e) =>
            {
                try
                {
                    action.Invoke(e);
                }
                catch (Exception exc)
                {
                    RuntimeContext.Service.ExceptionHandler.Handle(exc);
                }
            });
            var commandModel = new EventCommandModel(attribute, cmd);
            commandService.Register(commandModel);
            return cmd;
        }

        /// <summary>
        /// 如果有特性EventCommandAttribute,則註冊事件命令,註冊依賴命令
        /// </summary>
        public static ICommand GetCommand<T>(this ICommandContext commandService,
            Expression<Func<ICommand>> commandPropertyExpression,
            Action<T> action, Func<T, bool> canExecute = null)
        {
            var member = (commandPropertyExpression.Body as MemberExpression).Member;
            var eventAttr = Attribute.GetCustomAttribute(member, typeof(EventCommandAttribute)) as EventCommandAttribute;
            if (eventAttr != null)
            {
                if (eventAttr.Name.IsNullOrEmpty())
                    eventAttr.Name = member.Name;
                return GetCommand(commandService, eventAttr, action);
            }
            var relayAttr = Attribute.GetCustomAttribute(member, typeof(RelayCommandAttribute)) as RelayCommandAttribute;
            if (relayAttr != null)
            {
                if (relayAttr.Name.IsNullOrEmpty())
                    relayAttr.Name = member.Name;
            }
            else
                relayAttr = new RelayCommandAttribute(member.Name);
            return GetCommand(commandService, relayAttr, action, canExecute);
        }

        /// <summary>
        /// 如果有特性EventCommandAttribute,則註冊事件命令,註冊依賴命令
        /// </summary>
        public static ICommand GetCommand(this ICommandContext commandService,
            Expression<Func<ICommand>> commandPropertyExpression,
            Action action, Func<bool> canExecute = null)
        {
            var member = (commandPropertyExpression.Body as MemberExpression).Member;
            var eventAttr = Attribute.GetCustomAttribute(member, typeof(EventCommandAttribute)) as EventCommandAttribute;
            if (eventAttr != null)
            {
                if (eventAttr.Name.IsNullOrEmpty())
                    eventAttr.Name = member.Name;
                return GetCommand(commandService, eventAttr, action);
            }
            var relayAttr = Attribute.GetCustomAttribute(member, typeof(RelayCommandAttribute)) as RelayCommandAttribute;
            if (relayAttr != null)
            {
                if (relayAttr.Name.IsNullOrEmpty())
                    relayAttr.Name = member.Name;
            }
            else
                relayAttr = new RelayCommandAttribute(member.Name);
            return GetCommand(commandService, relayAttr, action, canExecute);
        }

        /// <summary>
        /// 如果有特性EventCommandAttribute,則註冊事件命令,註冊依賴命令
        /// </summary>
        public static ICommand GetCommand<T>(this ICommandContext commandService,
            Expression<Func<ICommand>> commandPropertyExpression, IReceiver<T> action)
        {
            var member = (commandPropertyExpression.Body as MemberExpression).Member;
            var eventAttr = Attribute.GetCustomAttribute(member, typeof(EventCommandAttribute)) as EventCommandAttribute;
            if (eventAttr != null)
            {
                if (eventAttr.Name.IsNullOrEmpty())
                    eventAttr.Name = member.Name;
                return GetCommand(commandService, eventAttr, (Action<T>)action.Execute);
            }
            var relayAttr = Attribute.GetCustomAttribute(member, typeof(RelayCommandAttribute)) as RelayCommandAttribute;
            if (relayAttr != null)
            {
                if (relayAttr.Name.IsNullOrEmpty())
                    relayAttr.Name = member.Name;
            }
            else
                relayAttr = new RelayCommandAttribute(member.Name);
            return GetCommand(commandService, relayAttr, (Action<T>)action.Execute, action.CanExecute);
        }

        /// <summary>
        /// 如果有特性EventCommandAttribute,則註冊事件命令,註冊依賴命令
        /// </summary>
        public static ICommand GetCommand(this ICommandContext commandService,
            Expression<Func<ICommand>> commandPropertyExpression, IReceiver action)
        {
            var member = (commandPropertyExpression.Body as MemberExpression).Member;
            var eventAttr = Attribute.GetCustomAttribute(member, typeof(EventCommandAttribute)) as EventCommandAttribute;
            if (eventAttr != null)
            {
                if (eventAttr.Name.IsNullOrEmpty())
                    eventAttr.Name = member.Name;
                return GetCommand(commandService, eventAttr, action.Execute);
            }
            var relayAttr = Attribute.GetCustomAttribute(member, typeof(RelayCommandAttribute)) as RelayCommandAttribute;
            if (relayAttr != null)
            {
                if (relayAttr.Name.IsNullOrEmpty())
                    relayAttr.Name = member.Name;
            }
            else
                relayAttr = new RelayCommandAttribute(member.Name);
            return GetCommand(commandService, relayAttr, action.Execute, action.CanExecute);
        }
    }
}
