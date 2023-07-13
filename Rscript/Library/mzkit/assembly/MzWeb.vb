#Region "Microsoft.VisualBasic::bdb34306931fb9ddfb1bbab62656f05d, mzkit\Rscript\Library\mzkit\assembly\MzWeb.vb"

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


' Code Statistics:

'   Total Lines: 574
'    Code Lines: 374
' Comment Lines: 133
'   Blank Lines: 67
'     File Size: 21.55 KB


' Module MzWeb
' 
'     Function: GetChromatogram, loadStream, MassCalibration, Ms1Peaks, Ms1ScanPoints
'               Ms2ScanPeaks, Open, openFile, openFromFile, readCache
'               setMzpackThumbnail, TIC, ToMzPack, uniqueReference, writeCache
'               writeMzpack, writeStream, writeToCDF
' 
'     Sub: WriteCache
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.ProteinModel.ChouFasmanRules.Rules
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Components.Interface
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports ChromatogramTick = BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram.ChromatogramTick

#If NET48 Then
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
#End If

''' <summary>
''' biodeep mzweb data viewer raw data file helper
''' </summary>
<Package("mzweb")>
Module MzWeb

    <ExportAPI("loadXcmsRData")>
    <RApiReturn(GetType(PeakMs2))>
    Public Function loadXcmsRData(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim buf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If buf Like GetType(Message) Then
            Return buf.TryCast(Of Message)
        End If

        Dim dataset = XcmsRData.ReadRData(buf.TryCast(Of Stream))
        Dim peaks As PeakMs2() = dataset.GetMsMs.ToArray

        Return peaks
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="file">
    ''' the file path to the mzpack data file
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("open")>
    Public Function openFile(file As String, Optional env As Environment = Nothing) As IMzPackReader
        Return MzPackAccess.open_mzpack(file, env)
    End Function

    <ExportAPI("TIC")>
    Public Function TIC(mzpack As IMzPackReader) As ChromatogramTick()
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
    ''' Parse the ms scan data from a given raw byte stream data
    ''' </summary>
    ''' <param name="bytes">the raw vector which could be parsed from the HDS file via HDS read data function.</param>
    ''' <param name="level">specific the ms level to parse the scan data, level could be 1(scan ms1) or 2(scan ms2)</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("parse.scanMs")>
    Public Function parseScanMsBuffer(<RRawVectorArgument> bytes As Object,
                                      <RRawVectorArgument(GetType(Integer))>
                                      Optional level As Object = "1,2",
                                      Optional env As Environment = Nothing) As Object

        Dim raw = SMRUCC.Rsharp.GetFileStream(bytes, FileAccess.Read, env)
        Dim levels As Integer() = CLRVector.asInteger(level)

        If raw Like GetType(Message) Then
            Return raw.TryCast(Of Message)
        End If

        If levels.ElementAtOrDefault(0, 1) = 1 Then
            ' parse scanms1
            Dim ms1 As New ScanMS1 With {.meta = New Dictionary(Of String, String)}
            Dim reader As New BinaryDataReader(raw.TryCast(Of Stream)) With {.ByteOrder = ByteOrder.LittleEndian}

            Call reader.Seek(Scan0, SeekOrigin.Begin)
            Call Serialization.ReadScan1(ms1, file:=reader, readmeta:=True)

            Return ms1
        Else
            ' parse scanms2
            Dim scan2 As ScanMS2
            Dim pool As New BinaryDataReader(raw.TryCast(Of Stream)) With {.ByteOrder = ByteOrder.LittleEndian}

            pool.Seek(Scan0, SeekOrigin.Begin)
            scan2 = pool.ReadScanMs2

            Return scan2
        End If
    End Function

    ''' <summary>
    ''' write ASCII text format of mzweb stream
    ''' </summary>
    ''' <param name="scans"></param>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("write.text_cache")>
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

    ''' <summary>
    ''' read the mzPack data file liked simple msn cached data
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read.cache")>
    Public Function readCache(file As String) As PeakMs2()
        Using buffer As New BinaryDataReader(file.Open(FileMode.Open, doClear:=False, [readOnly]:=True)) With {
            .ByteOrder = ByteOrder.BigEndian
        }
            If buffer.ReadString(BinaryStringFormat.ZeroTerminated) <> "mzcache" Then
                Throw New InvalidProgramException("magic header should be 'mzcache'!")
            End If

            Dim nsize As Integer = buffer.ReadInt32
            Dim data As PeakMs2() = New PeakMs2(nsize - 1) {}

            For i As Integer = 0 To nsize - 1
                data(i) = mzPack.CastToPeakMs2(Serialization.ReadScanMs2(file:=buffer))
            Next

            Return data
        End Using
    End Function

    <ExportAPI("write.cache")>
    Public Function writeCache(ions As PeakMs2(), file As String) As Boolean
        Using buffer As New BinaryDataWriter(file.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)) With {
            .ByteOrder = ByteOrder.BigEndian
        }
            Call buffer.Write("mzcache", BinaryStringFormat.ZeroTerminated)
            Call buffer.Write(ions.Length)

            For Each ion As PeakMs2 In ions
                Call Serialization.WriteBuffer(ion.Scan2, file:=buffer)
            Next
        End Using

        Return True
    End Function

    ''' <summary>
    ''' write version 2 format of the mzpack by default
    ''' </summary>
    ''' <param name="mzpack"></param>
    ''' <param name="file"></param>
    ''' <param name="headerSize">
    ''' negative value or zero means auto-evaluated via the different file size
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("write.mzPack")>
    Public Function writeMzpack(mzpack As mzPack, file As Object,
                                Optional version As Integer = 2,
                                Optional headerSize As Long = -1,
                                Optional env As Environment = Nothing) As Object

        Dim filestream As [Variant](Of Stream, Message) = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)
        Dim println As Action(Of Object) = env.WriteLineHandler

        If filestream Like GetType(Message) Then
            Return filestream.TryCast(Of Message)
        End If

        Return mzpack.Write(filestream.TryCast(Of Stream), version, headerSize, Sub(s) println(s))
    End Function

    <ExportAPI("write.cdf")>
    Public Function writeToCDF(mzpack As mzPack,
                               file As Object,
                               Optional Ms2Only As Boolean = False,
                               Optional env As Environment = Nothing) As Object

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
    <RApiReturn(GetType(mzPack))>
    Public Function Open(file As Object, Optional env As Environment = Nothing) As Object
        If file Is Nothing Then
            Return Internal.debug.stop("the required file object can not be nothing!", env)
        End If
        If TypeOf file Is String Then
            Dim mzpack As mzPack = openFromFile(file)

            If mzpack.source.StringEmpty Then
                mzpack.source = DirectCast(file, String).FileName
            End If

            Return mzpack
        Else
            Return Internal.debug.stop(New NotImplementedException($"unsure for how to handling '{file.GetType.FullName}' as a file stream for read mzpack data!"), env)
        End If
    End Function

    ''' <summary>
    ''' open mzpack data from a raw data file in xml file format.
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="prefer">
    ''' the prefer file format used when the given <paramref name="file"/> its extension
    ''' suffix name is ``XML``. value of this parameter could be imzml/mzml/mzxml
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("open_mzpack.xml")>
    Public Function openFromFile(file As String, Optional prefer As String = Nothing) As mzPack
        If file.ExtensionSuffix("mzXML", "mzML", "imzML", "xml") Then
            Return Converter.LoadRawFileAuto(
                xml:=file,
                prefer:=prefer,
                progress:=AddressOf VBDebugger.EchoLine
            )
        ElseIf file.ExtensionSuffix("mgf", "msp") Then
            Return Converter.LoadAsciiFileAuto(file)
#If NET48 Then
        ElseIf file.ExtensionSuffix("raw") Then
            Using msRaw As New MSFileReader(file)
                Return msRaw.LoadFromXRaw
            End Using
#End If
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

    ''' <summary>
    ''' set thumbnail image to the raw data file
    ''' </summary>
    ''' <param name="mzpack"></param>
    ''' <param name="thumb">
    ''' Thumbnail image object data can be a gdi+ image or 
    ''' bitmap or a gdi+ canvas object in type <see cref="ImageData"/>.
    ''' And also this parameter could be a lambda function that
    ''' could be used for invoke and generates the image data.
    ''' </param>
    ''' <returns>
    ''' returns a modified mzpack data object with Thumbnail 
    ''' property data has been updated.
    ''' </returns>
    ''' <remarks>
    ''' the parameter value of the <paramref name="thumb"/> lambda
    ''' function will be <paramref name="mzpack"/> parameter value
    ''' input
    ''' </remarks>
    <ExportAPI("setThumbnail")>
    <RApiReturn(GetType(mzPack))>
    Public Function setMzpackThumbnail(mzpack As mzPack, thumb As Object, Optional env As Environment = Nothing) As Object
        If mzpack Is Nothing Then
            Return Internal.debug.stop("the required mzpack data object can not be nothing!", env)
        End If
        If thumb Is Nothing Then
            mzpack.Thumbnail = Nothing
            Return mzpack
        ElseIf thumb.GetType.ImplementInterface(Of RFunction) Then
            thumb = DirectCast(thumb, RFunction).Invoke(env, InvokeParameter.CreateLiterals(mzpack))
        End If

        If TypeOf thumb Is Message Then
            Return thumb
        End If

        If TypeOf thumb Is GraphicsData Then
            thumb = DirectCast(thumb, GraphicsData).AsGDIImage
        ElseIf (Not TypeOf thumb Is Image) AndAlso (Not TypeOf thumb Is Bitmap) Then
            Return Message.InCompatibleType(GetType(Image), thumb.GetType, env)
        End If

        mzpack.Thumbnail = DirectCast(thumb, Image)

        Return mzpack
    End Function

    ''' <summary>
    ''' get all ms1 scan data points
    ''' </summary>
    ''' <param name="mzpack"></param>
    ''' <returns></returns>
    <ExportAPI("ms1_scans")>
    Public Function Ms1ScanPoints(mzpack As mzPack) As ms1_scan()
        Return mzpack.GetAllScanMs1.ToArray
    End Function

    ''' <summary>
    ''' get a overview ms1 spectrum data from the mzpack raw data
    ''' </summary>
    ''' <param name="mzpack"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="cutoff"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
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
    ''' <param name="centroid">
    ''' and also convert the data to centroid mode? 
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("ms2_peaks")>
    <RApiReturn(GetType(PeakMs2))>
    Public Function Ms2ScanPeaks(mzpack As mzPack,
                                 Optional precursorMz As Double = Double.NaN,
                                 Optional tolerance As Object = "ppm:30",
                                 Optional tag_source As Boolean = True,
                                 Optional centroid As Boolean = False,
                                 Optional norm As Boolean = False,
                                 Optional filter_empty As Boolean = True,
                                 Optional into_cutoff As Object = 0,
                                 Optional env As Environment = Nothing) As Object

        Dim ms2peaks As PeakMs2()

        If precursorMz.IsNaNImaginary Then
            ms2peaks = mzpack.GetMs2Peaks.ToArray
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
                .Select(Function(mz2)
                            Return mzPack.CastToPeakMs2(mz2, file:=mzpack.source)
                        End Function) _
                .ToArray

            ms2peaks = ms2_xic
        End If

        If into_cutoff > 0 Then
            Dim cutoff As New RelativeIntensityCutoff(into_cutoff)

            For Each peak As PeakMs2 In ms2peaks
                peak.mzInto = cutoff.Trim(peak.mzInto)
            Next
        End If

        If centroid Then
            Dim ms2diff As Tolerance = Math.getTolerance("da:0.3", env)
            Dim cutoff As New RelativeIntensityCutoff(0.01)

            For Each peak As PeakMs2 In ms2peaks
                peak.mzInto = peak.mzInto _
                    .Centroid(ms2diff, cutoff) _
                    .ToArray
            Next
        End If

        If norm Then
            For Each peak As PeakMs2 In ms2peaks
                If peak.mzInto.Length = 0 Then
                    Continue For
                End If

                Dim max As Double = peak.mzInto.Select(Function(i) i.intensity).Max
                Dim ms2 As ms2() = peak.mzInto _
                    .Select(Function(i)
                                Return New ms2 With {
                                    .mz = i.mz,
                                    .Annotation = i.Annotation,
                                    .intensity = i.intensity / max
                                }
                            End Function) _
                    .ToArray

                peak.mzInto = ms2
            Next
        End If

        If filter_empty Then
            ms2peaks = ms2peaks _
                .Where(Function(a) Not a.mzInto.IsNullOrEmpty) _
                .ToArray
        End If

        Return ms2peaks.uniqueReference(tag_source)
    End Function

    <Extension>
    Private Function uniqueReference(ms2peaks As PeakMs2(), tag_source As Boolean) As PeakMs2()
        Dim unique As String() = ms2peaks _
          .Select(Function(p)
                      If tag_source Then
                          Return $"{p.file} {p.lib_guid}"
                      Else
                          Return p.lib_guid
                      End If
                  End Function) _
          .uniqueNames

        For i As Integer = 0 To unique.Length - 1
            ms2peaks(i).lib_guid = unique(i)
            ms2peaks(i).lib_guid = ms2peaks(i).lib_guid.Replace("ms2.mzPack#", "").Replace("queryMs2.mzPack#", "")

            Dim src As String = ms2peaks(i).lib_guid.Match(".+?\.mzPack[# ]")

            If Not src.StringEmpty Then
                ms2peaks(i).file = src.Trim("#"c, " "c)
                ms2peaks(i).lib_guid = ms2peaks(i).lib_guid.Replace(ms2peaks(i).lib_guid.Match(".+\.mzPack[# ]"), "")

                If tag_source Then
                    ms2peaks(i).lib_guid = $"{ms2peaks(i).file} {ms2peaks(i).lib_guid}".Trim
                End If
            End If
        Next

        Return ms2peaks
    End Function

    ''' <summary>
    ''' convert assembly file to mzpack format data object
    ''' </summary>
    ''' <param name="assembly"></param>
    ''' <param name="env"></param>
    ''' <param name="modtime">
    ''' [GCxGC]
    ''' the modulation time of the chromatographic run. 
    ''' modulation period in time unit 'seconds'.
    ''' </param>
    ''' <param name="sample_rate">
    ''' [GCxGC]
    ''' the sampling rate of the equipment.
    ''' If sam_rate is missing, the sampling rate is calculated by the dividing 1 by
    ''' the difference of two adjacent scan time.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("as.mzpack")>
    <RApiReturn(GetType(mzPack))>
    Public Function ToMzPack(assembly As Object,
                             Optional modtime As Double = -1,
                             Optional sample_rate As Double = Double.NaN,
                             Optional env As Environment = Nothing) As Object

        If TypeOf assembly Is netCDFReader Then
            Return GC2Dimensional.ToMzPack(
                agilentGC:=assembly,
                modtime:=modtime,
                sam_rate:=sample_rate
            )
        Else
            Return Message.InCompatibleType(GetType(netCDFReader), assembly.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' do mass calibration
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="mzdiff">
    ''' mass tolerance in delta dalton
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("mass_calibration")>
    <RApiReturn(GetType(mzPack))>
    Public Function MassCalibration(data As mzPack, Optional mzdiff As Double = 0.1, Optional env As Environment = Nothing) As Object
        Return data.MassCalibration(da:=mzdiff)
    End Function
End Module
