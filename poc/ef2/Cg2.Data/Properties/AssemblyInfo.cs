using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Cg2.Data")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Cg2.Data")]
[assembly: AssemblyCopyright("Copyright ©  2008")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("54b29c50-3208-4e2e-8e5d-86c0c6f2b717")]

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

[assembly: global::System.Data.Objects.DataClasses.EdmSchemaAttribute()]

[assembly: global::System.Data.Objects.DataClasses.EdmRelationshipAttribute("Cg2.Core.Domain"
    , "PersoonCommunicatieVorm", "Persoon"
    , global::System.Data.Metadata.Edm.RelationshipMultiplicity.One
    , typeof(Cg2.Core.Domain.Persoon)
    , "CommunicatieVorm", global::System.Data.Metadata.Edm.RelationshipMultiplicity.Many
    , typeof(Cg2.Core.Domain.CommunicatieVorm))]
