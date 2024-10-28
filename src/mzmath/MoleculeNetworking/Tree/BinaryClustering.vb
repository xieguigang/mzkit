#Region "Microsoft.VisualBasic::8f5e12fc5b122f6f9a6268a1ed5af8eb, mzmath\MoleculeNetworking\Tree\BinaryClustering.vb"

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

    '   Total Lines: 74
    '    Code Lines: 43 (58.11%)
    ' Comment Lines: 16 (21.62%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 15 (20.27%)
    '     File Size: 2.24 KB


    ' Class BinaryClustering
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: Clear, GetClusters, Tree
    ' 
    '     Sub: Add
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' Spectrum binary tree clustering helper
''' </summary>
Public Class BinaryClustering

    Friend ReadOnly align As MSScoreGenerator
    Friend ReadOnly equals_cutoff As Double = 0.85

    Dim bin As BTreeCluster

    Sub New(Optional mzdiff As Double = 0.3,
            Optional intocutoff As Double = 0.05,
            Optional equals As Double = 0.85,
            Optional interval As Double = 0.1)

        align = NetworkingTree.CreateAlignment(mzdiff, intocutoff, equals)
        equals_cutoff = equals
    End Sub

    ''' <summary>
    ''' clear the spectrum cache
    ''' </summary>
    ''' <returns></returns>
    Public Function Clear() As BinaryClustering
        Call align.Clear()
        Return Me
    End Function

    ''' <summary>
    ''' Just add the spectrum data to memory cache
    ''' </summary>
    ''' <param name="spec"></param>
    Public Sub Add(spec As PeakMs2)
        Call align.Add(spec)
    End Sub

    ''' <summary>
    ''' do spectrum tree alignment
    ''' </summary>
    ''' <param name="ions"></param>
    ''' <returns></returns>
    Public Function Tree(ions As IEnumerable(Of PeakMs2)) As BinaryClustering
        Dim uniqueIds As New List(Of String)

        For Each spectrum As PeakMs2 In ions.SafeQuery
            Call align.Add(spectrum)
            Call uniqueIds.Add(spectrum.lib_guid)
        Next

        bin = BuildTree.BTreeCluster(uniqueIds, align)

        Return Me
    End Function

    Public Iterator Function GetClusters() As IEnumerable(Of NamedCollection(Of PeakMs2))
        Dim pull As New List(Of BTreeCluster)
        Dim spectrum As PeakMs2()

        BTreeCluster.PullAllClusterNodes(bin, pull)

        For Each cluster As BTreeCluster In pull
            spectrum = cluster.data.Values _
                .OfType(Of PeakMs2) _
                .ToArray

            Yield New NamedCollection(Of PeakMs2)(cluster.uuid, spectrum)
        Next
    End Function
End Class

