﻿#Region "Microsoft.VisualBasic::742c60a8598ad15240c69e2e90df1893, Rscript\Library\mzkit_app\src\mzkit\assembly\MzWeb.vb"

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

    '   Total Lines: 1183
    '    Code Lines: 748 (63.23%)
    ' Comment Lines: 287 (24.26%)
    '    - Xml Docs: 91.64%
    ' 
    '   Blank Lines: 148 (12.51%)
    '     File Size: 48.43 KB


    ' Module MzWeb
    ' 
    '     Function: BPC, GetChromatogram, getMs1PointTable, loadStream, loadXcmsRData
    '               MassCalibration, Ms1Peaks, Ms1ScanPoints, Ms2ScanPeaks, Open
    '               openFile, openFromFile, parse_base64, parseScanMsBuffer, readCache
    '               setMzpackThumbnail, TIC, ToMzPack, uniqueReference, writeCache
    '               writeMzpack, writeStream, writeToCDF
    ' 
    '     Sub: Main, WriteCache
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
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Components.Interface
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports ChromatogramTick = BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram.ChromatogramTick
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal
Imports SIMDAdd = Microsoft.VisualBasic.Math.SIMD.Add

#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

''' <summary>
''' biodeep mzweb data viewer raw data file helper
''' </summary>
<Package("mzweb")>
<RTypeExport("ms1_data", GetType(ms1_scan))>
Module MzWeb

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(ms1_scan()), AddressOf getMs1PointTable)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Public Function getMs1PointTable(data As ms1_scan(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}

        Call df.add("mz", data.Select(Function(i) i.mz))
        Call df.add("scan_time", data.Select(Function(i) i.scan_time))
        Call df.add("intensity", data.Select(Function(i) i.intensity))

        Return df
    End Function

    ''' <summary>
    ''' load the xcms cache dataset
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' this function get a set of the spectrum peak ms2 data from the given R dataset
    ''' </returns>
    ''' <example>
    ''' loadXcmsRData(file = "./data_sample.RData");
    ''' </example>
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
    ''' open a mzpack data file lazy reader
    ''' </summary>
    ''' <param name="file">
    ''' the file path to the mzpack data file
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("open")>
    Public Function openFile(file As String, Optional env As Environment = Nothing) As IMzPackReader
        Return MzPackAccess.open_mzpack(file, env)
    End Function

    ''' <summary>
    ''' Get TIC from the mzpack layer reader
    ''' </summary>
    ''' <param name="x">should be a file reader to a mzpack file or the mzpack in-memory data object.</param>
    ''' <returns></returns>
    ''' <example>
    ''' let rawdata = mzweb::open(file = "./LCMS-rawdata.mzPack");
    ''' let tic = mzweb::TIC(rawdata);
    ''' 
    ''' plot(tic);
    ''' </example>
    <ExportAPI("TIC")>
    <RApiReturn(GetType(ChromatogramTick))>
    Public Function TIC(x As Object, Optional env As Environment = Nothing) As Object
        Dim ticks As ChromatogramTick()

        If x Is Nothing Then
            Call env.AddMessage("the given rawdata file object is nothing for load TIC data!")
            Return Nothing
        End If

        If x.GetType.ImplementInterface(Of IMzPackReader) Then
            Dim mzpack As IMzPackReader = x
            Dim keys As String() = mzpack.EnumerateIndex.ToArray

            ticks = keys _
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
        ElseIf TypeOf x Is mzPack Then
            ticks = DirectCast(x, mzPack).MS _
                .Select(Function(a)
                            Return New ChromatogramTick(a.rt, a.into.Sum)
                        End Function) _
                .ToArray
        Else
            Return Message.InCompatibleType(GetType(mzPack), x.GetType, env)
        End If

        Return ticks
    End Function

    ''' <summary>
    ''' Get BPC from the mzpack layer reader
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("BPC")>
    <RApiReturn(GetType(ChromatogramTick))>
    Public Function BPC(x As Object, Optional env As Environment = Nothing) As Object
        Dim ticks As ChromatogramTick()

        If x Is Nothing Then
            Call env.AddMessage("the given rawdata file object is nothing for load TIC data!")
            Return Nothing
        End If

        If x.GetType.ImplementInterface(Of IMzPackReader) Then
            Dim mzpack As IMzPackReader = x
            Dim keys As String() = mzpack.EnumerateIndex.ToArray

            ticks = keys _
                .Select(Function(i)
                            Dim scan_time As Double
                            Dim BPCpoint As Double

                            Call mzpack.ReadChromatogramTick(i, scan_time, BPCpoint, 0)

                            Return New ChromatogramTick With {
                                .Time = scan_time,
                                .Intensity = BPCpoint
                            }
                        End Function) _
                .ToArray
        ElseIf TypeOf x Is mzPack Then
            ticks = DirectCast(x, mzPack).MS _
                .Select(Function(a)
                            Return New ChromatogramTick(a.rt, If(a.into.IsNullOrEmpty, 0, a.into.Max))
                        End Function) _
                .ToArray
        Else
            Return Message.InCompatibleType(GetType(mzPack), x.GetType, env)
        End If

        Return ticks
    End Function

    ''' <summary>
    ''' load chromatogram data from the raw file data
    ''' </summary>
    ''' <param name="scans">
    ''' the scan data object that reads from the mzXML/mzML/mzPack raw data file
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' the chromatogram data wrapper of TIC/BPC
    ''' </returns>
    ''' <example>
    ''' let rawdata = mzweb::open.mzpack(file = "./lcms-rawdata.mzPack");
    ''' let chromatogram = rawdata |> load.chromatogram();
    ''' 
    ''' </example>
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
                Return ChromatogramBuffer.GetChromatogram(scanPip.populates(Of scan)(env))
            ElseIf scanPip.elementType Like GetType(mzML.spectrum) Then
                Return ChromatogramBuffer.GetChromatogram(scanPip.populates(Of mzML.spectrum)(env))
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
    ''' <param name="x"></param>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <remarks>
    ''' this method used for create ascii text package data for the biodeep
    ''' web application js code to read the ms rawdata.
    ''' </remarks>
    ''' <returns></returns>
    ''' <example>
    ''' let rawdata = open.mzpack(file = "./rawdata.mzPack");
    ''' let ms1 = [rawdata]::MS;
    ''' 
    ''' write.text_cache(ms1, file = "./msdata.txt");
    ''' </example>
    <ExportAPI("write.text_cache")>
    Public Function writeStream(<RRawVectorArgument> x As Object,
                                Optional file As Object = Nothing,
                                Optional tabular As Boolean = False,
                                Optional env As Environment = Nothing) As Object
        Dim stream As Stream
        Dim scans As pipeline = pipeline.TryCreatePipeline(Of ScanMS1)(x, env)

        If scans.isError Then
            Return scans.getError
        Else
            Dim buf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

            If buf Like GetType(Message) Then
                Return buf.TryCast(Of Message)
            Else
                stream = buf.TryCast(Of Stream)
            End If
        End If

        If tabular Then
            Call scans.populates(Of ScanMS1)(env).WriteTabularCache(stream)
        Else
            Call scans.populates(Of ScanMS1)(env).Write(stream)
        End If

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
    ''' <param name="file">The cache file path or the file binary data buffer object</param>
    ''' <returns>
    ''' A vector of the mzkit peakms2 object
    ''' </returns>
    <ExportAPI("read.cache")>
    <RApiReturn(GetType(PeakMs2))>
    Public Function readCache(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim is_filepath As Boolean = False
        Dim buf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env, is_filepath:=is_filepath)

        If buf Like GetType(Message) Then
            Return buf.TryCast(Of Message)
        End If

        Using rd As New BinaryDataReader(buf.TryCast(Of Stream)) With {.ByteOrder = ByteOrder.BigEndian}
            If rd.ReadString(BinaryStringFormat.ZeroTerminated) <> mzcacheMagic Then
                Throw New InvalidProgramException($"magic header should be '{mzcacheMagic}'!")
            End If

            Dim nsize As Integer = rd.ReadInt32
            Dim data As PeakMs2() = New PeakMs2(nsize - 1) {}
            Dim meta As String()

            If is_filepath Then
                Dim filepath = CLRVector.asCharacter(file).First
                Dim dir As String = filepath.ParentPath
                Dim jsonl As String = $"{dir}/{filepath.BaseName}.jsonl"

                If jsonl.FileExists Then
                    meta = jsonl.ReadAllLines
                Else
                    meta = New String(nsize - 1) {}
                End If
            Else
                meta = New String(nsize - 1) {}
            End If

            For i As Integer = 0 To nsize - 1
                data(i) = mzPack.CastToPeakMs2(Serialization.ReadScanMs2(file:=rd))

                If is_filepath Then
                    Dim json_str As String = meta.ElementAtOrDefault(i, "{}")

                    If Not json_str.StringEmpty(, True) Then
                        With If(json_str.LoadJSON(Of AnnotationMetadata)(throwEx:=False), New AnnotationMetadata)
                            data(i).meta = .meta

                            If Not .annotation.IsNullOrEmpty Then
                                Dim offset As Integer = 0
                                Dim peaks As ms2() = data(i).mzInto

                                For Each str As String In .annotation
                                    peaks(offset).Annotation = str
                                    offset += 1
                                Next
                            End If
                        End With

                        If Not data(i).meta.IsNullOrEmpty Then
                            ' 20241105
                            ' due to the reason of scanms2 data has no file source attribute
                            ' so we needs to restore the file source information from the metadata
                            ' try get file source information via tags:
                            ' source, file, rawdata, filename, etc something
                            Dim m As Dictionary(Of String, String) = data(i).meta

                            ' some possible tag name that could be used for represents 
                            ' of the source file name metadata information.
                            Static tags As String() = {
                                "source", "file", "rawdata", "filename",
                                "datafile",
                                "data_raw",
                                "data"
                            }

                            For Each name As String In tags
                                If m.ContainsKey(name) Then
                                    data(i).file = m(name)
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                End If
            Next

            Return data
        End Using
    End Function

    Const mzcacheMagic As String = "mzcache"

    ''' <summary>
    ''' Write the ms2 spectrum collection into binary cache file
    ''' </summary>
    ''' <param name="ions">Should be a collection of the mzkit <see cref="PeakMs2"/> object.</param>
    ''' <param name="file">The file path to save the spectrum data collection as cache file.</param>
    ''' <returns>
    ''' this function returns a logical value for indicate operation is success or not.
    ''' </returns>
    <ExportAPI("write.cache")>
    <RApiReturn(TypeCodes.boolean)>
    Public Function writeCache(<RRawVectorArgument>
                               ions As Object, file As Object,
                               Optional tag_filesource As Boolean = True,
                               Optional env As Environment = Nothing) As Object

        Dim is_filepath As Boolean = False
        Dim buf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env, is_filepath:=is_filepath)
        Dim pool As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(ions, env)

        If buf Like GetType(Message) Then
            Return buf.TryCast(Of Message)
        End If
        If pool.isError Then
            Return pool.getError
        End If
        If tag_filesource Then
            Call VBDebugger.EchoLine("the source file name of the spectrum data will also tagged with the guid of the spectrum as unique id.")
        Else
            Call VBDebugger.EchoLine("the unique reference id of each spectrum data will not be changed.")
        End If

        Using buffer As New BinaryDataWriter(buf.TryCast(Of Stream)) With {.ByteOrder = ByteOrder.BigEndian}
            Dim all_spec As PeakMs2() = pool _
                .populates(Of PeakMs2)(env) _
                .ToArray
            Dim bar As Tqdm.ProgressBar = Nothing
            Dim metadata As New List(Of String)

            Call buffer.Write(mzcacheMagic, BinaryStringFormat.ZeroTerminated)
            Call buffer.Write(all_spec.Length)

            For Each ion As PeakMs2 In Tqdm.Wrap(all_spec, bar:=bar)
                Call bar.SetLabel(ion.lib_guid)
                Call Serialization.WriteBuffer(ion.Scan2(tag_filesource), file:=buffer)

                ' 20241022
                ' scanms2 can not save the metadata into cache binary 
                ' so an external json list file was generated for
                ' save the spectrum metadata
                ' for avoid the possible data missing problem
                If is_filepath Then
                    Call metadata.Add(ion.GetAnnotationJsonModel.GetJson(simpleDict:=True))
                End If
            Next

            If is_filepath Then
                Dim filepath = CLRVector.asCharacter(file).First
                Dim dir = filepath.ParentPath
                Dim filename = filepath.BaseName & ".jsonl"

                Call metadata.SaveTo($"{dir}/{filename}")
            End If
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
    Public Function Open(file As Object, Optional verbose As Boolean = True, Optional env As Environment = Nothing) As Object
        If file Is Nothing Then
            Return RInternal.debug.stop("the required file object can not be nothing!", env)
        End If
        If TypeOf file Is String Then
            Dim mzpack As mzPack = openFromFile(file, verbose:=verbose, env:=env)

            If mzpack.source.StringEmpty Then
                mzpack.source = DirectCast(file, String).FileName
            End If

            Return mzpack
        ElseIf TypeOf file Is Stream Then
            Dim stream As Stream = file
            Return mzPack.ReadAll(file:=stream, verbose:=verbose)
        Else
            Return RInternal.debug.stop(New NotImplementedException($"unsure for how to handling '{file.GetType.FullName}' as a file stream for read mzpack data!"), env)
        End If
    End Function

    ''' <summary>
    ''' open mzpack data from a raw data file in xml file format.
    ''' </summary>
    ''' <param name="file">the file path to the xml rawdata file</param>
    ''' <param name="prefer">
    ''' the prefer file format used when the given <paramref name="file"/> its extension
    ''' suffix name is ``XML``. value of this parameter could be imzml/mzml/mzxml
    ''' </param>
    ''' <returns>A mzkit mzpack rawdata object</returns>
    <ExportAPI("open_mzpack.xml")>
    Public Function openFromFile(file As String,
                                 Optional prefer As String = Nothing,
                                 Optional da As Double = 0.001,
                                 Optional noise_cutoff As Double = 0.0001,
                                 Optional verbose As Boolean = True,
                                 Optional env As Environment = Nothing) As mzPack

        Dim println As Action(Of String) = AddressOf VBDebugger.EchoLine

        If Not verbose Then
            println = Sub()
                          ' do nothing
                      End Sub
        End If

        If file.ExtensionSuffix("mzXML", "mzML", "imzML", "xml") Then
            Return Converter.LoadRawFileAuto(
                xml:=file,
                prefer:=prefer,
                progress:=If(verbose, println, Nothing),
                tolerance:=$"da:{da}",
                intocutoff:=noise_cutoff
            )
        ElseIf file.ExtensionSuffix("mgf", "msp") Then
            Return Converter.LoadAsciiFileAuto(file)

        ElseIf file.ExtensionSuffix("raw") Then
            Using msRaw As New MSFileReader(file)
                Return msRaw.LoadFromXcaliburRaw
            End Using

        ElseIf file.ExtensionSuffix("cdf") Then
            Using cdf As New netCDFReader(file)
                If cdf.IsLecoGCMS Then
                    Dim sig As mzPack = GCMSConvertor.ConvertGCMS(cdf, println)
                    sig.source = file.FileName
                    Return sig
                Else
                    ' convert MSI cdf to mzpack
                    ' cdf for save MS-imaging
                    Return New mzPack With {
                        .MS = cdf.CreateMs1.ToArray,
                        .Application = FileApplicationClass.MSImaging,
                        .source = file.FileName,
                        .Scanners = New Dictionary(Of String, ChromatogramOverlapList),
                        .Chromatogram = Nothing,
                        .Thumbnail = Nothing
                    }
                End If
            End Using
        Else
            Using stream As Stream = file.Open(FileMode.Open, doClear:=False, [readOnly]:=True, verbose:=verbose)
                Return mzPack.ReadAll(file:=stream, verbose:=verbose)
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
            Return RInternal.debug.stop("the required mzpack data object can not be nothing!", env)
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
    ''' <example>
    ''' let rawdata = open.mzpack(file = "./rawdata.mzPack");
    ''' let ms1 = rawdata |> ms1_scans();
    ''' 
    ''' # get xic liked data
    ''' let xic = rawdata |> ms1_scans(mz = 999.0911, tolerance = "da:0.01");
    ''' let mz_vec = [xic]::mz;
    ''' 
    ''' print(as.data.frame(xic));
    ''' 
    ''' bitmap(file = "./mz_histogram.png") {
    '''     plot(hist(mz_vec));
    ''' }
    ''' </example>
    <ExportAPI("ms1_scans")>
    <RApiReturn(GetType(ms1_scan))>
    Public Function Ms1ScanPoints(mzpack As mzPack,
                                  Optional mz As Double? = Nothing,
                                  Optional tolerance As Object = "ppm:20",
                                  Optional env As Environment = Nothing) As Object

        Dim mzdiff = Math.getTolerance(tolerance, env, [default]:="ppm:20")

        If mzdiff Like GetType(Message) Then
            Return mzdiff.TryCast(Of Message)
        End If

        If mz Is Nothing Then
            Return mzpack.GetAllScanMs1.ToArray
        Else
            Dim mzVal As Double = mz
            Dim err As Tolerance = mzdiff.TryCast(Of Tolerance)
            Dim xic As ms1_scan() = mzpack.GetAllScanMs1 _
                .AsParallel _
                .Where(Function(i) err(i.mz, mzVal)) _
                .ToArray

            Return xic
        End If
    End Function

    ''' <summary>
    ''' get a overview ms1 spectrum data from the mzpack raw data
    ''' </summary>
    ''' <param name="mzpack">
    ''' usually be the <see cref="mzPack"/> rawdata object, or a general <see cref="MzMatrix"/> object.
    ''' </param>
    ''' <param name="tolerance">The mass tolerance error</param>
    ''' <param name="cutoff">intensity cutoff percentage value for removes the noised liked peaks.</param>
    ''' <param name="ionset">
    ''' A numeric vector for make subset of the ion features 
    ''' which is extract from the input mzpack rawdata file
    ''' object.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>A ms peaks object</returns>
    ''' <example>
    ''' let rawdata = open.mzpack("/path/to/rawdata.mzPack");
    ''' let ms1 = ms1_peaks(rawdata, tolerance = "da:0.001", 
    '''      cutoff = 0.01, 
    '''      ionset = [347.56 238.3712 631.31 131.23]);
    ''' </example>
    <ExportAPI("ms1_peaks")>
    <RApiReturn(GetType(LibraryMatrix))>
    Public Function Ms1Peaks(mzpack As Object,
                             Optional tolerance As Object = "da:0.001",
                             Optional cutoff As Double = 0.05,
                             <RRawVectorArgument>
                             Optional ionset As Object = Nothing,
                             Optional env As Environment = Nothing) As Object

        Dim mzdiff = Math.getTolerance(tolerance, env)
        Dim mzsubset As Double() = CLRVector.asNumeric(ionset)

        If mzdiff Like GetType(Message) Then
            Return mzdiff.TryCast(Of Message)
        End If

        Dim source_label As String
        Dim allMassPeaks As ms2()

        If mzpack Is Nothing Then
            Call env.AddMessage("the given rawdata object is nothing.")
            Return Nothing
        End If

        If TypeOf mzpack Is mzPack Then
            source_label = DirectCast(mzpack, mzPack).source
            allMassPeaks = DirectCast(mzpack, mzPack).MS _
                .Select(Function(scan) scan.GetMs) _
                .IteratesALL _
                .ToArray
        ElseIf TypeOf mzpack Is MzMatrix Then
            Dim mat As MzMatrix = DirectCast(mzpack, MzMatrix)
            Dim intensity_vec As Double() = New Double(mat.featureSize - 1) {}

            source_label = "mzImage matrix"

            For Each spot In Tqdm.Wrap(mat.matrix)
                intensity_vec = SIMDAdd.f64_op_add_f64(intensity_vec, spot.intensity)
            Next

            Return New LibraryMatrix(mat.mz, intensity_vec) With {
                .name = source_label
            }
        Else
            Return Message.InCompatibleType(GetType(mzPack), mzpack.GetType, env)
        End If

        Dim ms As ms2()

        If Not mzsubset.IsNullOrEmpty Then
            Dim findMz As Tolerance = New DAmethod(0.05)
            Dim peaksubset As ms2() = allMassPeaks _
                .AsParallel _
                .Where(Function(a)
                           Return mzsubset.Any(Function(mzi) findMz(mzi, a.mz))
                       End Function) _
                .ToArray

            allMassPeaks = peaksubset
        End If

        ms = allMassPeaks _
            .Centroid(mzdiff.TryCast(Of Tolerance), New RelativeIntensityCutoff(cutoff)) _
            .ToArray

        Return New LibraryMatrix With {
            .centroid = True,
            .ms2 = ms,
            .name = source_label & " MS1"
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
    ''' <param name="rt_window">
    ''' the rt range for filter of the ms2 spectrum data exports, 
    ''' should be a numeric vector that consists with two elements
    ''' for specific the range min and range max. rt data should 
    ''' be in data unit of seconds.
    ''' </param>
    ''' <param name="tag_source">
    ''' tag the source reference to the metadata of each spectrum data object?
    ''' </param>
    ''' <param name="loadProductTree">
    ''' Load MSn product tree.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' metadata of the spectrum source reference includes data slots:
    ''' 
    ''' 1. ``source``, the file name of the spectrum rawdata file source
    ''' 2. ``precursor``, the precursor ion mz of the spectrum data
    ''' 3. ``rt``, the retention time of the spectrum data object
    ''' </remarks>
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
                                 <RRawVectorArgument()>
                                 Optional rt_window As Object = Nothing,
                                 Optional loadProductTree As Boolean = False,
                                 Optional env As Environment = Nothing) As Object

        Dim ms2peaks As PeakMs2()

        If precursorMz.IsNaNImaginary Then
            ms2peaks = mzpack.GetMs2Peaks(loadProductTree).ToArray
        Else
            Dim mzErr = Math.getTolerance(tolerance, env)

            If mzErr Like GetType(Message) Then
                Return mzErr.TryCast(Of Message)
            End If

            Dim mzdiff As Tolerance = mzErr.TryCast(Of Tolerance)
            Dim ms2_xic = mzpack.MS _
                .Select(Function(d) d.products) _
                .IteratesALL _
                .Where(Function(scan)
                           Return mzdiff(scan.parentMz, precursorMz)
                       End Function) _
                .Select(Function(mz2)
                            Return mzPack.CastToPeakMs2(
                                ms2:=mz2,
                                file:=mzpack.source,
                                loadProductTree:=loadProductTree
                            )
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

        Dim rtwin As Double() = CLRVector.asNumeric(rt_window)

        If Not rtwin.IsNullOrEmpty Then
            Dim window As New DoubleRange(rtwin)

            If window.Length > 0 Then
                ms2peaks = ms2peaks _
                    .Where(Function(a) window.IsInside(a.rt)) _
                    .ToArray
            End If
        End If

        Return ms2peaks.uniqueReference(tag_source)
    End Function

    ''' <summary>
    ''' make unique of the spectrum reference id
    ''' </summary>
    ''' <param name="ms2peaks"></param>
    ''' <param name="tag_source"></param>
    ''' <returns></returns>
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
          .UniqueNames

        For i As Integer = 0 To unique.Length - 1
            ms2peaks(i).lib_guid = unique(i)
            ms2peaks(i).lib_guid = ms2peaks(i).lib_guid _
                .Replace("ms2.mzPack#", "") _
                .Replace("queryMs2.mzPack#", "")

            Dim src As String = ms2peaks(i).lib_guid.Match(".+?\.mzPack[# ]")

            If Not src.StringEmpty Then
                ms2peaks(i).file = src.Trim("#"c, " "c)
                ms2peaks(i).lib_guid = ms2peaks(i).lib_guid _
                    .Replace(ms2peaks(i).lib_guid _
                    .Match(".+\.mzPack[# ]"), "")

                If tag_source Then
                    ms2peaks(i).lib_guid = $"{ms2peaks(i).file} {ms2peaks(i).lib_guid}".Trim

                    If ms2peaks(i).meta Is Nothing Then
                        ms2peaks(i).meta = New Dictionary(Of String, String)
                    End If

                    ms2peaks(i).meta("source") = ms2peaks(i).file
                    ms2peaks(i).meta("precursor") = ms2peaks(i).mz.ToString("F4")
                    ms2peaks(i).meta("rt") = (ms2peaks(i).rt / 60).ToString("F2") & "min"
                End If
            End If
        Next

        Return ms2peaks
    End Function

    ''' <summary>
    ''' convert assembly file to mzpack format data object
    ''' </summary>
    ''' <param name="assembly"></param>
    ''' <param name="args">
    ''' 1. modtime: [GCxGC] the modulation time of the chromatographic run. 
    '''    modulation period in time unit 'seconds'.
    ''' 2. sample_rate: [GCxGC] the sampling rate of the equipment.
    '''    If sam_rate is missing, the sampling rate is calculated by the dividing 1 by
    '''    the difference of two adjacent scan time.
    ''' 3. imzml: [MS-Imaging] the pixel spot scan metadata collection for
    '''    read ms data from the ibd file, the corresponding assembly object should
    '''    be a <see cref="ibdReader"/> object
    ''' 4. dims: [MS-Imaging] the canvas dimension size value for the ms-imaging
    '''    heatmap rendering
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("as.mzpack")>
    <RApiReturn(GetType(mzPack))>
    Public Function ToMzPack(assembly As Object,
                             <RListObjectArgument>
                             Optional args As list = Nothing,
                             Optional env As Environment = Nothing) As Object

        If TypeOf assembly Is netCDFReader Then
            Dim modtime As Double = args.getValue({"modtime", "modulation"}, env, [default]:=-1.0)
            Dim sample_rate As Double = args.getValue({"sample_rate", "sample.rate"}, env, [default]:=Double.NaN)

            Return GC2Dimensional.ToMzPack(
                agilentGC:=assembly,
                modtime:=modtime,
                sam_rate:=sample_rate
            )
        ElseIf TypeOf assembly Is ibdReader Then
            Dim ibd As ibdReader = DirectCast(assembly, ibdReader)

            Throw New NotImplementedException
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

    ''' <summary>
    ''' Parse the given network base64 data as spectrum
    ''' </summary>
    ''' <param name="mz">a character vector of the base64 string for the ms2 spectrum ion peaks</param>
    ''' <param name="intensity">a character vector of the base64 string for the corresponding ion peaks intensity value.</param>
    ''' <param name="id">a character vector of the spectrum reference id</param>
    ''' <param name="auto_scalar">
    ''' returns a scalar spectrum object if the input data just contains one spectrum data or not?
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' this function will populate a set of the ms2 spectrum data object
    ''' that which was parsed from the given base64 data collection.
    ''' </returns>
    <ExportAPI("parse_base64")>
    <RApiReturn(GetType(LibraryMatrix))>
    Public Function parse_base64(<RRawVectorArgument> mz As Object,
                                 <RRawVectorArgument> intensity As Object,
                                 <RRawVectorArgument>
                                 Optional id As Object = Nothing,
                                 Optional auto_scalar As Boolean = True,
                                 Optional env As Environment = Nothing) As Object

        Dim mz_str As String() = CLRVector.asCharacter(mz)
        Dim into_str As String() = CLRVector.asCharacter(intensity)
        Dim id_str As String() = CLRVector.asCharacter(id)

        If mz_str.TryCount <> into_str.TryCount Then
            Return RInternal.debug.stop($"the vector size of the mz data({mz_str.Length}) should be matched with the intensity vector data({into_str.Length})!", env)
        ElseIf mz_str.TryCount = 0 Then
            Call env.AddMessage("there is no spectrum data to run base64 decoded!")
            Return Nothing
        End If

        Dim spectrum As New List(Of LibraryMatrix)

        For i As Integer = 0 To mz_str.Length - 1
            Dim spec As ms2() = SpectraEncoder _
                .Decode(mz_str(i), into_str(i)) _
                .ToArray
            Dim mat As New LibraryMatrix(
                name:=id_str.ElementAtOrDefault(i, [default]:=$"spectral_{i + 1}"),
                spectrum:=spec
            )

            Call spectrum.Add(mat)
        Next

        If auto_scalar AndAlso spectrum.Count = 1 Then
            Return spectrum(0)
        Else
            Return spectrum.ToArray
        End If
    End Function
End Module
