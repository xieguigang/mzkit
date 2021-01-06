Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices
#if netcore5=0 then 
' General Information about an assembly is controlled through the following
' set of attributes. Change these attribute values to modify the information
' associated with an assembly.

' Review the values of the assembly attributes

<Assembly: AssemblyTitle("assembly")>
<Assembly: AssemblyDescription("")>
<Assembly: AssemblyCompany("Microsoft")>
<Assembly: AssemblyProduct("assembly")>
<Assembly: AssemblyCopyright("Copyright © Microsoft 2017")>
<Assembly: AssemblyTrademark("")>

<Assembly: ComVisible(False)>

'The following GUID is for the ID of the typelib if this project is exposed to COM
<Assembly: Guid("2ca079f8-29f8-4fe6-9baf-e0a6a9302f2e")>

' Version information for an assembly consists of the following four values:
'
'      Major Version
'      Minor Version
'      Build Number
'      Revision
'
' You can specify all the values or you can default the Build and Revision Numbers
' by using the '*' as shown below:
' <Assembly: AssemblyVersion("1.0.*")>

<Assembly: AssemblyVersion("1.0.0.0")>
<Assembly: AssemblyFileVersion("1.0.0.0")>
#end if