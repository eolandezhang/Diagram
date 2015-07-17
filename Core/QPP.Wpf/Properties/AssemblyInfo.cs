using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("QPP.Wpf")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("QPP.Wpf")]
[assembly: AssemblyCopyright("Copyright ©  2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("0c1422ef-62fa-4bf8-bfce-eb405c7eaae6")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
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
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.Layout")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.Query")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.Windows")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/controls", "QPP.Wpf.Controls")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/lib", "QPP.Wpf")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/lib", "QPP.Wpf.Markup")]
[assembly: XmlnsDefinition("http://qpp.com/winfx/xaml/lib", "QPP.Wpf.Properties")]
