#Region "Microsoft.VisualBasic::cbfad983a0bcae168c1947597572f3ca, mzkit\src\mzkit\ControlLibrary\MSI\Editor\Vertex.vb"

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

    '   Total Lines: 59
    '    Code Lines: 49
    ' Comment Lines: 0
    '   Blank Lines: 10
    '     File Size: 1.53 KB


    '     Class Vertex
    ' 
    '         Properties: Coord, Edges, X, Y
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetInEdge, GetOutEdge, IsEmpty
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.Drawing

Namespace PolygonEditor
    Friend Class Vertex
        Public Property Edges As List(Of Edge) '0 - in, 1 - out

        Public Function GetInEdge() As Edge
            Return If(Edges(0).From Is Me, Edges(1), Edges(0))
        End Function

        Public Function GetOutEdge() As Edge
            Return If(Edges(0).To Is Me, Edges(1), Edges(0))
        End Function

        Public Property Coord As Point
            Get
                Return coordField
            End Get
            Set(ByVal value As Point)
                coordField = value
            End Set
        End Property

        Private coordField As Point

        Public Property X As Integer
            Get
                Return Coord.X
            End Get
            Set(ByVal value As Integer)
                coordField.X = value
            End Set
        End Property

        Public Property Y As Integer
            Get
                Return Coord.Y
            End Get
            Set(ByVal value As Integer)
                coordField.Y = value
            End Set
        End Property

        Public Sub New()
            Edges = New List(Of Edge)()
            Coord = New Point()
        End Sub

        Public Sub New(ByVal X As Integer, ByVal Y As Integer)
            Coord = New Point(X, Y)
            Edges = New List(Of Edge)()
        End Sub

        Public Function IsEmpty() As Boolean
            Return Coord.IsEmpty
        End Function
    End Class
End Namespace
