using QPP.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace QPP.Wpf.Command
{
    public class EventCommandModel : StatefulObject, QPP.Command.ICommandModel, ICommand
    {
        public EventCommandModel()
        {
        }

        public EventCommandModel(EventCommandAttribute attr, QPP.Command.ICommand command)
        {
            Name = attr.Name;
            EventName = attr.EventName;
            Target = attr.Target;
            Command = command;
        }

        public string Name
        {
            get { return Get<string>("Name"); }
            set { Set("Name", value); }
        }

        public QPP.Command.ICommand Command
        {
            get { return Get<QPP.Command.ICommand>("Command"); }
            set { Set("Command", value); }
        }

        public string EventName
        {
            get { return Get<string>("EventName"); }
            set { Set("EventName", value); }
        }

        public string Target
        {
            get { return Get<string>("Target"); }
            set { Set("Target", value); }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return Command.CanExecute(parameter);
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { Command.CanExecuteChanged += value; }
            remove { Command.CanExecuteChanged -= value; }
        }

        void ICommand.Execute(object parameter)
        {
            Command.Execute(parameter);
        }
    }
}
