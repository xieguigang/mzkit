#Region "Microsoft.VisualBasic::d72c94acf61ee05e4d3c26cc136dd02b, metadb\XrefEngine\AnnotationWorkspace.vb"

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

'   Total Lines: 306
'    Code Lines: 186 (60.78%)
' Comment Lines: 71 (23.20%)
'    - Xml Docs: 81.69%
' 
'   Blank Lines: 49 (16.01%)
'     File Size: 11.63 KB


' Class AnnotationWorkspace
' 
'     Properties: file
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: CheckXicCache, GetLibraryHits, LoadMemory, LoadPeakTable, LoadXicGroup
' 
'     Sub: CacheXicTable, CreateLibraryResult, (+2 Overloads) Dispose, Flush, SetPeakTable
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' The temp workspace file of the biodeep annotation pipeline
''' </summary>
Public Class AnnotationWorkspace : Implements IDisposable, IWorkspaceReader

    ReadOnly pack As StreamPack
    ''' <summary>
    ''' summary of the library result count
    ''' </summary>
    ReadOnly libraries As New Dictionary(Of String, Integer)
    ReadOnly samplefiles As New List(Of String)
    ReadOnly source As String
    ReadOnly wrap_tqdmConsole As Boolean = True

    Private disposedValue As Boolean

    ''' <summary>
    ''' try to get the file path of the pack file
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' file path value could be nothing when the data pack is in readonly mode.
    ''' </remarks>
    Public ReadOnly Property file As String
        Get
            Return If(source, pack.filepath)
        End Get
    End Property

    Public ReadOnly Property Chromatographic As String = "*"
    Public ReadOnly Property Polarity As String = "*"

    ''' <summary>
    ''' construct of the <see cref="AnnotationPack"/> file reader/writer
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="source_file"></param>
    Sub New(file As Stream,
            Optional source_file As String = Nothing,
            Optional meta_allocated As Long = 32 * ByteSize.MB,
            Optional wrap_tqdm As Boolean? = Nothing)

        source = source_file
        pack = New StreamPack(file, meta_size:=meta_allocated)
        wrap_tqdmConsole = If(wrap_tqdm Is Nothing, App.EnableTqdm, CBool(wrap_tqdm))

        If pack.FileExists("/chromatographic.txt", ZERO_Nonexists:=True) Then
            Chromatographic = Strings.Trim(pack.ReadText("/chromatographic.txt")).TrimNewLine.Trim
        End If
        If pack.FileExists("/polarity.txt", ZERO_Nonexists:=True) Then
            Polarity = Strings.Trim(pack.ReadText("/polarity.txt")).TrimNewLine.Trim
        End If
        If pack.FileExists("/libraries.json", ZERO_Nonexists:=True) Then
            libraries = pack.ReadText("/libraries.json").LoadJSON(Of Dictionary(Of String, Integer))
        End If
        If pack.FileExists("/samplefiles.json", ZERO_Nonexists:=True) Then
            samplefiles.AddRange(pack.ReadText("/samplefiles.json").LoadJSON(Of String()))
        End If

        If libraries.IsNullOrEmpty Then
            ' scan from dir names
            Dim result_dir As StreamGroup = pack.GetObject("/result/")

            If Not result_dir Is Nothing Then
                libraries = result_dir.dirs _
                    .ToDictionary(Function(a) a.fileName,
                                  Function(a)
                                      Return 0
                                  End Function)
            End If
        End If
    End Sub

    Public Sub SetExperimentLabel(chromatographic As String, polarity As String)
        _Chromatographic = Strings.Trim(chromatographic).TrimNewLine.Trim
        _Polarity = Strings.Trim(polarity).TrimNewLine.Trim

        Call pack.WriteText(chromatographic, "/chromatographic.txt", allocate:=False)
        Call pack.WriteText(polarity, "/polarity.txt", allocate:=False)
    End Sub

    ''' <summary>
    ''' Load in-memory data pack from the pack file stream
    ''' </summary>
    ''' <returns></returns>
    Public Function LoadMemory() As AnnotationPack Implements IWorkspaceReader.LoadMemory
        Dim libraries As New Dictionary(Of String, AlignmentHit())

        For Each name As String In Me.libraries.Keys
            Call VBDebugger.cat($"  -> load_memory: {name} ... ")
            Call libraries.Add(name, GetLibraryHits(name).ToArray)
            Call VBDebugger.EchoLine("ok!")
        Next

        Return New AnnotationPack With {
            .libraries = libraries,
            .file = file,
            .peaks = LoadPeakTable.ToArray,
            .RI = ReadRI()
        }
    End Function

    ''' <summary>
    ''' load ms1 peaktable data
    ''' </summary>
    ''' <returns></returns>
    Public Function LoadPeakTable() As IEnumerable(Of xcms2)
        If Not pack.FileExists(peaktablefile, ZERO_Nonexists:=True) Then
            Return New xcms2() {}
        End If

        Dim file As Stream = pack.OpenFile(peaktablefile, FileMode.Open, FileAccess.Read)
        Dim peaks As xcms2() = SaveXcms.ReadSamplePeaks(file)

        Return peaks
    End Function

    Public Iterator Function GetLibraryHits(library As String) As IEnumerable(Of AlignmentHit)
        Dim dir As StreamGroup = pack.GetObject($"/result/{library}/")

        For Each file As StreamBlock In dir.ListFiles(recursive:=True).OfType(Of StreamBlock)
            Dim buf As Stream = pack.OpenBlock(file)
            Dim result As AlignmentHit = ReadPack.ReadMs2Annotation(buf)

            Yield result
        Next
    End Function

    Const peaktablefile As String = "/peaktable.dat"

    Public Sub SaveRIReference(RI As Dictionary(Of String, RIRefer()))
        For Each name As String In RI.Keys
            Call pack.WriteText((From data As RIRefer In RI(name) Select data.GetJson), $"/RI/{name}.jsonl", allocate:=False)
        Next
    End Sub

    Public Function ReadRI() As Dictionary(Of String, RIRefer())
        Dim table As New Dictionary(Of String, RIRefer())

        For Each file As StreamBlock In pack.ListFiles("/RI/", recursive:=False)
            table(file.fileName.BaseName) = pack _
                .ReadText(file) _
                .LineTokens _
                .Where(Function(line) Not line.StringEmpty(, True)) _
                .Select(Function(line) line.LoadJSON(Of RIRefer)) _
                .ToArray
        Next

        Return table
    End Function

    ''' <summary>
    ''' Extract the XIC cache data from a given set of rawdata objects based on 
    ''' the peaktable information inside the workspace file
    ''' </summary>
    ''' <param name="files"></param>
    ''' <param name="mass_da">
    ''' the mass error window for extract the ms1 scatter data for the peak ion
    ''' </param>
    ''' <param name="rt_win">
    ''' the rt window size for extract the XIC peak data for the peak ion
    ''' </param>
    Public Sub CacheXicTable(Of mzPack As IMsAssemblyPack)(files As IEnumerable(Of mzPack), Optional mass_da As Double = 0.5, Optional rt_win As Double = 15)
        Dim pool As mzPack() = files.ToArray

        For i As Integer = 0 To pool.Length - 1
            Call SourceTagCheck(pool(i))
        Next

        ' commit current data pool
        Call Flush()
        ' and then load the peaktable back from the filesystem
        For Each peak As xcms2 In TqdmWrapper.Wrap(LoadPeakTable.ToArray, wrap_console:=wrap_tqdmConsole)
            Dim scatter = pool.AsParallel _
                .Select(Function(file)
                            Return New NamedCollection(Of ms1_scan)(file.source, file.PickIonScatter(peak.mz, peak.rt, mass_da, rt_win))
                        End Function) _
                .ToArray

            For Each file As NamedCollection(Of ms1_scan) In scatter
                Using s As Stream = pack.OpenFile($"/xic_table/{file.name}/{peak.ID}.xic",, FileAccess.Write)
                    Call file.SaveDataFrame(s)
                    Call s.Flush()
                End Using
            Next
        Next
    End Sub

    Private Sub SourceTagCheck(<Out> ByRef raw As IMsAssemblyPack)
        If raw.source.StringEmpty() Then
            Throw New InvalidDataException("Missing sample source file name for the mzpack rawdata object!")
        Else
            raw.source = raw.source _
               .Replace(".mzPack", "") _
               .Replace(".mzpack", "") _
               .Replace(".MZPACK", "")
        End If
    End Sub

    ''' <summary>
    ''' Make cache xic data from a given raw data file
    ''' </summary>
    ''' <typeparam name="mzPack"></typeparam>
    ''' <param name="file"></param>
    ''' <param name="mass_da"></param>
    ''' <param name="rt_win"></param>
    Public Sub CacheXicTable(Of mzPack As IMsAssemblyPack)(file As mzPack, Optional mass_da As Double = 0.5, Optional rt_win As Double = 15)
        Call SourceTagCheck(file)

        ' commit current data pool
        Call Flush()
        ' and then load the peaktable back from the filesystem
        For Each peak As xcms2 In TqdmWrapper.Wrap(LoadPeakTable.ToArray, wrap_console:=wrap_tqdmConsole)
            Dim xicScatter As ms1_scan() = file.PickIonScatter(peak.mz, peak.rt, mass_da, rt_win).ToArray

            Using s As Stream = pack.OpenFile($"/xic_table/{file.source}/{peak.ID}.xic",, FileAccess.Write)
                Call xicScatter.SaveDataFrame(s)
                Call s.Flush()
            End Using
        Next
    End Sub

    ''' <summary>
    ''' Check of the xic cache data is existed inside current workspace file
    ''' </summary>
    ''' <returns></returns>
    Public Function CheckXicCache() As Boolean
        Dim dir As StreamGroup = pack.GetObject("/xic_table/")

        If dir Is Nothing Then
            Return False
        End If

        Return dir.ListFiles(recursive:=True) _
            .OfType(Of StreamBlock) _
            .Where(Function(f) f.extensionSuffix = ("xic")) _
            .Any
    End Function

    ''' <summary>
    ''' load xic data from cache pool
    ''' </summary>
    ''' <param name="ion"></param>
    ''' <returns></returns>
    Public Iterator Function LoadXicGroup(ion As String) As IEnumerable(Of NamedCollection(Of ms1_scan))
        Dim dir As StreamGroup = pack.GetObject("/xic_table/")

        For Each file As StreamGroup In dir.ListFiles(recursive:=False).OfType(Of StreamGroup)
            Dim ionfile = file.ListFiles(recursive:=False) _
                .OfType(Of StreamBlock) _
                .Where(Function(f) f.fileName.BaseName = ion) _
                .FirstOrDefault

            If ionfile Is Nothing Then
                Continue For
            End If

            Dim cache As Stream = pack.OpenBlock(ionfile)
            Dim scatter As ms1_scan() = Ms1ScatterCache.LoadDataFrame(cache).ToArray

            Yield New NamedCollection(Of ms1_scan)(file.fileName, scatter)
        Next
    End Function

    ''' <summary>
    ''' save xcms peaktable to pack file
    ''' </summary>
    ''' <param name="peaks"></param>
    ''' <remarks>
    ''' the peaktable data has been commit to the filesystem automatically in this method.
    ''' </remarks>
    Public Sub SetPeakTable(peaks As IEnumerable(Of xcms2))
        Using file As Stream = pack.OpenFile(peaktablefile,, FileAccess.Write)
            Dim pool As xcms2() = peaks.ToArray
            Dim ROIs As Integer = pool.Length
            Dim sampleNames As String() = pool.PropertyNames

            Call samplefiles.Clear()
            Call samplefiles.AddRange(sampleNames)
            Call pool.DumpSample(ROIs, sampleNames, file)
        End Using
    End Sub

    Public Sub CreateLibraryResult(library$, result As IEnumerable(Of AlignmentHit))
        Dim i As Integer = 0
        Dim tags As New List(Of String)

        If library.StringEmpty Then
            library = "missing!"
        End If

        For Each peak_result As AlignmentHit In result.SafeQuery
            i += 1

            If peak_result Is Nothing Then
                Call $"found null alignment hit result value for '{library}' at index offset [{i}]!".warning
                Continue For
            End If

            Using file As Stream = pack.OpenFile($"/result/{library}/{peak_result.xcms_id}/{peak_result.libname}.dat",, FileAccess.Write)
                Dim bin As New BinaryDataWriter(file)
                Dim buf As MemoryStream = peak_result.PackAlignment

                Call tags.Add($"{peak_result.xcms_id}/{peak_result.libname}")
                Call bin.Write(buf.ToArray)
                Call bin.Flush()
            End Using
        Next

        If library = "missing!" Then
            Call $"missing library reference name for {tags.Take(10).JoinBy(", ")}...".warning
        End If

        If libraries.ContainsKey(library) Then
            Call libraries.Add(library, i + libraries(library))
        Else
            Call libraries.Add(library, i)
        End If
    End Sub

    ''' <summary>
    ''' Commit the memory cache data into filesystem
    ''' </summary>
    Public Sub Flush()
        Call pack.WriteText(libraries.GetJson, "/libraries.json", allocate:=False)
        Call pack.WriteText(samplefiles.ToArray.GetJson, "/samplefiles.json", allocate:=False)

        Call DirectCast(pack, IFileSystemEnvironment).Flush()
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                Call Flush()

                ' TODO: 释放托管状态(托管对象)
                Call pack.Dispose()
            End If

            ' TODO: 释放未托管的资源(未托管的对象)并重写终结器
            ' TODO: 将大型字段设置为 null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
    ' Protected Overrides Sub Finalize()
    '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    ''' <summary>
    ''' save the annotation result workspace
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
