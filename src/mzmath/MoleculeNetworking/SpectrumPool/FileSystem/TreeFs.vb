Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
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
        ''' due to the reason of contains in-memory cache system
        ''' inside this module, so we should share this object between
        ''' multiple pool object
        ''' </summary>
        Dim score As MSScoreGenerator

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

        Public Overrides Function LoadMetadata(path As String) As MetadataProxy
            Dim inMemory = fs.ReadText($"{path}/node_data/metadata.json").LoadJSON(Of Dictionary(Of String, Metadata))
            inMemory = If(inMemory, New Dictionary(Of String, Metadata))
            Return New InMemoryKeyValueMetadataPool(inMemory)
        End Function

        Public Overrides Function FindRootId(path As String) As String
            Return Strings.Trim(fs.ReadText($"{path}/node_data/root.txt")).Trim(ASCII.CR, ASCII.LF, ASCII.TAB, " "c)
        End Function

        Friend Sub SetScore(da As Double, intocutoff As Double, getSpectral As Func(Of String, PeakMs2))
            score = New MSScoreGenerator(
                align:=AlignmentProvider.Cosine(Tolerance.DeltaMass(da), New RelativeIntensityCutoff(intocutoff)),
                getSpectrum:=getSpectral,
                equals:=0,
                gt:=0
            )
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Sub Add(data As PeakMs2)
            Call score.Add(data)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetScore(x As String, y As String) As Double
            Return score.GetSimilarity(x, y)
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
            Dim meta As New Metadata With {
                .block = p,
                .guid = spectral.lib_guid,
                .intensity = spectral.intensity,
                .mz = spectral.mz,
                .organism = spectral.meta("organism"),
                .rt = spectral.rt,
                .sample_source = spectral.meta("biosample"),
                .source_file = spectral.file,
                .biodeep_id = spectral.meta.TryGetValue("biodeep_id", [default]:="unknown conserved"),
                .formula = spectral.meta.TryGetValue("formula", [default]:="NA"),
                .name = spectral.meta.TryGetValue("name", [default]:="unknown conserved"),
                .adducts = If(spectral.precursor_type.StringEmpty, "NA", spectral.precursor_type)
            }

            Call writer.Flush()

            Return meta
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