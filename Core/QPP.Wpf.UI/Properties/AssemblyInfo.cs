using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// 組件的一般資訊是由下列的屬性集控制。
// 變更這些屬性的值即可修改組件的相關
// 資訊。
[assembly: AssemblyTitle("QPP.Wpf.UI")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("QPP.Wpf.UI")]
[assembly: AssemblyCopyright("Copyright ©  2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 將 ComVisible 設定為 false 會使得這個組件中的型別
// 對 COM 元件而言為不可見。如果您需要從 COM 存取這個組件中
// 的型別，請在該型別上將 ComVisible 屬性設定為 true。
[assembly: ComVisible(false)]

// 下列 GUID 為專案公開 (Expose) 至 COM 時所要使用的 typelib ID
[assembly: Guid("cb43dc87-495e-47ab-8a31-621a241225a2")]

// 組件的版本資訊是由下列四項值構成:
//
//      主要版本
//      次要版本 
//      組建編號
//      修訂編號
//
// 您可以指定所有的值，也可以依照以下的方式，使用 '*' 將組建和修訂編號
// 指定為預設值:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsPrefix("http://qpp.com/winfx/xaml/controls", "wpf")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/lib", "QPP.Wpf.UI.Behaviours")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/lib", "QPP.Wpf.UI.Converters")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/lib", "QPP.Wpf.UI.Actions")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/lib", "QPP.Wpf.UI.Models")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/lib", "QPP.Wpf.UI.Markup")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.Metro")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.AvalonDock")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.AvalonDock.Layout")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.MultiComboBox")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.ToolbarIcon")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.Toolkit")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.Toolkit.Panels")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.Toolkit.Chromes")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.Expander")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.Dashboards")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.Gantt")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.XGantt")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.Form")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.Pager")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.Range")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.FilterControl")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.Accordion")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.DragDrop")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls")]

[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.AvalonEdit")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.AvalonEdit.Editing")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.AvalonEdit.Rendering")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.AvalonEdit.Highlighting")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.UI.Controls.AvalonEdit.Search")]