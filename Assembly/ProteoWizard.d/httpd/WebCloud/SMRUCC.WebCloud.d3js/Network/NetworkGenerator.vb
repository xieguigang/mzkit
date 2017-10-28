#Region "Microsoft.VisualBasic::5131f0c9b1d473b98bbdc350e0b7bb6d, ..\workbench\d3js\Force-Directed Graph\Force-Collapsible\NetworkGenerator.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.WebCloud.d3js.Network.JSON
Imports NetGraph = Microsoft.VisualBasic.Data.visualize.Network.FileStream.NetworkTables
Imports NetworkEdge = Microsoft.VisualBasic.Data.visualize.Network.FileStream.NetworkEdge

Namespace Network

    ''' <summary>
    ''' Network visualization model json data generator.
    ''' </summary>
    Public Module NetworkGenerator

        ''' <summary>
        ''' Creates network data from network model
        ''' </summary>
        ''' <param name="net"></param>
        ''' <param name="indent">默认值False是为了网络传输所优化的无换行的格式</param>
        ''' <returns></returns>
        <Extension> Public Function FromNetwork(net As NetGraph, Optional indent As Boolean = False) As String
            Dim types$() = net.Nodes _
                .Select(Function(x) x.NodeType) _
                .Distinct _
                .ToArray
            Dim nodes As node() = LinqAPI.Exec(Of node) <=
 _
                From x As FileStream.Node
                In net.Nodes
                Let color As String = x("color")
                Select New node With {
                    .name = x.ID,
                    .group = Array.IndexOf(types, x.NodeType),
                    .type = x.NodeType,
                    .size = net.Links(x.ID),
                    .color = color
                }

            Dim nodeTable As Dictionary(Of node) = nodes _
                .WriteAddress _
                .ToDictionary
            Dim links As link() = LinqAPI.Exec(Of link) <=
 _
                From edge As NetworkEdge
                In net.Edges
                Select New link With {
                    .source = nodeTable(edge.FromNode).Address,
                    .target = nodeTable(edge.ToNode).Address,
                    .value = edge.value
                }

            Dim JSON$ = New out With {
                .nodes = nodes,
                .links = links
            }.GetJson(indent:=indent)
            Return JSON
        End Function

        ''' <summary>
        ''' Build network json data from the bacterial transcription regulation network
        ''' </summary>
        ''' <param name="regs"></param>
        ''' <returns></returns>
        <Extension>
        Public Function FromRegulations(regs As IEnumerable(Of Regulation)) As String
            Dim nodes As String() =
                LinqAPI.Exec(Of String) <= From x As Regulation
                                           In regs
                                           Select {x.ORF_ID, x.Regulator}
            Dim net As New NetGraph
            Dim nodesTable = (From x As Regulation
                              In regs
                              Select x
                              Group x By x.ORF_ID Into Group) _
                                  .ToDictionary(Function(x) x.ORF_ID,
                                                Function(x) (From g As Regulation
                                                             In x.Group
                                                             Select g
                                                             Group g By g.MotifFamily Into Count
                                                             Order By Count Descending).First.MotifFamily)

            For Each tf As String In regs.Select(Function(x) x.Regulator).Distinct
                If nodesTable.ContainsKey(tf) Then
                    Call nodesTable.Remove(tf)
                End If
                Call nodesTable.Add(tf, NameOf(tf))
            Next

            net += nodes.Distinct.ToArray(Function(x) New FileStream.Node(x, nodesTable(x)))
            net += From o In (From x As Regulation
                              In regs
                              Select x
                              Group x By x.GetJson Into Group)
                   Let edge As Regulation = o.Group.First
                   Let n As Integer = o.Group.Count
                   Select New NetworkEdge(edge.Regulator, edge.ORF_ID, n)

            Return net.FromNetwork
        End Function

        ''' <summary>
        ''' Build network json data from the bacterial transcription regulation network
        ''' </summary>
        ''' <param name="regs">The raw csv document file path.</param>
        ''' <returns></returns>
        Public Function FromRegulations(regs As String) As String
            Dim json As String =
            regs.LoadCsv(Of Regulation).FromRegulations()
            Return json
        End Function

        Public Function LoadJson(netDIR As String) As String
            Dim net As NetGraph = NetGraph.Load(netDIR)
            Dim json As String = FromNetwork(net)
            Return json
        End Function
    End Module
End Namespace