using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.UI.Controls.DragDrop
{
    public interface IDragSource
    {
        void StartDrag(DragInfo dragInfo);
    }
}
