
Imports System.Reflection
Imports System.Runtime.CompilerServices

<Assembly: AssemblyTitle("sciBASIC zip stream library")>
<Assembly: AssemblyDescription("zlibnet")>
<Assembly: AssemblyConfiguration("")>
<Assembly: AssemblyCompany("http://zlibnet.codeplex.com")>
<Assembly: AssemblyCopyright("Copyright (C) Gerry Shaw, Dave F. Baskin, Gunnar Dalsnes")>
<Assembly: AssemblyTrademark("")>
<Assembly: AssemblyCulture("")>
<Assembly: AssemblyVersion("1.3.3")>


Public NotInheritable Class ZLibDll
	Private Sub New()
	End Sub
	Friend Const Name32 As String = "zlib32.dll"
	Friend Const Name64 As String = "zlib64.dll"

	Friend Shared Is64 As Boolean = (IntPtr.Size = 8)

	Friend Const ZLibDllFileVersion As String = "1.2.8.1"

	Friend Shared Function GetDllName() As String
		If Is64 Then
			Return Name64
		Else
			Return Name32
		End If
	End Function

End Class

