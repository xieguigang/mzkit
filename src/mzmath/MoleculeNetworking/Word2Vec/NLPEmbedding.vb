Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
Imports Microsoft.VisualBasic.Data.NLP.Word2Vec

Public Class NLPEmbedding

    ReadOnly wv As Word2Vec

    Dim index As MzPool
    Dim pool As New List(Of MzIndex)
    Dim mzdiff As Double = 0.005

    Sub New(Optional method As TrainMethod = TrainMethod.Skip_Gram, Optional freq As Integer = 1)
        wv = (New Word2VecFactory()) _
            .setMethod(method) _
            .setNumOfThread(1) _
            .setFreqThresold(freq) _
            .build()
    End Sub

    Public Sub Add(spec As ISpectrum)
        Dim words As New List(Of String)

        For Each mz As ms2 In spec.GetIons.OrderByDescending(Function(i) i.intensity)
            Dim check = index.Search(mz.mz, mzdiff)
        Next

        Call wv.readTokens(words)
    End Sub

End Class
