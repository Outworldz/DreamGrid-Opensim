using System.Reflection;
using System.Runtime.InteropServices;
using Mono.Addins;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("WhoGotWhat")]
[assembly: AssemblyDescription("OpenSim addin to determin who got an item")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("WhoGotWhat.Properties")]
[assembly: AssemblyCopyright("Diva Canto")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("a915261b-15fc-4107-b65d-58ad6640f129")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion(OpenSim.VersionInfo.AssemblyVersionNumber)]

[assembly: Addin("WhoGotWhat", OpenSim.VersionInfo.VersionNumber + ".2")]
[assembly: AddinDependency("OpenSim.Region.Framework", OpenSim.VersionInfo.VersionNumber)]
[assembly: AddinDescription("OpenSim Addin tyo detect touches and log them to disk")]
[assembly: AddinAuthor("Diva Canto")]

[assembly: ImportAddinAssembly("WhoGotWhat.dll")]
[assembly: ImportAddinFile("WhoGotWhat.ini")]
[assembly: ImportAddinFile("WhoGotWhat.html")]
