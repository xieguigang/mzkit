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
'  // VERSION:   2.3.7188.26508
'  // ASSEMBLY:  mz, Version=2.3.7188.26508, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright © BioNovogene 2018
'  // GUID:      2b91aac7-8c37-4662-b38a-daebec27c539
'  // BUILT:     9/6/2019 2:43:36 PM
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
'  /export:            Export a single ms2 scan data.
'  /mgf:               Export all of the ms2 ions in target mzXML file and save as mgf file format.
'  /mgf.batch:         
'  /mz.calculate:      
'  /selective.TIC:     Do TIC plot on a given list of selective parent ions.
'  /waves:             Export the ms1 intensity matrix.
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
''' /mgf /in &lt;rawdata.mzXML> [/relative /out &lt;ions.mgf>]
''' ```
''' Export all of the ms2 ions in target mzXML file and save as mgf file format.
''' </summary>
'''
Public Function DumpMs2([in] As String, Optional out As String = "", Optional relative As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/mgf")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If relative Then
        Call CLI.Append("/relative ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /mgf.batch /in &lt;data.directory> [/index_only /out &lt;data.directory>]
''' ```
''' </summary>
'''
Public Function DumpMs2Batch([in] As String, Optional out As String = "", Optional index_only As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/mgf.batch")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If index_only Then
        Call CLI.Append("/index_only ")
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
''' /selective.TIC /mz &lt;mz.list> /raw &lt;raw.mzXML> [/out &lt;TIC.png>]
''' ```
''' Do TIC plot on a given list of selective parent ions.
''' </summary>
'''
Public Function SelectiveTIC(mz As String, raw As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/selective.TIC")
    Call CLI.Append(" ")
    Call CLI.Append("/mz " & """" & mz & """ ")
    Call CLI.Append("/raw " & """" & raw & """ ")
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
