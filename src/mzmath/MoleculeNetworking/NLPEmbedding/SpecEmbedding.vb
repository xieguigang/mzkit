Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
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

        If index Is Nothing Then
            index = pool.Tree(pull)
        Else
            index = pool.Tree(index, pull)
        End If

        For Each spec As PeakMs2 In pull

        Next
    End Sub

End Class
