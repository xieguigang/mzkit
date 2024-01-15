Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
Imports Microsoft.VisualBasic.Data.NLP.Word2Vec
Imports Microsoft.VisualBasic.Linq

Public Class SpecEmbedding

    ReadOnly wv As Word2Vec

    Dim index As TreeCluster

    Sub New(Optional method As TrainMethod = TrainMethod.Skip_Gram, Optional freq As Integer = 1)
        wv = New Word2VecFactory() _
            .setMethod(method) _
            .setNumOfThread(1) _
            .setFreqThresold(freq) _
            .build()
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sample">should be re-order by rt asc or something else?</param>
    Public Sub AddSample(sample As IEnumerable(Of PeakMs2))
        Dim docs As New List(Of String)

        For Each spec As PeakMs2 In sample.SafeQuery

        Next
    End Sub

End Class
