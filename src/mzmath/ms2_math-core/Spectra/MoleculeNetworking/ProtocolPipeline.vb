#Region "Microsoft.VisualBasic::0b1b492df813cafd8fda50e95611844e, E:/mzkit/src/mzmath/ms2_math-core//Spectra/MoleculeNetworking/ProtocolPipeline.vb"

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

    '   Total Lines: 92
    '    Code Lines: 65
    ' Comment Lines: 3
    '   Blank Lines: 24
    '     File Size: 3.12 KB


    '     Class ProtocolPipeline
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+3 Overloads) Networking, ProduceNodes
    ' 
    '     Structure LinkSet
    ' 
    '         Properties: links, reference
    ' 
    '     Class NetworkClusterLinkEndPoint
    ' 
    '         Properties: forward, id, reverse
    ' 
    '         Function: GetScore, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports stdNum = System.Math

Namespace Spectra.MoleculeNetworking

    Public Class ProtocolPipeline

        ReadOnly protocol As Protocols
        ReadOnly progress As Action(Of String)

        ReadOnly raw As PeakMs2()

        Dim nodes As IEnumerable(Of NetworkingNode)

        Sub New(protocol As Protocols, raw As PeakMs2(), progress As Action(Of String))
            Me.raw = raw
            Me.protocol = protocol
            Me.progress = progress
        End Sub

        Public Function ProduceNodes() As ProtocolPipeline
            nodes = protocol.ProduceNodes(raw)
            Return Me
        End Function

        Public Function Networking() As LinkSet()
            Return protocol.Networking(nodes, progress).ToArray
        End Function

        Public Function Networking(Of T As {New, INamedValue, DynamicPropertyBase(Of Double)})(aggrate As Func(Of Double, Double, Double)) As IEnumerable(Of T)
            Return Networking(Of T)(protocol.Networking(nodes, progress), aggrate)
        End Function

        Public Shared Iterator Function Networking(Of T As {New, INamedValue, DynamicPropertyBase(Of Double)})(
            linkSet As IEnumerable(Of LinkSet),
            Optional aggrate As Func(Of Double, Double, Double) = Nothing) As IEnumerable(Of T)

            If aggrate Is Nothing Then
                aggrate = AddressOf stdNum.Min
            End If

            For Each row As LinkSet In linkSet
                Dim obj As New T With {.Key = row.reference}

                For Each homologous In row.links
                    Call obj.Add(homologous.Key, aggrate(homologous.Value.forward, homologous.Value.reverse))
                Next

                Yield obj
            Next
        End Function
    End Class

    ''' <summary>
    ''' A collection of the cluster similarity score
    ''' </summary>
    Public Structure LinkSet

        Public Property reference As String
        Public Property links As Dictionary(Of String, NetworkClusterLinkEndPoint)

        Default Public ReadOnly Property GetScore(id As String) As (forward As Double, reverse As Double)
            Get
                Dim partner As NetworkClusterLinkEndPoint = links.TryGetValue(id)

                If Not partner Is Nothing Then
                    Return (partner.forward, partner.reverse)
                Else
                    Return Nothing
                End If
            End Get
        End Property

    End Structure

    Public Class NetworkClusterLinkEndPoint

        Public Property id As String
        Public Property forward As Double
        Public Property reverse As Double

        Public Function GetScore() As Double
            Return forward * reverse
        End Function

        Public Overrides Function ToString() As String
            Return $"[{id}] forward:{forward}, reverse:{reverse}"
        End Function

    End Class
End Namespace
