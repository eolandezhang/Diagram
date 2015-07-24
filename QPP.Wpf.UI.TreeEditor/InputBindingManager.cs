using System.Windows;
using System.Windows.Input;

namespace QPP.Wpf.UI.TreeEditor
{
    public class GlobalInputBindingManager
    {
        public static GlobalInputBindingManager Default = new GlobalInputBindingManager();
        static InputBindingCollection temp = new InputBindingCollection();
        public void Clear()
        {
            foreach (InputBinding inputBinding in Application.Current.MainWindow.InputBindings)
            {
                temp.Add(inputBinding);
            }
            Application.Current.MainWindow.InputBindings.Clear();
        }

        public void Recover()
        {
            if (temp.Count != 0)
            {
                foreach (InputBinding inputBinding in temp)
                {
                    Application.Current.MainWindow.InputBindings.Add(inputBinding);
                }
            }

        }

    }
}
