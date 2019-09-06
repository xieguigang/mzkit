Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\Apps\mzplot.exe

' 
'  // 
'  // 
'  // 
'  // VERSION:   1.0.0.0
'  // ASSEMBLY:  mzplot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright ©  2019
'  // GUID:      26fba577-47cd-4da1-a2bb-fccd493c9ff1
'  // BUILT:     1/1/2000 12:00:00 AM
'  // 
' 
' 
'  < mzplot.CLI >
' 
' 
' SYNOPSIS
' mzplot command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /TIC:     
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "mzplot ??<commandName>" for getting more details command help.
'    2. Using command "mzplot /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "mzplot /i" for enter interactive console mode.

Namespace CLI


''' <summary>
''' mzplot.CLI
''' </summary>
'''
Public Class mzplot : Inherits InteropService

    Public Const App$ = "mzplot.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub

     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As mzplot
          Return New mzplot(App:=directory & "/" & mzplot.App)
     End Function

''' <summary>
''' ```
''' /TIC /in &lt;data.csv> [/out &lt;plot.png>]
''' ```
''' </summary>
'''
Public Function TICplot([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/TIC")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
