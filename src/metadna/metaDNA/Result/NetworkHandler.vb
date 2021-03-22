#Region "Microsoft.VisualBasic::721886279fa9f56bd3f0f834c26c2095, src\metadna\metaDNA\Result\NetworkHandler.vb"

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

    ' Module NetworkHandler
    ' 
    '     Function: ExportNetwork
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Public Module NetworkHandler

    <Extension>
    Public Function ExportNetwork(table As IEnumerable(Of MetaDNAResult)) As NetworkGraph
        Dim g As New NetworkGraph
        Dim edgeData As EdgeData
        Dim names As New Dictionary(Of String, String)

        For Each inferLink As MetaDNAResult In table
            If g.GetElementByID(inferLink.KEGGId) Is Nothing Then
                g.CreateNode(inferLink.KEGGId)
            End If
            If g.GetElementByID(inferLink.partnerKEGGId) Is Nothing Then
                g.CreateNode(inferLink.partnerKEGGId)
            End If

            edgeData = New EdgeData With {
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, inferLink.inferLevel}
                }
            }
            g.CreateEdge(inferLink.partnerKEGGId, inferLink.KEGGId, inferLink.parentTrace, edgeData)
            names(inferLink.KEGGId) = inferLink.name
        Next

        For Each vertex In g.vertex
            vertex.data.label = names.TryGetValue(vertex.label, [default]:=vertex.label)
        Next

        Return g
    End Function
End Module

