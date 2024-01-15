Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.Data.NLP.Word2Vec

Public Class NLPEmbedding

    ReadOnly wv As Word2Vec

    Dim index As BlockSearchFunction(Of MzIndex)
    Dim pool As New List(Of MzIndex)

    Sub New(Optional method As TrainMethod = TrainMethod.Skip_Gram, Optional freq As Integer = 1)
        wv = (New Word2VecFactory()) _
            .setMethod(method) _
            .setNumOfThread(1) _
            .setFreqThresold(freq) _
            .build()
    End Sub

    Public Sub Add(spec As ISpectrum)
        For Each mz As ms2 In spec.GetIons
            Dim check = index.
        Next
    End Sub

End Class
