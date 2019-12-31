#Region "Microsoft.VisualBasic::cd8264fb18963f4be15eb0e96ec53af6, Assembly\mz\Pipeline\mz.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    ' Class mz
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: FromEnvironment
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\Apps\mz.exe

' 
'  // 
'  // m/z assembly file toolkit
'  // 
'  // VERSION:   2.3.7249.23804
'  // ASSEMBLY:  mz, Version=2.3.7249.23804, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright © BioNovogene 2019
'  // GUID:      2b91aac7-8c37-4662-b38a-daebec27c539
'  // BUILT:     11/6/2019 1:13:28 PM
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
'  /centroid:         Removes low abundance fragment details from the ms2 peaks from the profile mode
'                     raw data.
'  /export:           Export a single ms2 scan data.
'  /mgf:              Export all of the ms2 ions in target mzXML file and save as mgf file format. Load
'                     data from mgf file is more faster than mzXML raw data file.
'  /mgf.batch:        
'  /mz.calculate:     
'  /peaktable:        
'  /TIC:              
'  /waves:            Export the ms1 intensity matrix.
'  /XIC:              Do TIC plot on a given list of selective parent ions.
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
''' ```bash
''' /centroid /mgf &lt;raw.mgf&gt; [/ms2.tolerance &lt;default=da:0.1&gt; /into.cutoff &lt;default=0.05&gt; /out &lt;simple.mgf&gt;]
''' ```
''' Removes low abundance fragment details from the ms2 peaks from the profile mode raw data.
''' </summary>
'''
Public Function CentroidPeaksData(mgf As String, Optional ms2_tolerance As String = "da:0.1", Optional into_cutoff As String = "0.05", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/centroid")
    Call CLI.Append(" ")
    Call CLI.Append("/mgf " & """" & mgf & """ ")
    If Not ms2_tolerance.StringEmpty Then
            Call CLI.Append("/ms2.tolerance " & """" & ms2_tolerance & """ ")
    End If
    If Not into_cutoff.StringEmpty Then
            Call CLI.Append("/into.cutoff " & """" & into_cutoff & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /export /in &lt;data.mzXML&gt; /scan &lt;ms2_scan&gt; [/out &lt;out.txt&gt;]
''' ```
''' Export a single ms2 scan data.
''' </summary>
'''
Public Function printMatrix([in] As String, scan As String, Optional out As String = "") As Integer
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
''' ```bash
''' /mgf /in &lt;rawdata.mzXML&gt; [/relative /ms1 /out &lt;ions.mgf&gt;]
''' ```
''' Export all of the ms2 ions in target mzXML file and save as mgf file format. Load data from mgf file is more faster than mzXML raw data file.
''' </summary>
'''
Public Function DumpAsMgf([in] As String, Optional out As String = "", Optional relative As Boolean = False, Optional ms1 As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/mgf")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If relative Then
        Call CLI.Append("/relative ")
    End If
    If ms1 Then
        Call CLI.Append("/ms1 ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /mgf.batch /in &lt;data.directory&gt; [/index_only /out &lt;data.directory&gt;]
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
''' ```bash
''' /mz.calculate /mass &lt;mass&gt; [/mode &lt;+/-, default=+&gt; /out &lt;out.csv/html/txt&gt;]
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
''' ```bash
''' /peaktable /in &lt;raw.mzXML&gt; [/ms2 /tolerance &lt;default=da:0.3&gt; /out &lt;peaktable.xls&gt;]
''' ```
''' </summary>
'''
Public Function GetPeaktable([in] As String, Optional tolerance As String = "da:0.3", Optional out As String = "", Optional ms2 As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/peaktable")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not tolerance.StringEmpty Then
            Call CLI.Append("/tolerance " & """" & tolerance & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If ms2 Then
        Call CLI.Append("/ms2 ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /TIC /raw &lt;data.mgf&gt; [/out &lt;TIC.png&gt;]
''' ```
''' </summary>
'''
Public Function TIC(raw As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/TIC")
    Call CLI.Append(" ")
    Call CLI.Append("/raw " & """" & raw & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /waves /in &lt;data.mzXML&gt; [/mz.range &lt;[min, max], default is all&gt; /mz.round &lt;default=5&gt; /out &lt;data.xls&gt;]
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

''' <summary>
''' ```bash
''' /XIC /mz &lt;mz.list&gt; /raw &lt;raw.mzXML&gt; [/tolerance &lt;default=ppm:20&gt; /out &lt;XIC.png&gt;]
''' ```
''' Do TIC plot on a given list of selective parent ions.
''' </summary>
'''
Public Function XIC(mz As String, raw As String, Optional tolerance As String = "ppm:20", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/XIC")
    Call CLI.Append(" ")
    Call CLI.Append("/mz " & """" & mz & """ ")
    Call CLI.Append("/raw " & """" & raw & """ ")
    If Not tolerance.StringEmpty Then
            Call CLI.Append("/tolerance " & """" & tolerance & """ ")
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


