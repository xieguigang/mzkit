Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

#If netcore5 = 0 Then

' General Information about an assembly is controlled through the following
' set of attributes. Change these attribute values to modify the information
' associated with an assembly.

' Review the values of the assembly attributes

<Assembly: AssemblyTitle("SMILES - A Simplified Chemical Language")>
<Assembly: AssemblyDescription("SMILES - A Simplified Chemical Language")>
<Assembly: AssemblyCompany("BioDeep")>
<Assembly: AssemblyProduct("SMILES")>
<Assembly: AssemblyCopyright("Copyright © gg.xie@bionovogene.com 2021")>
<Assembly: AssemblyTrademark("mzkit")>

<Assembly: ComVisible(False)>

'The following GUID is for the ID of the typelib if this project is exposed to COM
<Assembly: Guid("860a80f4-0a6b-4aca-9d64-58996b45eb09")>

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