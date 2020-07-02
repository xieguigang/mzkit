#Region "Microsoft.VisualBasic::e8a3c9d32ee50537a8573c927f7a98dc, Rscript\Library\mzkit\Assembly.vb"

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

' Module Assembly
' 
'     Function: centroid, getMs1Scans, IonPeaks, mzXML2Mgf, ReadMgfIons
'               ReadMslIons, scanLoader, writeMgfIons
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSL
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.ValueTypes
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports mzXMLAssembly = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' The mass spectrum assembly file read/write library module.
''' </summary>
<Package("mzkit.assembly", Category:=APICategories.UtilityTools)>
Module Assembly

    <ExportAPI("read.msl")>
    Public Function ReadMslIons(file$, Optional unit As TimeScales = TimeScales.Second) As MSLIon()
        Return MSL.FileReader.Load(file, unit).ToArray
    End Function

    <ExportAPI("read.mgf")>
    Public Function ReadMgfIons(file As String) As Ions()
        Return MgfReader.StreamParser(path:=file).ToArray
    End Function

    ''' <summary>
    ''' this function ensure that the output result of the any input ion objects is peakms2 data type.
    ''' </summary>
    ''' <param name="ions">a vector of mgf <see cref="Ions"/> from the ``read.mgf`` function or other data source.</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("mgf.ion_peaks")>
    <RApiReturn(GetType(PeakMs2))>
    Public Function IonPeaks(<RRawVectorArgument> ions As Object, Optional env As Environment = Nothing) As Object
        Dim pipeline As pipeline = pipeline.TryCreatePipeline(Of Ions)(ions, env)

        If pipeline.isError Then
            Return pipeline.getError
        End If

        Return pipeline.populates(Of Ions) _
            .IonPeaks _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    ''' <summary>
    ''' write spectra data in mgf file format.
    ''' </summary>
    ''' <param name="ions"></param>
    ''' <param name="file">the file path of the mgf file to write spectra data.</param>
    ''' <param name="relativeInto">
    ''' write relative intensity value into the mgf file instead of the raw intensity value.
    ''' no recommended...
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("write.mgf")>
    Public Function writeMgfIons(ions As Object, file$, Optional relativeInto As Boolean = False) As Boolean
        If ions.GetType() Is GetType(pipeline) Then
            Using mgfWriter As StreamWriter = file.OpenWriter(Encodings.ASCII, append:=False)
                For Each ionPeak As PeakMs2 In DirectCast(ions, pipeline).populates(Of PeakMs2)
                    Call ionPeak _
                        .MgfIon _
                        .WriteAsciiMgf(mgfWriter, relativeInto)
                Next
            End Using
        ElseIf ions.GetType Is GetType(LibraryMatrix) Then
            Using mgf As StreamWriter = file.OpenWriter
                Call DirectCast(ions, LibraryMatrix) _
                    .MgfIon _
                    .WriteAsciiMgf(mgf)
            End Using
        End If

        Return True
    End Function

    ''' <summary>
    ''' get file index string of the given ms2 peak data.
    ''' </summary>
    ''' <param name="ms2"></param>
    ''' <returns></returns>
    <ExportAPI("file.index")>
    Public Function PeakMs2FileIndex(ms2 As PeakMs2) As String
        Return $"{ms2.file}#{ms2.scan}"
    End Function

    ''' <summary>
    ''' Convert mzxml file as mgf ions.
    ''' </summary>
    ''' <param name="mzXML"></param>
    ''' <returns></returns>
    <ExportAPI("mzxml.mgf")>
    Public Function mzXML2Mgf(mzXML$, Optional relativeInto As Boolean = False, Optional includesMs1 As Boolean = False) As pipeline
        Return mzXML _
            .scanLoader(relativeInto, includesMs1) _
            .Where(Function(peak) peak.mzInto.Length > 0) _
            .DoCall(Function(scans)
                        Return New pipeline(scans, GetType(PeakMs2))
                    End Function)
    End Function

    <Extension>
    Private Iterator Function scanLoader(mzXML$, relativeInto As Boolean, includesMs1 As Boolean) As IEnumerable(Of PeakMs2)
        Dim basename$ = mzXML.FileName

        For Each ms2Scan As scan In mzXMLAssembly.XML _
            .LoadScans(mzXML) _
            .Where(Function(s)
                       If includesMs1 Then
                           Return True
                       Else
                           Return s.msLevel = 2
                       End If
                   End Function)

            If ms2Scan.msLevel = 1 Then
                ' ms1的数据总是使用raw intensity值
                Yield ms2Scan.ScanData(basename, raw:=True)
            Else
                Yield ms2Scan.ScanData(basename, raw:=Not relativeInto)
            End If
        Next
    End Function

    ''' <summary>
    ''' get all ms1 raw scans from the raw files
    ''' </summary>
    ''' <param name="raw">
    ''' the file path of the raw data files.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("ms1.scans")>
    <RApiReturn(GetType(ms1_scan()))>
    Public Function getMs1Scans(<RRawVectorArgument> raw As Object, Optional env As Environment = Nothing) As Object
        Dim files As String() = REnv.asVector(Of String)(raw)
        Dim ms1 As New List(Of ms1_scan)
        Dim peakScans As PeakMs2

        For Each file As String In files
            Select Case file.ExtensionSuffix.ToLower
                Case "mzxml"
                    Dim basename$ = file.FileName

                    For Each scan As scan In mzXMLAssembly.XML _
                        .LoadScans(file) _
                        .Where(Function(s)
                                   Return s.msLevel = 1
                               End Function)

                        ' ms1的数据总是使用raw intensity值
                        peakScans = scan.ScanData(basename, raw:=True)
                        ms1 += peakScans.mzInto.ms2 _
                            .Select(Function(frag)
                                        Return New ms1_scan With {
                                            .intensity = frag.intensity,
                                            .mz = frag.mz,
                                            .scan_time = peakScans.rt
                                        }
                                    End Function)
                    Next
                Case Else
                    Throw New NotImplementedException
            End Select
        Next

        Return ms1.ToArray
    End Function
End Module
