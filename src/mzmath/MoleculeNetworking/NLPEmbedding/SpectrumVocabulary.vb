#Region "Microsoft.VisualBasic::ae0d9a144d3956fbfeb18652c5a02d06, G:/mzkit/src/mzmath/MoleculeNetworking//NLPEmbedding/SpectrumVocabulary.vb"

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

    '   Total Lines: 37
    '    Code Lines: 26
    ' Comment Lines: 3
    '   Blank Lines: 8
    '     File Size: 1.14 KB


    ' Class SpectrumVocabulary
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetClusters, ToString, ToTerm
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.DataMining.BinaryTree

''' <summary>
''' a term vocabulary mapper based on the <see cref="ClusterTree"/> clustering result.
''' </summary>
Public Class SpectrumVocabulary

    ReadOnly mapping As New Dictionary(Of String, String)
    ReadOnly clusters As New Dictionary(Of String, String())

    Sub New(taxonomy As ClusterTree)
        TreeCluster.GetTree(taxonomy, pull:=clusters)

        For Each cluster_id As String In clusters.Keys
            For Each id As String In clusters(cluster_id)
                mapping(id) = cluster_id
            Next
        Next
    End Sub

    Public Function GetClusters() As Dictionary(Of String, String())
        Return New Dictionary(Of String, String())(clusters)
    End Function

    Public Function ToTerm(id As String) As String
        If mapping.ContainsKey(id) Then
            Return mapping(id)
        Else
            Return "__None__"
        End If
    End Function

    Public Overrides Function ToString() As String
        Return $"{mapping.Count} objects in {clusters.Count} terms"
    End Function

End Class
