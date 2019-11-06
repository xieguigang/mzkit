Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\Apps\mzplot.exe

' 
'  // 
'  // Data visualization for mzXML file data
'  // 
'  // VERSION:   2.342.7249.24183
'  // ASSEMBLY:  mzplot, Version=2.342.7249.24183, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright © mzkit 2019
'  // GUID:      26fba577-47cd-4da1-a2bb-fccd493c9ff1
'  // BUILT:     11/6/2019 1:26:06 PM
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
'  /linear:     Test of the targetted metabolism quantify program.
'  /TIC:        Do TIC plot based on the given chromatogram table data.
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
''' ```bash
''' /linear /ref &lt;concentration.list&gt; /tpa &lt;tpa.list&gt; [/tpa.is &lt;tpa.list&gt; /title &lt;plot_title&gt; /weighted /out &lt;result.directory&gt;]
''' ```
''' Test of the targetted metabolism quantify program.
''' </summary>
'''
Public Function LinearFittings(ref As String, tpa As String, Optional tpa_is As String = "", Optional title As String = "", Optional out As String = "", Optional weighted As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/linear")
    Call CLI.Append(" ")
    Call CLI.Append("/ref " & """" & ref & """ ")
    Call CLI.Append("/tpa " & """" & tpa & """ ")
    If Not tpa_is.StringEmpty Then
            Call CLI.Append("/tpa.is " & """" & tpa_is & """ ")
    End If
    If Not title.StringEmpty Then
            Call CLI.Append("/title " & """" & title & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If weighted Then
        Call CLI.Append("/weighted ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /TIC /in &lt;data.csv&gt; [/XIC /rt &lt;rt_fieldName, default=rt&gt; /into &lt;intensity_fieldName, default=intensity&gt; /out &lt;plot.png&gt;]
''' ```
''' Do TIC plot based on the given chromatogram table data.
''' </summary>
'''
Public Function TICplot([in] As String, Optional rt As String = "rt", Optional into As String = "intensity", Optional out As String = "", Optional xic As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/TIC")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not rt.StringEmpty Then
            Call CLI.Append("/rt " & """" & rt & """ ")
    End If
    If Not into.StringEmpty Then
            Call CLI.Append("/into " & """" & into & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If xic Then
        Call CLI.Append("/xic ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace

