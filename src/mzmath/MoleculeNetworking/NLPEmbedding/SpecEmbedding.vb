#Region "Microsoft.VisualBasic::8155b9294c0a7efb71df82a6362bfb0f, G:/mzkit/src/mzmath/MoleculeNetworking//NLPEmbedding/SpecEmbedding.vb"

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

    '   Total Lines: 79
    '    Code Lines: 54
    ' Comment Lines: 12
    '   Blank Lines: 13
    '     File Size: 2.49 KB


    ' Class SpecEmbedding
    ' 
    '     Properties: GetClusters
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: CreateEmbedding
    ' 
    '     Sub: (+2 Overloads) AddSample
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.NLP.Word2Vec
Imports Microsoft.VisualBasic.Linq

Public Class SpecEmbedding

    ReadOnly wv As Word2Vec

    Dim pool As NetworkingTree
    ''' <summary>
    ''' the spectrum vocabulary builder
    ''' </summary>
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

        ' create vocabulary
        If index Is Nothing Then
            index = pool.Tree(pull)
            clusters = index.clusters
        Else
            index = pool.Tree(index, pull, clusters)
        End If

        Call wv.readTokens(clusters)
    End Sub

    Public Sub AddSample(terms As IEnumerable(Of String))
        Call wv.readTokens(terms.ToArray)
    End Sub

    Public Function CreateEmbedding() As VectorModel
        Call wv.training()
        Return wv.outputVector
    End Function
End Class

