Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class AnnotationWorkspace : Implements IDisposable, IWorkspaceReader

    ReadOnly pack As StreamPack
    ReadOnly libraries As New Dictionary(Of String, Integer)
    ReadOnly samplefiles As New List(Of String)
    ReadOnly source As String

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

    Sub New(file As Stream, Optional source_file As String = Nothing)
        source = source_file
        pack = New StreamPack(file)

        If pack.FileExists("/libraries.json", ZERO_Nonexists:=True) Then
            libraries = pack.ReadText("/libraries.json").LoadJSON(Of Dictionary(Of String, Integer))
        End If
        If pack.FileExists("/samplefiles.json", ZERO_Nonexists:=True) Then
            samplefiles.AddRange(pack.ReadText("/samplefiles.json").LoadJSON(Of String()))
        End If
    End Sub

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
            .peaks = LoadPeakTable.ToArray
        }
    End Function

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

        If (Not libraries.ContainsKey(library)) OrElse libraries(library) <= 0 Then
            Return
        End If

        For Each file As StreamBlock In dir.ListFiles(recursive:=True).OfType(Of StreamBlock)
            Dim buf As Stream = pack.OpenBlock(file)
            Dim result As AlignmentHit = ReadPack.ReadMs2Annotation(buf)

            Yield result
        Next
    End Function

    Const peaktablefile As String = "/peaktable.dat"

    ''' <summary>
    ''' save xcms peaktable to pack file
    ''' </summary>
    ''' <param name="peaks"></param>
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

        For Each peak_result As AlignmentHit In result.SafeQuery
            i += 1

            Using file As Stream = pack.OpenFile($"/result/{library}/{peak_result.xcms_id}/{peak_result.libname}.dat",, FileAccess.Write)
                Dim bin As New BinaryDataWriter(file)
                Dim buf As MemoryStream = peak_result.PackAlignment

                Call bin.Write(buf.ToArray)
                Call bin.Flush()
            End Using
        Next

        Call libraries.Add(library, i)
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                Call pack.WriteText(libraries.GetJson, "/libraries.json")
                Call pack.WriteText(samplefiles.ToArray.GetJson, "/samplefiles.json")

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

    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
