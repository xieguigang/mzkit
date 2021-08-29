Imports System.Collections.Generic

Namespace PolygonEditor
    Friend Class Polygon
        Public Property Vertices As List(Of Vertex)
        Public Property Edges As List(Of Edge)

        Public Sub New()
            Vertices = New List(Of Vertex)()
            Edges = New List(Of Edge)()
        End Sub

        Public Function HasEdge(ByVal e As Edge) As Boolean
            For Each edge In Edges
                If edge Is e Then Return True
            Next

            Return False
        End Function

        Public Sub New(ByVal vertices As List(Of Vertex), ByVal edges As List(Of Edge))
            Me.Vertices = vertices
            Me.Edges = edges
        End Sub
    End Class
End Namespace
