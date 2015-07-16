using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Command
{
    public interface ICommandModel
    {
        string Name { get; set; }

        ICommand Command { get; set; }

        //string Icon { get; set; }

        //int VisibleIndex { get; set; }

        //bool BeginGroup { get; set; }

        //CommandUsage Usage { get; set; }

        //string Text { get; set; }

        //string ToolTip { get; set; }
    }
}
