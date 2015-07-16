using QPP.Command;
using QPP.Modularity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Command
{
    public class CommandContext : ICommandContext
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<CancelCommandEventArgs> PreExecute;

        public event EventHandler<CommandEventArgs> Executed;

        public CommandCollection Commands { get; set; }

        public CommandContext()
        {
            Commands = new CommandCollection();
        }

        public void Register(ICommandModel command)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (Commands.ContainsName(command.Name))
                throw new CommandIllegalException("Command with name {0} is registed.".FormatArgs(command.Name));
            Commands.Add(command);
            command.Command.PreExecute += (s, e) => OnCommandPreExecute(command, e);
            command.Command.Executed += (s, e) => OnCommandExecuted(command, e);
            OnPropertyChanged(this, new PropertyChangedEventArgs("Commands"));
        }

        public void Unregister(string command)
        {
            var cmd = Commands.FirstOrDefault(p => p.Name == command);
            if (cmd != null)
            {
                Commands.Remove(cmd);
                OnPropertyChanged(this, new PropertyChangedEventArgs("Commands"));
            }
        }

        public void Execute(string commandName, object param)
        {
            if (commandName == null)
                throw new ArgumentNullException("commandName");

            if (Commands.ContainsName(commandName))
                throw new CommandIllegalException("Command with name {0} is not registed.".FormatArgs(commandName));

            if (Commands[commandName].Command.CanExecute(param))
                Commands[commandName].Command.Execute(param);
        }

        public bool CanExecute(string commandName, object param)
        {
            if (commandName == null)
                throw new ArgumentNullException("commandName");

            if (Commands.ContainsName(commandName) && Commands[commandName].Command.CanExecute(param))
                return true;
            return false;
        }


        public void Init(IPresenter module)
        {
            var type = module.GetType();
            foreach (var p in type.GetProperties())
            {
                var cmd = Attribute.GetCustomAttribute(p, typeof(RelayCommandAttribute), false) as RelayCommandAttribute;
                if (cmd != null)
                {
                    var name = cmd.Name ?? p.Name;
                    var c = p.GetValue(module, null) as ICommand;
                    if (!Commands.ContainsName(name))
                        Register(new RelayCommandModel { Name = name, Command = c });
                }
                var e = Attribute.GetCustomAttribute(p, typeof(EventCommandAttribute), false) as EventCommandAttribute;
                if (e != null)
                {
                    var name = e.Name ?? p.Name;
                    var c = p.GetValue(module, null) as ICommand;
                    if (!Commands.ContainsName(name))
                        Register(new EventCommandModel { Name = name, EventName = e.EventName, Command = c });
                }
            }
        }

        protected virtual void OnCommandExecuted(object sender, CommandEventArgs e)
        {
            if (Executed != null)
                Executed(sender, e);
        }

        protected virtual void OnCommandPreExecute(object sender, CancelCommandEventArgs e)
        {
            if (PreExecute != null)
                PreExecute(sender, e);
        }

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(sender, e);
        }
    }
}
