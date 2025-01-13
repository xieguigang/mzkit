Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.CrossReference
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Metadata = BioNovoGene.BioDeep.Chemistry.MetaLib.Models.MetaLib

''' <summary>
''' the data writer for <see cref="LocalRepository"/>.
''' </summary>
Public Class RepositoryWriter : Implements IDisposable

    ReadOnly s As StreamPack

    Dim block As New MemoryStream
    Dim blockIndex As New Dictionary(Of String, BufferRegion)
    Dim blockOffset As Integer = 1
    Dim disposedValue As Boolean

    Shared Sub New()
        Call MsgPackSerializer.DefaultContext.RegisterSerializer(Of BufferRegion)(NameOf(BufferRegion.position), NameOf(BufferRegion.size))
    End Sub

    Sub New(file As Stream)
        s = New StreamPack(file, meta_size:=128 * ByteSize.MB)
        blockOffset = 1 + DirectCast(s.GetObject("/block/"), StreamGroup) _
            .ListFiles() _
            .OfType(Of StreamBlock) _
            .Where(Function(obj)
                       Return obj.fileName.ExtensionSuffix("jsonl")
                   End Function) _
            .Count
    End Sub

    Public Sub Add(meta As Metadata)
        Dim json As String = meta.GetJson & vbLf
        Dim buf As Byte() = Encoding.UTF8.GetBytes(json)

        Const threshold As Long = 2 * ByteSize.GB - 4 * 1024

        If block.Length + buf.Length >= threshold Then
            ' create new block
            Call CommitBlock()
        End If

        Dim size As New BufferRegion(block.Length, buf.Length)

        Call block.Write(buf, Scan0, buf.Length)
        Call blockIndex.Add(meta.ID, size)
    End Sub

    Public Sub Add(meta As IMetabolite(Of xref))
        Call Add(New Metadata With {
            .xref = meta.CrossReference,
            .exact_mass = meta.ExactMass,
            .formula = meta.Formula,
            .ID = meta.Identity,
            .name = meta.CommonName,
            .synonym = {}
        })
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Friend Shared Function BlockFile(blockOffset As Integer) As String
        Return $"/block/{CInt(blockOffset).ToString.Last}/{blockOffset}.jsonl"
    End Function

    Friend Shared Iterator Function OffsetFiles(s As StreamPack) As IEnumerable(Of StreamBlock)
        For Each file As StreamBlock In DirectCast(s.GetObject("/offset/"), StreamGroup) _
            .ListFiles _
            .OfType(Of StreamBlock)()

            If file.fileName.ExtensionSuffix("msgpack") Then
                Yield file
            End If
        Next
    End Function

    Public Sub CommitBlock()
        Dim path As String = BlockFile(blockOffset)
        Dim offset As String = $"/offset/{CInt(blockOffset).ToString.Last}/{blockOffset}.msgpack"
        Dim s As Stream = Me.s.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Write)

        Call s.Write(block.ToArray, Scan0, block.Length)
        Call s.Flush()
        Call s.Dispose()
        Call block.Dispose()

        Dim offsetdata As Stream = Me.s.OpenFile(offset, FileMode.OpenOrCreate, FileAccess.Write)

        Call MsgPackSerializer.SerializeObject(blockIndex, offsetdata)
        Call offsetdata.Flush()
        Call offsetdata.Dispose()

        blockOffset += 1
        block = New MemoryStream
        blockIndex = New Dictionary(Of String, BufferRegion)
    End Sub

    Public Sub MakeIndex()
        If blockIndex.Any Then
            Call CommitBlock()
        End If

        Call VBDebugger.EchoLine("make index for the metabolite repository...")

    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call MakeIndex()
                Call s.Dispose()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
