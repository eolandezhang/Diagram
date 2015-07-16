using QPP.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace QPP.Wpf.Command
{
    /// <summary>
    /// 用於綁定的CommandModel
    /// </summary>
    public class RelayCommandModel : StatefulObject, QPP.Command.ICommandModel, ICommand
    {
        public RelayCommandModel()
        {
        }

        public RelayCommandModel(RelayCommandAttribute attr, QPP.Command.ICommand command)
        {
            Command = command;
            Name = attr.Name;
            Target = attr.Target;
            Usage = attr.Usage;
            Icon = attr.Icon;
            VisibleIndex = attr.VisibleIndex;
            BeginGroup = attr.BeginGroup;
            if (attr.Key != System.Windows.Input.Key.None)
                InputGestures.Add(new System.Windows.Input.KeyGesture(attr.Key, attr.ModifierKeys));
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

        public string Target
        {
            get { return Get<string>("Target"); }
            set { Set("Target", value); }
        }

        public string Icon
        {
            get { return Get<string>("Icon"); }
            set { Set("Icon", value); }
        }

        public int VisibleIndex
        {
            get { return Get<int>("VisibleIndex"); }
            set { Set("VisibleIndex", value); }
        }

        public bool BeginGroup
        {
            get { return Get<bool>("BeginGroup"); }
            set { Set("BeginGroup", value); }
        }

        public QPP.Command.CommandUsage Usage
        {
            get { return Get<QPP.Command.CommandUsage>("Usage"); }
            set { Set("Usage", value); }
        }

        public InputGestureCollection InputGestures
        {
            get { return Get<InputGestureCollection>("InputGestures", () => new InputGestureCollection()); }
        }

        public string Text
        {
            get { return Get<string>("Text"); }
            set { Set("Text", value); }
        }

        public string ToolTip
        {
            get { return Get<string>("ToolTip"); }
            set { Set("ToolTip", value); }
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
