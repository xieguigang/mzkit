Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.NLP.Word2Vec
Imports Microsoft.VisualBasic.Linq

Public Class SpecEmbedding

    ReadOnly wv As Word2Vec

    Dim pool As NetworkingTree
    Dim index As TreeCluster

    ''' <summary>
    ''' get the tree clustering result
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property GetClusters As Dictionary(Of String, String())
        Get
            Return index.GetTree
        End Get
    End Property

    Sub New(Optional ndims As Integer = 30,
            Optional method As TrainMethod = TrainMethod.Skip_Gram,
            Optional freq As Integer = 1,
            Optional diff As Double = 0.1)

        wv = New Word2VecFactory() _
            .setMethod(method) _
            .setNumOfThread(1) _
            .setFreqThresold(freq) _
            .setVectorSize(size:=ndims) _
            .build()
        pool = New NetworkingTree(interval:=diff)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sample">should be re-order by rt asc or something else?</param>
    Public Sub AddSample(sample As IEnumerable(Of PeakMs2), Optional centroid As Boolean = False)
        Dim pull As PeakMs2() = sample.SafeQuery.ToArray
        Dim clusters As String() = Nothing

        If centroid Then
            pull = pull.AsParallel _
                .Select(Function(s)
                            Dim ms As ms2() = s.mzInto _
                                .Centroid(Tolerance.DeltaMass(0.3), New RelativeIntensityCutoff(0.01)) _
                                .ToArray

                            Return New PeakMs2(s.lib_guid, ms)
                        End Function) _
                .ToArray
        End If

        If index Is Nothing Then
            index = pool.Tree(pull)
            clusters = index.clusters
        Else
            index = pool.Tree(index, pull, clusters)
        End If

        Call wv.readTokens(clusters)
    End Sub

    Public Function CreateEmbedding() As VectorModel
        Call wv.training()
        Return wv.outputVector
    End Function
End Class
