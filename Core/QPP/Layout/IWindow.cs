using QPP.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Layout
{
    public interface IWindow : IView
    {
        IWindow Owner { get; set; }

        event EventHandler Activated;

        bool Activate();

        void Show();

        bool? ShowDialog();

        void Close();
    }
}
