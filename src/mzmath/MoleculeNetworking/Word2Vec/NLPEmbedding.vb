Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
Imports Microsoft.VisualBasic.Data.NLP.Word2Vec

Public Class NLPEmbedding

    ReadOnly wv As Word2Vec

    Dim index As MzPool
    Dim pool As New List(Of MzIndex)
    Dim mzdiff As Double = 0.005

    Sub New(Optional method As TrainMethod = TrainMethod.Skip_Gram, Optional freq As Integer = 1)
        wv = New Word2VecFactory() _
            .setMethod(method) _
            .setNumOfThread(1) _
            .setFreqThresold(freq) _
            .build()
    End Sub

    Private Sub Indexing()
        index = New MzPool(pool.Select(Function(a) a.mz))
    End Sub

    Public Sub Add(spec As ISpectrum)
        Dim words As New List(Of String)
        Dim rebuild As Boolean

        For Each mz As ms2 In spec _
            .GetIons _
            .OrderByDescending(Function(i) i.intensity)

            Dim check As MzIndex = index.SearchBest(mz.mz, mzdiff)

            If check Is Nothing Then
                rebuild = True
                check = New MzIndex(mz.mz, index:=pool.Count)
                pool.Add(check)
            End If

            Call words.Add(check.index)
        Next

        If rebuild Then
            Call Indexing()
        End If

        Call wv.readTokens(words)
    End Sub

    Public Function CreateEmbedding() As VectorModel
        Call wv.training()
        Return wv.outputVector
    End Function

End Class
