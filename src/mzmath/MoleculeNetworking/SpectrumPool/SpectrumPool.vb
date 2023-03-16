Imports System.IO
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Tree
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace PoolData

    ''' <summary>
    ''' A pool storage object for the spectrum tree data
    ''' </summary>
    Public Class SpectrumPool : Implements IDisposable

        Dim classTree As New Dictionary(Of String, SpectrumPool)
        Dim zeroBlock As SpectrumPool

        ''' <summary>
        ''' the representative spectrum of current spectrum tree node
        ''' </summary>
        Dim representative As PeakMs2
        ''' <summary>
        ''' index of the spectrum data in current cluster node
        ''' </summary>
        Dim metadata As New Dictionary(Of String, Metadata)
        ''' <summary>
        ''' the first element in the hash list
        ''' </summary>
        Dim rootId As String

        Dim disposedValue As Boolean

        ReadOnly fs As TreeFs
        ReadOnly handle As String

        Const tags As String = "123456789abcdef"

        Private ReadOnly Property nextTag As String
            Get
                Return tags(classTree.Count)
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="fs">
        ''' the pool filesystem storage
        ''' </param>
        ''' <param name="tag"></param>
        ''' <param name="level"></param>
        ''' <param name="split">
        ''' split into n parts
        ''' </param>
        Private Sub New(fs As TreeFs, path As String)
            Static not_branch As Index(Of String) = {"z", "node_data"}

            Me.fs = fs
            Me.handle = path.StringReplace("/{2,}", "/")

            metadata = fs.ReadText($"{path}/node_data/metadata.json").LoadJSON(Of Dictionary(Of String, Metadata))
            metadata = If(metadata, New Dictionary(Of String, Metadata))
            rootId = fs.ReadText($"{path}/node_data/root.txt")

            For Each dir As StreamGroup In fs.OpenFolder(path).dirs
                If dir.fileName Like not_branch Then
                    Continue For
                End If

                classTree.Add(dir.fileName, New SpectrumPool(fs, dir.referencePath))
            Next

            If Not rootId.StringEmpty Then
                ' first element always the root element
                ' read as representive spectrum data
                representative = fs.ReadSpectrum(metadata(rootId))
            End If
        End Sub

        Public Sub Add(spectrum As PeakMs2)
            Dim score As Double

            Call fs.Add(spectrum)

            If representative Is Nothing Then
                representative = spectrum
                rootId = spectrum.lib_guid
                score = 1
                VBDebugger.EchoLine($"create_root@{ToString()}: {spectrum.lib_guid}")
            Else
                If Not representative Is Nothing Then
                    Call fs.Add(representative)
                End If

                score = fs.GetScore(spectrum.lib_guid, representative.lib_guid)
            End If

            If score > fs.level Then
                ' in current class node
                metadata(spectrum.lib_guid) = WriteSpectrum(spectrum)
                VBDebugger.EchoLine($"join_pool@{ToString()}: {spectrum.lib_guid}")
            ElseIf score <= 0 Then
                If zeroBlock Is Nothing Then
                    zeroBlock = New SpectrumPool(fs, handle & $"/z/")
                End If

                Call zeroBlock.Add(spectrum)
            Else
                Dim t As Double = fs.splitDelta
                Dim i As Integer = 0

                Do While t < fs.level
                    If score <= t Then
                        Dim key As String = tags(i)

                        If Not classTree.ContainsKey(key) Then
                            Call classTree.Add(key, New SpectrumPool(fs, handle & $"/{key}/"))
                        End If

                        Call classTree(key).Add(spectrum)

                        Return
                    Else
                        t += fs.splitDelta
                        i += 1
                    End If
                Loop
            End If
        End Sub

        Private Function WriteSpectrum(spectral As PeakMs2) As Metadata
            Dim writer As New BinaryDataWriter(fs.baseStream) With {
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

        ''' <summary>
        ''' Find the spectra object from this function if the cache is not hit
        ''' </summary>
        ''' <param name="guid"></param>
        ''' <returns></returns>
        Public Function GetSpectral(guid As String) As PeakMs2
            If representative IsNot Nothing AndAlso representative.lib_guid = guid Then
                Return representative
            End If

            If metadata.ContainsKey(guid) Then
                Dim p As Metadata = metadata(guid)
                Dim data As PeakMs2 = fs.ReadSpectrum(p)

                Return data
            Else
                For Each tag As String In classTree.Keys
                    Dim sp As PeakMs2 = classTree(tag).GetSpectral(guid)

                    If Not sp Is Nothing Then
                        Return sp
                    End If
                Next
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' open root folder
        ''' </summary>
        ''' <param name="dir">
        ''' contains multiple files:
        ''' 
        ''' 1. cluster.pack  contains the metadata and structure information of the cluster tree
        ''' 2. spectrum.dat  contains the spectrum data
        ''' </param>
        ''' <param name="level"></param>
        ''' <param name="split"></param>
        ''' <returns></returns>
        Public Shared Function OpenDirectory(dir As String, Optional level As Double = 0.85, Optional split As Integer = 3) As SpectrumPool
            Dim fs = New TreeFs(dir)
            Dim pool As New SpectrumPool(fs, "/")

            Call fs.SetLevel(level, split)
            Call fs.SetScore(0.3, 0.05, AddressOf pool.GetSpectral)

            Return pool
        End Function

        Public Overrides Function ToString() As String
            Return $"{handle}..."
        End Function

        Public Sub Commit()
            Call fs.WriteText(rootId, $"{handle}/node_data/root.txt")
            Call fs.WriteText(metadata.GetJson, $"{handle}/node_data/metadata.json")

            For Each label As String In classTree.Keys
                Call classTree(label).Commit()
            Next

            If Not zeroBlock Is Nothing Then
                Call zeroBlock.Commit()
            End If
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call Commit()
                    Call fs.Dispose()
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
End Namespace