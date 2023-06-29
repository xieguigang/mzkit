Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Tree
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text

Namespace PoolData

    Public Class TreeFs : Inherits PoolFs
        Implements IDisposable

        ''' <summary>
        ''' A sequence spectrum data pool in current level 
        ''' </summary>
        ''' <remarks>
        ''' the stream data should be open in read/write mode
        ''' </remarks>
        Dim spectrumPool As BinaryDataReader
        Dim fs As StreamPack

        Public ReadOnly Property baseStream As Stream
            Get
                Return spectrumPool.BaseStream
            End Get
        End Property

        ''' <summary>
        ''' open a local fs directory for the data storage
        ''' </summary>
        ''' <param name="dir"></param>
        Sub New(dir As String)
            Me.fs = New StreamPack(dir & "/cluster.pack", meta_size:=1024 * 1024 * 256)

            spectrumPool = New BinaryDataReader(
                input:=$"{dir}/spectrum.dat".Open(
                    mode:=FileMode.OpenOrCreate,
                    doClear:=False,
                    [readOnly]:=False
                )
            )
            spectrumPool.ByteOrder = ByteOrder.LittleEndian
            spectrumPool.Encoding = Encoding.ASCII
        End Sub

        Shared ReadOnly not_branch As Index(Of String) = {"z", "node_data"}

        Public Overrides Iterator Function GetTreeChilds(path As String) As IEnumerable(Of String)
            For Each dir As StreamGroup In OpenFolder(path).dirs
                If dir.fileName Like not_branch Then
                    Continue For
                End If

                Yield dir.referencePath.ToString
            Next
        End Function

        Public Overrides Function CheckExists(spectral As PeakMs2) As Boolean
            Return False
        End Function

        Public Overrides Function LoadMetadata(path As String) As MetadataProxy
            Dim depth As Integer = path.Split("/"c).Length
            Dim inMemory = fs.ReadText($"{path}/node_data/metadata.json").LoadJSON(Of Dictionary(Of String, Metadata))
            inMemory = If(inMemory, New Dictionary(Of String, Metadata))
            Return New InMemoryKeyValueMetadataPool(inMemory, depth)
        End Function

        Public Overrides Function FindRootId(path As String) As String
            Return Strings.Trim(fs.ReadText($"{path}/node_data/root.txt")).Trim(ASCII.CR, ASCII.LF, ASCII.TAB, " "c)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ReadText(path As String) As String
            Return fs.ReadText(path)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub WriteText(text As String, path As String)
            Call fs.Delete(path)
            Call fs.WriteText(text, path)
        End Sub

        Public Function OpenFolder(path As String) As StreamGroup
            Dim obj As StreamObject = fs.GetObject(path & "/")

            If obj Is Nothing Then
                fs.OpenBlock(path & "/index.txt")
                obj = fs.GetObject(path & "/")
            End If

            Return obj
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ReadSpectrum(metadata As Metadata) As PeakMs2
            Return InternalFileSystem.ReadSpectrum(spectrumPool, block:=metadata.block)
        End Function

        Public Overrides Function WriteSpectrum(spectral As PeakMs2) As Metadata
            Dim writer As New BinaryDataWriter(baseStream) With {
                .ByteOrder = ByteOrder.LittleEndian,
                .Encoding = Encoding.ASCII
            }

            Call writer.Seek(writer.BaseStream.Length, SeekOrigin.Begin)
            ' Call writer.Align(8)

            Dim p As BufferRegion = InternalFileSystem.WriteSpectrum(spectral, writer)
            Dim meta As Metadata = GetMetadata(spectral)

            meta.block = p
            writer.Flush()

            Return meta
        End Function

        Public Shared Function GetMetadata(spectral As PeakMs2) As Metadata
            Dim name As String = spectral.meta.TryGetValue("name")

            If name.StringEmpty Then
                name = "unknown conserved[" & spectral.mzInto _
                    .OrderByDescending(Function(m) m.intensity) _
                    .Take(5) _
                    .Select(Function(mzi) mzi.mz.ToString("F2")) _
                    .JoinBy("/") & "]"
            End If

            Return New Metadata With {
                .guid = spectral.lib_guid,
                .intensity = spectral.intensity,
                .mz = spectral.mz,
                .organism = spectral.meta("organism"),
                .rt = spectral.rt,
                .sample_source = spectral.meta("biosample"),
                .source_file = spectral.file,
                .biodeep_id = spectral.meta.TryGetValue("biodeep_id", [default]:="unknown conserved"),
                .formula = spectral.meta.TryGetValue("formula", [default]:="NA"),
                .name = name,
                .adducts = If(spectral.precursor_type.StringEmpty, "NA", spectral.precursor_type),
                .instrument = spectral.meta.TryGetValue("instrument", [default]:="unknown instrument"),
                .project = spectral.meta.TryGetValue("project", [default]:="unknown project")
            }
        End Function

        Public Overrides Sub CommitMetadata(path As String, data As MetadataProxy)
            Call fs.WriteText(
                text:=data.AllClusterMembers.ToDictionary(Function(m) m.guid).GetJson,
                fileName:=$"{path}/node_data/metadata.json"
            )
        End Sub

        Public Overrides Sub SetRootId(path As String, id As String)
            Call fs.WriteText(id, $"{path}/node_data/root.txt")
        End Sub

        Protected Overrides Sub Close()
            Call spectrumPool.Dispose()
            Call fs.Dispose()
        End Sub
    End Class
End Namespace