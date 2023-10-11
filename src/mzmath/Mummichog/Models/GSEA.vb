#Region "Microsoft.VisualBasic::c8c5f2b037cc8b904983687ff132f640, mzkit\src\mzmath\Mummichog\Models\GSEA.vb"

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

'   Total Lines: 23
'    Code Lines: 13
' Comment Lines: 6
'   Blank Lines: 4
'     File Size: 706 B


' Module GSEA
' 
'     Function: SingularGraph
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Analysis.HTS.GSEA

''' <summary>
''' Create background model from the GSEA background model
''' </summary>
Public Module GSEA

    ''' <summary>
    ''' Create a graph model for run Mummichog annotation without 
    ''' any network topology information
    ''' </summary>
    ''' <param name="cluster">
    ''' One of the cluster inside a gsea <see cref="Background"/> model
    ''' </param>
    ''' <returns>
    ''' just create a graph with node set, no edges
    ''' </returns>
    <Extension>
    Public Function SingularGraph(cluster As Cluster) As NetworkGraph
        Dim g As New NetworkGraph
        Dim uniqs As IEnumerable(Of BackgroundGene) = cluster.members _
            .GroupBy(Function(a) a.accessionID) _
            .Select(Function(d) d.First)
        Dim metadata As NodeData

        For Each member As BackgroundGene In uniqs
            metadata = New NodeData With {
                .label = member.name,
                .origID = member.accessionID
            }
            g.CreateNode(member.accessionID, metadata)
        Next

        ' just create a graph with node set, no edges
        Return g
    End Function
End Module
