Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis

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
        Dim metadata As MetadataProxy
        ''' <summary>
        ''' the first element in the hash list
        ''' </summary>
        Dim rootId As String

        Dim disposedValue As Boolean

        ReadOnly fs As PoolFs
        ReadOnly handle As String

        Public Const tags As String = "123456789abcdef"

        ''' <summary>
        ''' Get child node by a given key name
        ''' </summary>
        ''' <param name="key">
        ''' + 0, z, zero means z tree
        ''' + other chars in hex <see cref="tags"/> means corresponding child
        ''' </param>
        ''' <returns>
        ''' returns null if not found
        ''' </returns>
        Default Public ReadOnly Property NextChild(key As String) As SpectrumPool
            Get
                If key = "0" OrElse key = "z" OrElse key = "zero" Then
                    Return zeroBlock
                Else
                    If classTree.ContainsKey(key) Then
                        Return classTree(key)
                    Else
                        Return Nothing
                    End If
                End If
            End Get
        End Property

        Public ReadOnly Property ClusterInfo As IEnumerable(Of Metadata)
            Get
                Return metadata.AllClusterMembers
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="fs">
        ''' the pool filesystem storage
        ''' </param>
        Private Sub New(fs As PoolFs, path As String)
            Me.fs = fs
            Me.handle = path.StringReplace("/{2,}", "/")
            Me.metadata = fs.LoadMetadata(path)
            Me.rootId = fs.FindRootId(path)

            For Each dir As String In fs.GetTreeChilds(path)
                If dir.BaseName = "z" Then
                    zeroBlock = New SpectrumPool(fs, dir)
                Else
                    classTree.Add(dir.BaseName, New SpectrumPool(fs, dir))
                End If
            Next

            If Not rootId.StringEmpty Then
                ' first element always the root element
                ' read as representive spectrum data
                representative = fs.ReadSpectrum(metadata(rootId))
            End If
        End Sub

        ''' <summary>
        ''' add spectrum into current molecular networking cluster tree data
        ''' </summary>
        ''' <param name="spectrum"></param>
        Public Sub Add(spectrum As PeakMs2)
            Dim score As AlignmentOutput
            Dim PIScore As Double
            Dim pval As Double

            If representative Is Nothing Then
                representative = spectrum
                rootId = spectrum.lib_guid
                metadata.SetRootId(rootId)
                score = Nothing
                PIScore = 1
                pval = 0
                VBDebugger.EchoLine($"create_root@{ToString()}: {spectrum.lib_guid}")
            Else
                Static zero As Double() = New Double() {.0, .0, .0, .0}

                score = fs.GetScore(spectrum, representative)
                PIScore = score.forward *
                    score.reverse *
                    score.jaccard *
                    score.entropy
                pval = t.Test({
                    score.forward, score.reverse, score.jaccard, score.entropy
                }, zero, Hypothesis.TwoSided).Pvalue
            End If

            If score Is Nothing OrElse PIScore > fs.level Then
                ' in current class node
                metadata.Add(spectrum.lib_guid, fs.WriteSpectrum(spectrum))
                metadata.Add(spectrum.lib_guid, PIScore, score, pval)

                VBDebugger.EchoLine($"join_pool@{ToString()}: {spectrum.lib_guid}")
            ElseIf PIScore <= 0 Then
                If zeroBlock Is Nothing Then
                    zeroBlock = New SpectrumPool(fs, handle & $"/z/")
                End If

                Call zeroBlock.Add(spectrum)
            Else
                Dim t As Double = fs.splitDelta
                Dim i As Integer = 0

                Do While t <= fs.level
                    If PIScore <= t Then
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

        ''' <summary>
        ''' Find the spectra object from this function if the cache is not hit
        ''' </summary>
        ''' <param name="guid"></param>
        ''' <returns></returns>
        Public Function GetSpectral(guid As String) As PeakMs2
            If representative IsNot Nothing AndAlso representative.lib_guid = guid Then
                Return representative
            End If

            If metadata.HasGuid(guid) Then
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
        ''' <param name="link">
        ''' ### for local filesystem
        ''' 
        ''' contains multiple files:
        ''' 
        ''' 1. cluster.pack  contains the metadata and structure information of the cluster tree
        ''' 2. spectrum.dat  contains the spectrum data
        ''' 
        ''' ### for web filesystem
        ''' 
        ''' </param>
        ''' <param name="level"></param>
        ''' <param name="split"></param>
        ''' <returns></returns>
        Public Shared Function Open(link As String, Optional level As Double = 0.85, Optional split As Integer = 3) As SpectrumPool
            Dim fs As PoolFs = PoolFs.CreateAuto(link)
            Dim pool As New SpectrumPool(fs, "/")

            Call fs.SetLevel(level, split)
            Call fs.SetScore(0.3, 0.05)

            Return pool
        End Function

        Public Overrides Function ToString() As String
            Return $"{handle}..."
        End Function

        Public Sub Commit()
            Call fs.SetRootId(handle, rootId)
            Call fs.CommitMetadata(handle, metadata)

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