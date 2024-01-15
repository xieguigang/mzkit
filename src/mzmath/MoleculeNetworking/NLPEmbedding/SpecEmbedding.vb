Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.NLP.Word2Vec
Imports Microsoft.VisualBasic.Linq

Public Class SpecEmbedding

    ReadOnly wv As Word2Vec

    Dim pool As NetworkingTree
    Dim index As TreeCluster

    Sub New(Optional method As TrainMethod = TrainMethod.Skip_Gram, Optional freq As Integer = 1)
        wv = New Word2VecFactory() _
            .setMethod(method) _
            .setNumOfThread(1) _
            .setFreqThresold(freq) _
            .build()
        pool = New NetworkingTree()
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sample">should be re-order by rt asc or something else?</param>
    Public Sub AddSample(sample As IEnumerable(Of PeakMs2))
        Dim pull As PeakMs2() = sample.SafeQuery.ToArray
        Dim clusters As String() = Nothing

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
