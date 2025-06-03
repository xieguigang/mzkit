﻿#Region "Microsoft.VisualBasic::ac0bdc10bd5d781cf9d51a7e716b089d, Rscript\Library\mzkit_app\src\mzkit\assembly\MzPackAccess.vb"

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

    '   Total Lines: 549
    '    Code Lines: 344 (62.66%)
    ' Comment Lines: 149 (27.14%)
    '    - Xml Docs: 94.63%
    ' 
    '   Blank Lines: 56 (10.20%)
    '     File Size: 21.28 KB


    ' Module MzPackAccess
    ' 
    '     Function: appendMzPack, convertTo_mzXML, denoiseMsMs, denoiseMzpack, GetMetaData
    '               getSampleTags, handlingMs2products, index, open_mzpack, open_mzwork
    '               packData, packMs1, populateMzPacks, readFileCache, removeSciexNoise
    '               scanInfo, SplitSamples, writeStream
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports std = System.Math
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' raw data accessor for the mzpack data object
''' </summary>
<Package("mzPack")>
Module MzPackAccess

    ''' <summary>
    ''' get all of the sample file data tags from target mzpack file
    ''' </summary>
    ''' <param name="mzpack"></param>
    ''' <returns></returns>
    ''' <example>
    ''' let rawdata = open.mzpack(file = "./rawdata.mzPack");
    ''' let tags = mzPack::getSampleTags(rawdata);
    ''' 
    ''' print(tags);
    ''' </example>
    <ExportAPI("getSampleTags")>
    <RApiReturn(TypeCodes.string)>
    Public Function getSampleTags(mzpack As Object, Optional env As Environment = Nothing) As Object
        If mzpack Is Nothing Then
            Return Nothing
        End If

        If TypeOf mzpack Is mzPack Then
            Return DirectCast(mzpack, mzPack).MS _
                .Select(Function(ms1) ms1.meta.TryGetValue(mzStreamWriter.SampleMetaName)) _
                .Where(Function(si) Not si.StringEmpty) _
                .Distinct _
                .ToArray
        End If

        If TypeOf mzpack Is String Then
            Dim file As New StreamPack(DirectCast(mzpack, String))
            Dim data As String() = mzStream.GetSampleTags(file)

            Return data
        ElseIf TypeOf mzpack Is Stream Then
            Dim file As New StreamPack(DirectCast(mzpack, Stream))
            Dim data As String() = mzStream.GetSampleTags(file)

            Return data
        Else
            Return Message.InCompatibleType(GetType(Stream), mzpack.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' try to split target mzpack file into multiple parts based on the sample tags
    ''' </summary>
    ''' <param name="mzpack"></param>
    ''' <returns></returns>
    <ExportAPI("split_samples")>
    Public Function SplitSamples(mzpack As String) As Object
        Dim raw As New mzStream(mzpack.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
        Dim tags = raw.SampleScans

        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' open mzwork file and then populate all of the mzpack raw data file
    ''' </summary>
    ''' <param name="mzwork"></param>
    ''' <returns>
    ''' a collection of mzpack raw data objects
    ''' </returns>
    <ExportAPI("open.mzwork")>
    <RApiReturn(GetType(mzPack))>
    Public Function populateMzPacks(mzwork As String, Optional env As Environment = Nothing) As pipeline
        Dim stdout = env.WriteLineHandler
        Dim println As Action(Of String) =
            Sub(text)
                Call stdout(text)
            End Sub
        Dim verbose As Boolean = env.globalEnvironment.options.verbose
        Dim print2 As Action(Of String, String) =
            Sub(text1, text2)
                If verbose Then Call stdout($"[{text1}] {text2}")
            End Sub
        Dim stream As IEnumerable(Of mzPack) =
            Iterator Function() As IEnumerable(Of mzPack)
                Using pack As New ZipArchive(mzwork.Open(FileMode.Open, doClear:=False, [readOnly]:=True), ZipArchiveMode.Read)
                    For Each group In ParseArchive.LoadRawGroups(zip:=pack, msg:=println)
                        For Each raw As Raw In group
                            Dim mzpack As mzPack = raw.LoadMzpack(print2, verbose).GetLoadedMzpack
                            mzpack.source = group.name
                            Yield mzpack
                        Next
                    Next
                End Using
            End Function()

        Return pipeline.CreateFromPopulator(stream)
    End Function

    ''' <summary>
    ''' open a mzwork package file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("mzwork")>
    <RApiReturn(GetType(WorkspaceAccess))>
    Public Function open_mzwork(file As Object, Optional env As Environment = Nothing) As Object
        Dim buffer = ApiArgumentHelpers.GetFileStream(file, FileAccess.Read, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        End If

        Dim println As Action(Of Object) = env.WriteLineHandler

        Return New WorkspaceAccess(
            file:=buffer.TryCast(Of Stream),
            msg:=Sub(line)
                     Call println(line)
                 End Sub)
    End Function

    ''' <summary>
    ''' ### get mzpack object from mzwork archive
    ''' 
    ''' read mzpack data from the mzwork package by a 
    ''' given raw data file name as reference id
    ''' </summary>
    ''' <param name="mzwork">a zip archive liked data package, contains multiple mzpack object</param>
    ''' <param name="fileName">the reference key for extract the mzpack data</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("readFileCache")>
    <RApiReturn(GetType(mzPack))>
    Public Function readFileCache(mzwork As WorkspaceAccess,
                                  fileName As String,
                                  Optional [single] As Boolean = False,
                                  Optional env As Environment = Nothing) As Object

        Dim verbose As Boolean = env.globalEnvironment.options.verbose
        Dim cache = mzwork.GetByFileName(fileName, verbose).ToArray

        If [single] Then
            Return cache.FirstOrDefault
        Else
            Return cache
        End If
    End Function

    ''' <summary>
    ''' open a mzpack data object reader, not read all data into memory in one time.
    ''' </summary>
    ''' <param name="file">
    ''' the file path for the mzpack file or the mzpack data object it self
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' the ms scan data can be load into memory in lazy 
    ''' require by a given scan id of the target ms1 scan
    ''' </returns>
    ''' <remarks>
    ''' a in-memory reader wrapper will be created if the given file object 
    ''' is a in-memory mzpack object itself
    ''' </remarks>
    <ExportAPI("mzpack")>
    <RApiReturn(GetType(IMzPackReader))>
    Public Function open_mzpack(file As Object, Optional env As Environment = Nothing) As Object
        Dim buffer = ApiArgumentHelpers.GetFileStream(file, FileAccess.Read, env, suppress:=True)

        If buffer Like GetType(Message) Then
            If TypeOf file Is mzPack Then
                ' wrap the in-memory data
                Return New MemoryReader(DirectCast(file, mzPack))
            Else
                Return buffer.TryCast(Of Message)
            End If
        End If

        Dim ver As Integer = buffer.TryCast(Of Stream).GetFormatVersion

        If ver = 1 Then
            Return New mzPackReader(buffer.TryCast(Of Stream))
        ElseIf ver = 2 Then
            Return New mzStream(buffer.TryCast(Of Stream))
        Else
            Return RInternal.debug.stop(New NotImplementedException("unknow version of the mzpack file format!"), env)
        End If
    End Function

    ''' <summary>
    ''' Get object list inside the MS packdata
    ''' </summary>
    ''' <param name="mzpack"></param>
    ''' <param name="env"></param>
    ''' <remarks>
    ''' show all ms1 scan id in a mzpack data object or 
    ''' show all raw data file names in a mzwork data 
    ''' package.
    ''' </remarks>
    ''' <example>
    ''' let rawdata = open.mzpack(file = "./rawdata.mzPack");
    ''' let scan_id = mzPack::ls(mzpack = rawdata);
    ''' 
    ''' print(scan_id);
    ''' </example>
    ''' <returns></returns>
    <ExportAPI("ls")>
    <RApiReturn(GetType(String))>
    Public Function index(mzpack As Object, Optional env As Environment = Nothing) As Object
        If mzpack Is Nothing Then
            Return Nothing
        ElseIf TypeOf mzpack Is mzPack Then
            Return DirectCast(mzpack, mzPack).MS _
                .Select(Function(m) m.scan_id) _
                .ToArray
        ElseIf mzpack.GetType.ImplementInterface(Of IMzPackReader) Then
            Return DirectCast(mzpack, IMzPackReader) _
                .EnumerateIndex _
                .ToArray
        ElseIf TypeOf mzpack Is WorkspaceAccess Then
            Return DirectCast(mzpack, WorkspaceAccess).ListAllFileNames
        Else
            Return Message.InCompatibleType(GetType(mzPackReader), mzpack.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' get metadata list from a specific ms1 scan
    ''' </summary>
    ''' <param name="mzpack">A mzpack data lazy reader object that created via ``mzpack`` function.</param>
    ''' <param name="index">the scan id of the target ms1 scan data</param>
    ''' <returns></returns>
    ''' <example>
    ''' let rawdata = open.mzpack(file = "./rawdata.mzPack");
    ''' let scan_id = mzPack::ls(mzpack = rawdata);
    ''' 
    ''' print(scan_id);
    ''' 
    ''' for(id in scan_id) {
    '''     str(mzPack::metadata(rawdata, id));
    ''' }
    ''' </example>
    <ExportAPI("metadata")>
    Public Function GetMetaData(mzpack As IMzPackReader, index As String) As list
        Return New list(mzpack.GetMetadata(index))
    End Function

    ''' <summary>
    ''' get ms scan information metadata list
    ''' </summary>
    ''' <param name="mzpack">A mzpack data lazy reader object that created via ``mzpack`` function.</param>
    ''' <param name="index">the scan id of the target ms1 scan data</param>
    ''' <returns></returns>
    ''' <example>
    ''' let rawdata = open.mzpack(file = "./rawdata.mzPack");
    ''' let scan_id = mzPack::ls(mzpack = rawdata);
    ''' 
    ''' print(scan_id);
    ''' 
    ''' for(id in scan_id) {
    '''     str(mzPack::scaninfo(rawdata, id));
    ''' }
    ''' </example>
    <ExportAPI("scaninfo")>
    Public Function scanInfo(mzpack As IMzPackReader, index As String) As list
        Dim scan As ScanMS1 = mzpack.ReadScan(index, skipProducts:=True)
        Dim info As New list With {
            .slots = New Dictionary(Of String, Object) From {
                {NameOf(scan.scan_id), index},
                {NameOf(scan.BPC), scan.BPC},
                {NameOf(scan.into), scan.into},
                {NameOf(scan.meta), scan.meta},
                {NameOf(scan.mz), scan.mz},
                {NameOf(scan.products), scan.products.TryCount},
                {NameOf(scan.rt), scan.rt},
                {NameOf(scan.TIC), scan.TIC}
            }
        }

        Return info
    End Function

    ''' <summary>
    ''' method for write mzpack data object as a mzXML file
    ''' </summary>
    ''' <param name="mzpack"></param>
    ''' <param name="file">the file stream to the target mzXML file to write the data</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("convertTo_mzXML")>
    <RApiReturn(GetType(Boolean))>
    Public Function convertTo_mzXML(mzpack As mzPack, file As Object, Optional env As Environment = Nothing) As Object
        Dim buffer = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        End If

        Using mzXML As New mzXMLWriter({}, {}, {}, buffer.TryCast(Of Stream))
            Call mzXML.WriteData(mzpack.MS)
        End Using

        Return True
    End Function

    ''' <summary>
    ''' pack mzkit ms2 peaks data as a mzpack data object
    ''' </summary>
    ''' <param name="data">A collection of the scan ms1 or ms2 data for pack as mzpack object</param>
    ''' <param name="timeWindow">the time slide window size for create different
    ''' data scan groups in the mzpack object. this rt data grouping window parameter is not working
    ''' when the input ms spectrum data collection is scan ms1 objects</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("packData")>
    <RApiReturn(GetType(mzPack))>
    Public Function packData(<RRawVectorArgument>
                             data As Object,
                             Optional timeWindow As Double = 1,
                             Optional pack_singleCells As Boolean = False,
                             Optional env As Environment = Nothing) As Object

        Dim peaks As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(data, env, suppress:=True)

        If peaks.isError Then
            peaks = pipeline.TryCreatePipeline(Of ScanMS1)(data, env, suppress:=True)

            If Not peaks.isError Then
                Dim scanMs1 As ScanMS1() = peaks.populates(Of ScanMS1)(env).ToArray

                If pack_singleCells Then
                    scanMs1 = (From scan As ScanMS1 In scanMs1 Order By scan.scan_id).ToArray

                    For i As Integer = 0 To scanMs1.Length - 1
                        scanMs1(i).rt = (i + 1) * timeWindow
                    Next
                End If

                Return New mzPack With {
                    .MS = scanMs1,
                    .Application = If(
                        pack_singleCells,
                        FileApplicationClass.SingleCellsMetabolomics,
                        FileApplicationClass.LCMS
                    )
                }
            End If

            Return peaks.getError
        End If

        Dim groupScans = peaks _
            .populates(Of PeakMs2)(env) _
            .GroupBy(Function(t) t.rt,
                     Function(t1, t2)
                         Return std.Abs(t1 - t2) <= timeWindow
                     End Function) _
            .ToArray
        Dim groupMs1 = (From list As NamedCollection(Of PeakMs2)
                        In groupScans
                        Select list.Scan1).ToArray

        Return New mzPack With {
            .Application = FileApplicationClass.LCMS,
            .MS = groupMs1,
            .source = "<assembly>"
        }
    End Function

    ''' <summary>
    ''' write mzPack in v2 format
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="file">A file path that reference to the local file stream for save the data</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("packStream")>
    Public Function writeStream(data As mzPack, file As Object, Optional env As Environment = Nothing) As Object
        Dim buffer = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        Else
            Return data.WriteStream(buffer)
        End If
    End Function

    Private Function denoiseMzpack(mzPack As mzPack, cut As Integer) As Object
        mzPack.MS = mzPack.MS _
            .AsParallel _
            .Select(Function(ms1)
                        Return ms1.handlingMs2products(cut)
                    End Function) _
            .ToArray

        Return mzPack
    End Function

    <Extension>
    Private Function handlingMs2products(ms1 As ScanMS1, cut As Integer) As ScanMS1
        ms1.products = ms1.products _
            .Select(Function(ms2)
                        Dim peaks = ms2.GetMs _
                            .AbSciexBaselineHandling(cut) _
                            .ToArray
                        Dim mz As Double() = peaks.Select(Function(a) a.mz).ToArray
                        Dim into As Double() = peaks.Select(Function(a) a.intensity).ToArray
                        ms2.mz = mz
                        ms2.into = into
                        Return ms2
                    End Function) _
            .ToArray

        Return ms1
    End Function

    Private Function denoiseMsMs(raw As Object, cut As Integer, env As Environment) As Object
        Dim msms As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(raw, env)

        If msms.isError Then
            Return msms.getError
        End If

        Return msms.populates(Of PeakMs2)(env) _
            .AsParallel _
            .Select(Function(ms2)
                        ms2.mzInto = ms2.mzInto _
                            .AbSciexBaselineHandling _
                            .ToArray
                        Return ms2
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' Removes the sciex AB5600 noise data from the MS2 raw data
    ''' </summary>
    ''' <param name="raw">should be a mzpack object or a collection of the ms2 data for handling the noise spectra peak removes</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("removeSciexNoise")>
    Public Function removeSciexNoise(<RRawVectorArgument>
                                     raw As Object,
                                     Optional cut As Integer = 2,
                                     Optional env As Environment = Nothing) As Object
        If TypeOf raw Is mzPack Then
            Return denoiseMzpack(DirectCast(raw, mzPack), cut)
        Else
            Return denoiseMsMs(raw, cut, env)
        End If
    End Function

    ''' <summary>
    ''' pack a given ms2 spectrum collection as a single ms1 scan product
    ''' </summary>
    ''' <param name="ms2">a collection of the ms2 spectrum data</param>
    ''' <param name="scan_id1">the scan id which will be specificed to the result scan ms1 object</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("pack_ms1")>
    <RApiReturn(GetType(ScanMS1))>
    Public Function packMs1(<RRawVectorArgument> ms2 As Object,
                            Optional scan_id1 As String = Nothing,
                            Optional env As Environment = Nothing) As Object

        Dim scan2 As New List(Of ScanMS2)
        Dim m2 As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(ms2, env)

        If m2.isError Then
            Return m2.getError
        End If
        If scan_id1.StringEmpty Then
            scan_id1 = Guid.NewGuid.ToString
        End If

        For Each ms As PeakMs2 In m2.populates(Of PeakMs2)(env)
            Call scan2.Add(New ScanMS2 With {
                .scan_id = ms.lib_guid,
                .intensity = ms.intensity,
                .parentMz = ms.mz,
                .rt = ms.rt,
                .mz = ms.mzInto.Select(Function(a) a.mz).ToArray,
                .into = ms.mzInto.Select(Function(a) a.intensity).ToArray
            })
        Next

        Return New ScanMS1 With {
            .scan_id = scan_id1,
            .BPC = scan2.Max(Function(a) a.intensity),
            .into = scan2.Select(Function(a) a.intensity).ToArray,
            .mz = scan2.Select(Function(a) a.parentMz).ToArray,
            .products = scan2.ToArray,
            .rt = scan2.Average(Function(a) a.rt),
            .TIC = scan2.Sum(Function(a) a.intensity),
            .meta = New Dictionary(Of String, String)
        }
    End Function

    ''' <summary>
    ''' union merge two rawdata.
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <returns></returns>
    <ROperator("+")>
    Public Function appendMzPack(a As mzPack, b As mzPack) As Object
        If a Is Nothing Then
            Return b
        ElseIf b Is Nothing Then
            Return a
        ElseIf a Is Nothing AndAlso b Is Nothing Then
            Return Nothing
        End If

        Return New mzPack With {
            .Annotations = a.Annotations _
                .JoinIterates(b.Annotations) _
                .GroupBy(Function(ai) ai.Key) _
                .ToDictionary(Function(ai) ai.Key,
                              Function(ai)
                                  Return ai.First.Value
                              End Function),
            .Application = a.Application,
            .MS = a.MS.JoinIterates(b.MS).ToArray,
            .source = $"{a.source}+{b.source}"
        }
    End Function
End Module
