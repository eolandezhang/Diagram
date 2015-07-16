using System.Windows.Input;

namespace QPP.Wpf.UI.Controls.Gantt.Core
{
    public interface IMouseWheelObserver 
    { 
        void OnMouseWheel(MouseWheelArgs args); 
        event MouseEventHandler MouseEnter; 
        event MouseEventHandler MouseLeave;
    }
}
