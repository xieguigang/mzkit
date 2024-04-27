#Region "Microsoft.VisualBasic::09ba472b848b9a0060c03d6259429a73, G:/mzkit/src/mzmath/MoleculeNetworking//NLPEmbedding/IonEmbedding.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 62
    '    Code Lines: 42
    ' Comment Lines: 6
    '   Blank Lines: 14
    '     File Size: 1.79 KB


    ' Class IonEmbedding
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: CreateEmbedding
    ' 
    '     Sub: Add, Indexing
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
Imports Microsoft.VisualBasic.Data.NLP.Word2Vec

''' <summary>
''' Spec2Vec: Improved mass spectral similarity scoring through learning of structural relationships
''' </summary>
''' <remarks>
''' https://journals.plos.org/ploscompbiol/article?id=10.1371/journal.pcbi.1008724
''' </remarks>
Public Class IonEmbedding

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
