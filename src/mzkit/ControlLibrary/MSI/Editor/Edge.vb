
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
