using System.Windows.Interactivity;
using QPP.Wpf.UI.Controls.Metro;

namespace QPP.Wpf.UI.Behaviours
{
    public class WindowsSettingBehavior : Behavior<MetroWindow>
    {
        protected override void OnAttached()
        {
            if (AssociatedObject != null && AssociatedObject.SaveWindowPosition) {
                // save with custom settings class or use the default way
                var windowPlacementSettings = this.AssociatedObject.WindowPlacementSettings ?? new WindowApplicationSettings(this.AssociatedObject);
                WindowSettings.SetSave(AssociatedObject, windowPlacementSettings);
            }
        }
    }
}