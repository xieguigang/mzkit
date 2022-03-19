#Region "Microsoft.VisualBasic::73a65ce0f18af945728bda4097098c9e, mzkit\src\mzkit\ControlLibrary\MSI\Editor\Edge.vb"

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

    '   Total Lines: 27
    '    Code Lines: 24
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 822.00 B


    '     Enum EdgeType
    ' 
    '         [In], Out
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class Edge
    ' 
    '         Properties: [To], From, InRelation, Relation
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace PolygonEditor
    Public Enum EdgeType
        [In]
        Out
    End Enum

    Friend Class Edge
        Public Property From As Vertex
        Public Property [To] As Vertex
        Public Property InRelation As Edge
        Public Property Relation As Relation

        Public Sub New(ByVal from As Vertex, ByVal [to] As Vertex, ByVal Optional inRelation As Edge = Nothing)
            Relation = Relation.None
            Me.InRelation = inRelation
            from.Edges.Add(Me)
            [to].Edges.Add(Me)
            Me.From = from
            Me.To = [to]
        End Sub

        Public Sub New(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer)
            From = New Vertex(x1, y1)
            [To] = New Vertex(x2, y2)
        End Sub
    End Class
End Namespace
