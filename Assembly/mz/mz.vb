Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\Apps\mz.exe

' 
'  // 
'  // 
'  // 
'  // VERSION:   1.0.0.0
'  // COPYRIGHT: Copyright © Microsoft 2018
'  // GUID:      2b91aac7-8c37-4662-b38a-daebec27c539
'  // 
' 
' 
'  < mz.Program >
' 
' 
' SYNOPSIS
' mz command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /export:           Export a single ms2 scan data.
'  /mgf:              Export all of the ms2 ions in target mzXML file and save as mgf file format.
'  /mz.calculate:     
'  /waves:            Export the ms1 intensity matrix.
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "mz ??<commandName>" for getting more details command help.
'    2. Using command "mz /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "mz /i" for enter interactive console mode.

Namespace CLI


''' <summary>
''' mz.Program
''' </summary>
'''
Public Class mz : Inherits InteropService

    Public Const App$ = "mz.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub

     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As mz
          Return New mz(App:=directory & "/" & mz.App)
     End Function

''' <summary>
''' ```
''' /export /in &lt;data.mzXML> /scan &lt;ms2_scan> [/out &lt;out.txt>]
''' ```
''' Export a single ms2 scan data.
''' </summary>
'''
Public Function MGF([in] As String, scan As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/export")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/scan " & """" & scan & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /mgf /in &lt;rawdata.mzXML> [/out &lt;ions.mgf>]
''' ```
''' Export all of the ms2 ions in target mzXML file and save as mgf file format.
''' </summary>
'''
Public Function DumpMs2([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/mgf")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /mz.calculate /mass &lt;mass> [/mode &lt;+/-, default=+> /out &lt;out.csv/html/txt>]
''' ```
''' </summary>
'''
Public Function Calculator(mass As String, Optional mode As String = "+", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/mz.calculate")
    Call CLI.Append(" ")
    Call CLI.Append("/mass " & """" & mass & """ ")
    If Not mode.StringEmpty Then
            Call CLI.Append("/mode " & """" & mode & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /waves /in &lt;data.mzXML> [/mz.range &lt;[min, max], default is all> /mz.round &lt;default=5> /out &lt;data.xls>]
''' ```
''' Export the ms1 intensity matrix.
''' </summary>
'''
Public Function MzWaves([in] As String, Optional mz_range As String = "", Optional mz_round As String = "5", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/waves")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not mz_range.StringEmpty Then
            Call CLI.Append("/mz.range " & """" & mz_range & """ ")
    End If
    If Not mz_round.StringEmpty Then
            Call CLI.Append("/mz.round " & """" & mz_round & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
