using System.Windows.Input;

namespace QPP.Wpf.UI.Controls.XGantt.Core
{
    public interface IMouseWheelObserver 
    { 
        void OnMouseWheel(MouseWheelArgs args); 
        event MouseEventHandler MouseEnter; 
        event MouseEventHandler MouseLeave;
    }
}
