#Region "Microsoft.VisualBasic::e1de758d386a6ca1b4f70040acd7c80b, Rscript\Library\mzkit\assembly\MzWeb.vb"

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

' Module MzWeb
' 
'     Function: GetChromatogram, loadStream, Ms1ScanPoints, Ms2ScanPeaks, Open
'               setMzpackThumbnail, ToMzPack, writeMzpack, writeStream, writeToCDF
' 
'     Sub: WriteCache
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports ChromatogramTick = BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram.ChromatogramTick

''' <summary>
''' biodeep mzweb data viewer raw data file helper
''' </summary>
<Package("mzweb")>
Module MzWeb

    <ExportAPI("open")>
    Public Function openFile(file As String) As BinaryStreamReader
        Return New BinaryStreamReader(file)
    End Function

    <ExportAPI("TIC")>
    Public Function TIC(mzpack As BinaryStreamReader) As ChromatogramTick()
        Dim keys As String() = mzpack.EnumerateIndex.ToArray
        Dim ticks As ChromatogramTick() = keys _
            .Select(Function(i)
                        Dim scan_time As Double
                        Dim TICpoint As Double

                        Call mzpack.ReadChromatogramTick(i, scan_time, 0, TICpoint)

                        Return New ChromatogramTick With {
                            .Time = scan_time,
                            .Intensity = TICpoint
                        }
                    End Function) _
            .ToArray

        Return ticks
    End Function

    ''' <summary>
    ''' load chromatogram data from the raw file data
    ''' </summary>
    ''' <param name="scans">
    ''' the scan data object that reads from the mzXML/mzML/mzPack raw data file
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("load.chromatogram")>
    <RApiReturn(GetType(Chromatogram))>
    Public Function GetChromatogram(scans As Object, Optional env As Environment = Nothing) As Object
        If TypeOf scans Is mzPack Then
            Return DirectCast(scans, mzPack).GetChromatogram
        ElseIf Not TypeOf scans Is pipeline Then
            Return Message.InCompatibleType(GetType(mzPack), scans.GetType, env)
        Else
            Dim scanPip As pipeline = DirectCast(scans, pipeline)

            If scanPip.elementType Like GetType(mzXML.scan) Then
                Return Chromatogram.GetChromatogram(scanPip.populates(Of scan)(env))
            ElseIf scanPip.elementType Like GetType(mzML.spectrum) Then
                Return Chromatogram.GetChromatogram(scanPip.populates(Of mzML.spectrum)(env))
            Else
                Return Message.InCompatibleType(GetType(mzXML.scan), scanPip.elementType, env)
            End If
        End If
    End Function

    ''' <summary>
    ''' load the unify mzweb scan stream data from the mzml/mzxml raw scan data stream.
    ''' </summary>
    ''' <param name="scans"></param>
    ''' <param name="mzErr$"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("load.stream")>
    <RApiReturn(GetType(ScanMS1))>
    Public Function loadStream(scans As pipeline,
                               Optional mzErr$ = "da:0.1",
                               Optional env As Environment = Nothing) As pipeline

        If scans.elementType Like GetType(mzXML.scan) Then
            Return mzWebCache _
                .Load(scans.populates(Of scan)(env), mzErr) _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        ElseIf scans.elementType Like GetType(mzML.spectrum) Then
            Return mzWebCache _
                .Load(scans.populates(Of mzML.spectrum)(env), mzErr) _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        Else
            Return Message.InCompatibleType(GetType(mzXML.scan), scans.elementType, env)
        End If
    End Function

    ''' <summary>
    ''' write ASCII text format of mzweb stream
    ''' </summary>
    ''' <param name="scans"></param>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("write.cache")>
    Public Function writeStream(scans As pipeline,
                                Optional file As Object = Nothing,
                                Optional env As Environment = Nothing) As Object
        Dim stream As Stream

        If file Is Nothing Then
            stream = Console.OpenStandardOutput
        ElseIf TypeOf file Is String Then
            stream = DirectCast(file, String).Open(doClear:=True)
        ElseIf TypeOf file Is Stream Then
            stream = DirectCast(file, Stream)
        Else
            Return Message.InCompatibleType(GetType(Stream), file.GetType, env)
        End If

        Call scans.populates(Of ScanMS1)(env).Write(stream)

        Return True
    End Function

    ''' <summary>
    ''' write binary format of mzweb stream data
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="cache"></param>
    <ExportAPI("packBin")>
    Public Sub WriteCache(file As String, cache As String)
        Using stream As New BinaryStreamWriter(file:=cache)
            If file.ExtensionSuffix("mzXML") Then
                For Each item In New mzXMLScans().Load(file)
                    Call stream.Write(item)
                Next
            Else
                For Each item In New mzMLScans().Load(file)
                    Call stream.Write(item)
                Next
            End If
        End Using
    End Sub

    <ExportAPI("write.mzPack")>
    Public Function writeMzpack(mzpack As mzPack, file As Object, Optional env As Environment = Nothing) As Object
        Dim filestream As [Variant](Of Stream, Message) = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

        If filestream Like GetType(Message) Then
            Return filestream.TryCast(Of Message)
        End If

        Return mzpack.Write(filestream.TryCast(Of Stream))
    End Function

    <ExportAPI("write.cdf")>
    Public Function writeToCDF(mzpack As mzPack, file As Object, Optional Ms2Only As Boolean = False, Optional env As Environment = Nothing) As Object
        Dim filestream As [Variant](Of Stream, Message) = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

        If filestream Like GetType(Message) Then
            Return filestream.TryCast(Of Message)
        Else
            Call mzpack.WriteCDF(filestream.TryCast(Of Stream), Ms2Only)
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' open a raw data files in common raw data format and then returns 
    ''' in a unify mzpack data format.
    ''' </summary>
    ''' <param name="file">the ``*.mzXML``/``*.mzML``/``*.mzPack``/``*.raw`` raw data file</param>
    ''' <returns></returns>
    <ExportAPI("open.mzpack")>
    Public Function Open(file As String) As mzPack
        If file.ExtensionSuffix("mzXML", "mzML", "imzML") Then
            Return Converter.LoadRawFileAuto(xml:=file)
        ElseIf file.ExtensionSuffix("raw") Then
            Using msRaw As New MSFileReader(file)
                Return msRaw.LoadFromXRaw
            End Using
        ElseIf file.ExtensionSuffix("cdf") Then
            ' convert MSI cdf to mzpack
            Using cdf As New netCDFReader(file)
                Return New mzPack With {
                    .MS = cdf.CreateMs1.ToArray,
                    .Application = FileApplicationClass.MSImaging,
                    .source = file.FileName,
                    .Scanners = New Dictionary(Of String, ChromatogramOverlap),
                    .Chromatogram = Nothing,
                    .Thumbnail = Nothing
                }
            End Using
        Else
            Using stream As Stream = file.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Return mzPack.ReadAll(file:=stream)
            End Using
        End If
    End Function

    <ExportAPI("setThumbnail")>
    Public Function setMzpackThumbnail(mzpack As mzPack, thumb As Object) As mzPack
        If TypeOf thumb Is GraphicsData Then
            thumb = DirectCast(thumb, GraphicsData).AsGDIImage
        End If

        mzpack.Thumbnail = DirectCast(thumb, Image)
        Return mzpack
    End Function

    <ExportAPI("ms1_scans")>
    Public Function Ms1ScanPoints(mzpack As mzPack) As ms1_scan()
        Return mzpack.GetAllScanMs1.ToArray
    End Function

    <ExportAPI("ms1_peaks")>
    <RApiReturn(GetType(LibraryMatrix))>
    Public Function Ms1Peaks(mzpack As mzPack,
                             Optional tolerance As Object = "da:0.001",
                             Optional cutoff As Double = 0.05,
                             Optional env As Environment = Nothing) As Object

        Dim mzdiff = Math.getTolerance(tolerance, env)

        If mzdiff Like GetType(Message) Then
            Return mzdiff.TryCast(Of Message)
        End If

        Dim ms As ms2() = mzpack.MS _
            .Select(Function(scan) scan.GetMs) _
            .IteratesALL _
            .ToArray _
            .Centroid(mzdiff.TryCast(Of Tolerance), New RelativeIntensityCutoff(cutoff)) _
            .ToArray

        Return New LibraryMatrix With {
            .centroid = True,
            .ms2 = ms,
            .name = mzpack.source & " MS1"
        }
    End Function

    ''' <summary>
    ''' extract ms2 peaks data from the mzpack data object
    ''' </summary>
    ''' <param name="mzpack"></param>
    ''' <param name="precursorMz">
    ''' if the precursor m/z data is assign by this parameter
    ''' value, then this function will extract the ms2 xic data
    ''' only
    ''' </param>
    ''' <param name="tolerance">
    ''' ppm toleracne error for extract ms2 xic data.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("ms2_peaks")>
    <RApiReturn(GetType(PeakMs2))>
    Public Function Ms2ScanPeaks(mzpack As mzPack,
                                 Optional precursorMz As Double = Double.NaN,
                                 Optional tolerance As Object = "ppm:30",
                                 Optional env As Environment = Nothing) As Object

        If precursorMz.IsNaNImaginary Then
            Return mzpack.GetMs2Peaks.ToArray
        Else
            Dim mzerr = Math.getTolerance(tolerance, env)

            If mzerr Like GetType(Message) Then
                Return mzerr.TryCast(Of Message)
            End If

            Dim mzdiff As Tolerance = mzerr.TryCast(Of Tolerance)
            Dim ms2_xic = mzpack.MS _
                .Select(Function(d) d.products) _
                .IteratesALL _
                .Where(Function(scan)
                           Return mzdiff(scan.parentMz, precursorMz)
                       End Function) _
                .Select(AddressOf mzPack.CastToPeakMs2) _
                .ToArray

            Return ms2_xic
        End If
    End Function

    ''' <summary>
    ''' convert assembly file to mzpack format data object
    ''' </summary>
    ''' <param name="assembly"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("as.mzpack")>
    <RApiReturn(GetType(mzPack))>
    Public Function ToMzPack(assembly As Object,
                             Optional modtime As Double = -1,
                             Optional env As Environment = Nothing) As Object

        If TypeOf assembly Is netCDFReader Then
            Return GC2Dimensional.ToMzPack(agilentGC:=assembly, modtime:=modtime)
        Else
            Return Message.InCompatibleType(GetType(netCDFReader), assembly.GetType, env)
        End If
    End Function
End Module
