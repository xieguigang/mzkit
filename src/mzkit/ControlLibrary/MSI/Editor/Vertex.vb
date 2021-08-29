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
