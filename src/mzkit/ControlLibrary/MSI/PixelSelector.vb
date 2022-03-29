#Region "Microsoft.VisualBasic::0ddbca067bea5ea1a2e901d7a6d9ad1f, mzkit\src\mzkit\ControlLibrary\MSI\PixelSelector.vb"

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

'   Total Lines: 1307
'    Code Lines: 1024
' Comment Lines: 50
'   Blank Lines: 233
'     File Size: 48.90 KB


' Class PixelSelector
' 
'     Properties: HasRegionSelection, MSImage, Pixel, RegionSelectin, SelectPolygonMode
'                 ShowPointInform
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: AreEqual, ArePerpendicular, BelongsToCircle, BelongsToSegment, CalculateLength
'               CorrectClockwise, CorrectCounterclockwise, FindEdge, FindEdgeWithIndex, FindVertex
'               Frac, Ipart, IsLastPolygonCorrect, RFrac, Round
' 
'     Sub: AddVertex, AntialiasingWU, Bresenham, BresenhamSymmetric, canvasMouseDown
'          ClearSelection, clickGetPoint, doGauss, (+2 Overloads) DrawSelectionBox, EqualEdges
'          getPoint, HalveEdge, InvalidPolygonError, MoveEdge, MovePolygon
'          MoveVertex, OnAddVertexMenuItemClick, OnBoadMouseClick, OnBoadMouseDown, OnBoardMouseMove
'          OnBoardMouseUp, OnBoardPaint, OnEqualEdgesMenuItemClick, OnHalveEdgeMenuItemClick, OnMoveComponentMenuItemClick
'          OnMovePolygonMenuItemClick, OnPerpendiculateEdgesMenuItemClick, OnRemovePolygonMenuItemClick, OnRemoveRelationMenuItemClick, OnRemoveVertexMenuItemClick
'          PerpendiculateEdges, picCanvas_MouseMove, picCanvas_MouseUp, PixelSelector_Load, Plot
'          polygonDemo, RemovePolygon, RemoveRelation, RemoveVertex, renderWithLegend
'          RepaintPolygon, SetMsImagingOutput, ShowMessage, Timer1_Tick, toolStripMenuItem1_Click
' 
' /********************************************************************************/

#End Region

Imports ControlLibrary.PolygonEditor
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Filters
Imports stdNum = System.Math

Public Class PixelSelector

    Public Event SelectPixel(x As Integer, y As Integer)
    Public Event SelectPixelRegion(region As Rectangle)
    Public Event SelectPolygon(polygon As PointF())

    Private menuOption As MenuOption = MenuOption.MoveComponent
    Private relation As Relation = Relation.None
    Private polygons As List(Of Polygon) = New List(Of Polygon)()
    Private edgesInRelation As List(Of (Edge, Edge)) = New List(Of (Edge, Edge))()
    Private clickedEdges As List(Of Edge) = New List(Of Edge)()
    Private vertexCopy As List(Of Vertex) = New List(Of Vertex)()
    Private edgesRelationCopy As List(Of (Edge, Edge)) = New List(Of (Edge, Edge))()
    Private movingVertex As Vertex = Nothing
    Private movingEdge As Edge = Nothing
    Private movingPolygon As Polygon = Nothing
    Private mouse As Vertex = New Vertex()
    Private ismouseDown As Boolean = False
    Private algorithmIndex As Integer = 1

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        picCanvas.BackgroundImageLayout = ImageLayout.Stretch
    End Sub

    'Sub polygonDemo()
    '    Dim predefinedVertices As List(Of Vertex) = New List(Of Vertex)(New Vertex() {New Vertex(268, 223), New Vertex(313, 340), New Vertex(442, 340), New Vertex(489, 221), New Vertex(380, 112)})
    '    Dim predefinedEdges As List(Of Edge) = New List(Of Edge)()

    '    For i = 0 To 5 - 1
    '        predefinedEdges.Add(New Edge(predefinedVertices(i), predefinedVertices((i + 1) Mod 5)))
    '    Next

    '    Me.EqualEdges(predefinedEdges(0), predefinedEdges(2), predefinedEdges(CInt(0)).From)
    '    Me.PerpendiculateEdges(predefinedEdges(3), predefinedEdges(4), predefinedEdges(CInt(3)).From)
    '    polygons.Add(New Polygon(predefinedVertices, predefinedEdges))
    '    edgesInRelation.Add((predefinedEdges(0), predefinedEdges(2)))
    '    edgesInRelation.Add((predefinedEdges(3), predefinedEdges(4)))

    '    RepaintPolygon()
    'End Sub

#Region "Polygon Editor"
    ''' <summary>
    ''' MoveComponentMenuItem.Click
    ''' </summary>
    Public Sub OnMoveComponentMenuItemClick()
        If Not IsLastPolygonCorrect() Then Return
        menuOption = MenuOption.MoveComponent

        clickedEdges = New List(Of Edge)()
        RepaintPolygon()
    End Sub

    ''' <summary>
    ''' AddVertexMenuItem.Click
    ''' </summary>
    Public Sub OnAddVertexMenuItemClick()
        IsLastPolygonCorrect()
        Dim vertices As List(Of Vertex) = New List(Of Vertex)()
        Dim edges As List(Of Edge) = New List(Of Edge)()
        polygons.Add(New Polygon(vertices, edges))
        menuOption = MenuOption.AddVertex

        clickedEdges = New List(Of Edge)()
        RepaintPolygon()
    End Sub

    ''' <summary>
    ''' RemoveVertexMenuItem.Click
    ''' </summary>
    Public Sub OnRemoveVertexMenuItemClick()
        If Not IsLastPolygonCorrect() Then Return
        menuOption = MenuOption.DeleteVertex

        clickedEdges = New List(Of Edge)()
        RepaintPolygon()
    End Sub

    ''' <summary>
    ''' MovePolygonMenuItem.Click
    ''' </summary>
    Public Sub OnMovePolygonMenuItemClick()
        If Not IsLastPolygonCorrect() Then Return
        menuOption = MenuOption.MovePolygon

        clickedEdges = New List(Of Edge)()
    End Sub

    ''' <summary>
    ''' RemovePolygonMenuItem.Click
    ''' </summary>
    Public Sub OnRemovePolygonMenuItemClick()
        If Not IsLastPolygonCorrect() Then Return
        menuOption = MenuOption.RemovePolygon

        clickedEdges = New List(Of Edge)()
        RepaintPolygon()
    End Sub

    ''' <summary>
    ''' HalveEdgeMenuItem.Click
    ''' </summary>
    Public Sub OnHalveEdgeMenuItemClick()
        If Not IsLastPolygonCorrect() Then Return
        menuOption = MenuOption.HalveEdge

        clickedEdges = New List(Of Edge)()
        RepaintPolygon()
    End Sub

    ''' <summary>
    ''' EqualEdgesMenuItem.Click
    ''' </summary>
    Public Sub OnEqualEdgesMenuItemClick()
        If Not IsLastPolygonCorrect() Then Return
        clickedEdges = New List(Of Edge)()
        menuOption = MenuOption.AddRelation
        relation = Relation.Equality

        RepaintPolygon()
    End Sub

    ''' <summary>
    ''' PerpendiculateEdgesMenuItem.Click
    ''' </summary>
    Public Sub OnPerpendiculateEdgesMenuItemClick()
        If Not IsLastPolygonCorrect() Then Return
        clickedEdges = New List(Of Edge)()
        menuOption = MenuOption.AddRelation
        relation = Relation.Perpendicular
        RepaintPolygon()
    End Sub

    ''' <summary>
    ''' RemoveRelationMenuItem.Click
    ''' </summary>
    Public Sub OnRemoveRelationMenuItemClick()
        If Not IsLastPolygonCorrect() Then Return
        menuOption = MenuOption.RemoveRelation
        clickedEdges = New List(Of Edge)()
        RepaintPolygon()
    End Sub

    Private Sub AddVertex(ByVal e As Vertex)
        Dim vertices As List(Of Vertex) = polygons(polygons.Count - 1).Vertices
        Dim edges As List(Of Edge) = polygons(polygons.Count - 1).Edges

        If Me.FindVertex(CType(e, Vertex)).Vertex IsNot Nothing Then
            MessageBox.Show("Cannot place vertex on another vertex!")
            Return
        End If

        vertices.Add(New Vertex(e.Coord.X, e.Y))

        If vertices.Count > 1 Then
            Dim from As Vertex = vertices(vertices.Count - 2)
            Dim [to] As Vertex = vertices(vertices.Count - 1)
            edges.Add(New Edge(from, [to]))
        End If

        If vertices.Count > 2 Then
            Dim [to] As Vertex = vertices(0)
            Dim from As Vertex = vertices(vertices.Count - 1)

            If vertices.Count > 3 Then
                Dim edge As Edge = edges(edges.Count - 2)
                edge.From.Edges.Remove(edge)
                edge.To.Edges.Remove(edge)
                edges.Remove(edge)
            End If

            edges.Add(New Edge(from, [to]))
        End If

        RepaintPolygon()
    End Sub

    Private Sub RemoveVertex(ByVal soughtVertex As Vertex)
        Dim vertexToRemove As Vertex = Nothing, polygon As Polygon = Nothing

        With Me.FindVertex(soughtVertex)
            vertexToRemove = .Vertex
            polygon = .Polygon
        End With

        If vertexToRemove Is Nothing Then Return

        If polygon.Vertices.Count = 3 Then
            MessageBox.Show("A polygon has to have at least 3 vertices!")
            Return
        End If

        Dim [to] As Vertex = New Vertex(), from As Vertex = New Vertex()
        Dim list As List(Of (List(Of Edge), Edge)) = New List(Of (List(Of Edge), Edge))()

        For Each edge In vertexToRemove.Edges

            If edge.From Is vertexToRemove Then
                If edge.Relation <> Relation.None Then Me.RemoveRelation(edge)
                list.Add((edge.To.Edges, edge))
                [to] = edge.To
                polygon.Edges.Remove(edge)
            ElseIf edge.To Is vertexToRemove Then
                If edge.Relation <> Relation.None Then Me.RemoveRelation(edge)
                list.Add((edge.From.Edges, edge))
                polygon.Edges.Remove(edge)
                from = edge.From
            End If
        Next

        Dim removeFrom As List(Of Edge) = Nothing, toRemove As Edge = Nothing

        For Each t As (removeFrom As List(Of Edge), toRemove As Edge) In list
            removeFrom = t.removeFrom
            toRemove = t.toRemove
            removeFrom.Remove(toRemove)
        Next

        polygon.Edges.Add(New Edge(from, [to]))
        polygon.Vertices.Remove(vertexToRemove)
        RepaintPolygon()
    End Sub

    Private Sub RemovePolygon(ByVal polygon As Polygon)
        polygons.Remove(polygon)

        For Each edge In polygon.Edges
            If edge.Relation <> Relation.None Then Me.RemoveRelation(edge)
        Next

        RepaintPolygon()
    End Sub

    Private Sub RemoveRelation(ByVal edge As Edge)
        If Not edgesInRelation.Remove((edge, edge.InRelation)) Then edgesInRelation.Remove((edge.InRelation, edge))
        edge.InRelation.Relation = Relation.None
        edge.Relation = Relation.None
        edge.InRelation.InRelation = Nothing
        edge.InRelation = Nothing
    End Sub

    Private Sub EqualEdges(ByVal edge1 As Edge, ByVal edge2 As Edge, ByVal Optional staticPoint As Vertex = Nothing)
        edge1.Relation = Relation.Equality
        edge2.Relation = Relation.Equality
        edge1.InRelation = edge2
        edge2.InRelation = edge1

        If Me.AreEqual(edge1, edge2) Then
            RepaintPolygon()
            Return
        End If

        Dim e1Length = Me.CalculateLength(edge1)
        Dim e2Length = Me.CalculateLength(edge2)
        Dim diff = Math.Abs(e1Length - e2Length)

        If diff > 2 Then
            Dim r = e2Length
            Dim x0 As Integer = staticPoint.X
            Dim y0 As Integer = staticPoint.Y
            Dim movingPoint As Vertex = If(staticPoint.Coord.Equals(edge1.From.Coord), edge1.To, edge1.From)
            Dim a, b, delta As Double
            Dim a1, b1, c1, x1, y1, x2, y2 As Double

            If edge1.To.X <> edge1.From.X Then
                a = CDbl(staticPoint.Y - movingPoint.Y) / CDbl(staticPoint.X - movingPoint.X)
                b = movingPoint.Y - a * movingPoint.X
                a1 = a * a + 1
                b1 = 2 * (a * b - x0 - a * y0)
                c1 = x0 * x0 + b * b - 2 * b * y0 - r * r + y0 * y0
                delta = b1 * b1 - 4 * a1 * c1
                x1 = (-1 * b1 - Math.Sqrt(delta)) / (2 * a1)
                x2 = (-1 * b1 + Math.Sqrt(delta)) / (2 * a1)
                y1 = a * x1 + b
                y2 = a * x2 + b
            Else
                Dim x As Integer = edge1.From.X
                a1 = 1
                b1 = -2 * y0
                c1 = x * x - 2 * x * x0 + x0 * x0 + y0 * y0 - r * r
                delta = b1 * b1 - 4 * a1 * c1
                y1 = (-1 * b1 - Math.Sqrt(delta)) / (2 * a1)
                y2 = (-1 * b1 + Math.Sqrt(delta)) / (2 * a1)
                x2 = x
                x1 = x
            End If

            Dim length1 As Integer = Me.CalculateLength(New Edge(New Vertex(CInt(Math.Round(x1)), CInt(Math.Round(y1))), New Vertex(movingPoint.X, movingPoint.Y)))
            Dim length2 As Integer = Me.CalculateLength(New Edge(New Vertex(CInt(Math.Round(x2)), CInt(Math.Round(y2))), New Vertex(movingPoint.X, movingPoint.Y)))
            movingPoint.Coord = If(length1 < length2, New Point(Math.Round(x1), Math.Round(y1)), New Point(Math.Round(x2), Math.Round(y2)))
        End If

        RepaintPolygon()
    End Sub

    Private Sub PerpendiculateEdges(ByVal edge1 As Edge, ByVal edge2 As Edge, ByVal staticPoint As Vertex)
        edge1.Relation = Relation.Perpendicular
        edge2.Relation = Relation.Perpendicular
        edge1.InRelation = edge2
        edge2.InRelation = edge1

        If Me.ArePerpendicular(edge1, edge2) Then
            RepaintPolygon()
            Return
        End If

        Dim x1 As Double = edge2.From.X
        Dim y1 As Double = edge2.From.Y
        Dim x2 As Double = edge2.To.X
        Dim y2 As Double = edge2.To.Y
        Dim movingPoint As Vertex = If(edge1.From.Coord.Equals(staticPoint.Coord), edge1.To, edge1.From)
        Dim a = (y2 - y1) / (x2 - x1)
        Dim b = y1 - a * x1
        Dim e1Length = Me.CalculateLength(edge1)
        Dim e2Length = Me.CalculateLength(edge2)
        Dim r = e1Length
        Dim x0 As Integer = staticPoint.X
        Dim y0 As Integer = staticPoint.Y
        Dim delta As Double
        Dim a1, b1, c1 As Double

        If edge2.To.X = edge2.From.X Then
            a1 = 1
            b1 = -2 * x0
            c1 = x0 * x0 - r * r
            delta = b1 * b1 - 4 * a1 * c1
            x1 = (-1 * b1 - Math.Sqrt(delta)) / (2 * a1)
            x2 = (-1 * b1 + Math.Sqrt(delta)) / (2 * a1)
            y2 = y0
            y1 = y0
        ElseIf edge2.To.Y = edge2.From.Y Then
            a1 = 1
            b1 = -2 * y0
            c1 = y0 * y0 - r * r
            delta = b1 * b1 - 4 * a1 * c1
            y1 = (-1 * b1 - Math.Sqrt(delta)) / (2 * a1)
            y2 = (-1 * b1 + Math.Sqrt(delta)) / (2 * a1)
            x2 = x0
            x1 = x0
        Else
            a = -1 * (1 / a)
            b = staticPoint.Y - a * staticPoint.X
            a1 = a * a + 1
            b1 = 2 * (a * b - x0 - a * y0)
            c1 = x0 * x0 + b * b - 2 * b * y0 - r * r + y0 * y0
            delta = b1 * b1 - 4 * a1 * c1
            x1 = (-1 * b1 - Math.Sqrt(delta)) / (2 * a1)
            x2 = (-1 * b1 + Math.Sqrt(delta)) / (2 * a1)
            y1 = a * x1 + b
            y2 = a * x2 + b
        End If

        Dim length1 As Integer = Me.CalculateLength(New Edge(New Vertex(CInt(Math.Round(x1)), CInt(Math.Round(y1))), New Vertex(movingPoint.X, movingPoint.Y)))
        Dim length2 As Integer = Me.CalculateLength(New Edge(New Vertex(CInt(Math.Round(x2)), CInt(Math.Round(y2))), New Vertex(movingPoint.X, movingPoint.Y)))
        movingPoint.Coord = If(length1 < length2, New Point(Math.Round(x1), Math.Round(y1)), New Point(Math.Round(x2), Math.Round(y2)))
        RepaintPolygon()
    End Sub

    Private Sub HalveEdge(ByVal polygon As Polygon, ByVal edge As Edge, ByVal i As Integer)
        If Me.CalculateLength(edge) < 6 Then
            MessageBox.Show("Edge is too small to halve")
            Return
        End If

        Dim x As Integer = (Math.Max(edge.From.Coord.X, edge.To.Coord.X) + Math.Min(edge.From.Coord.X, edge.To.Coord.X)) / 2
        Dim y As Integer = (Math.Max(edge.From.Y, edge.To.Y) + Math.Min(edge.From.Y, edge.To.Y)) / 2
        Dim newVertex As Vertex = New Vertex(x, y)
        If edge.Relation <> Relation.None Then Me.RemoveRelation(edge)
        polygon.Edges.Remove(edge)
        edge.From.Edges.Remove(edge)
        edge.To.Edges.Remove(edge)
        polygon.Edges.Insert(i, New Edge(edge.From, newVertex))
        polygon.Edges.Insert(i + 1, New Edge(newVertex, edge.To))

        If edge.To Is polygon.Vertices(0) Then
            polygon.Vertices.Add(newVertex)
        Else
            Dim j1 As Integer = polygon.Vertices.FindIndex(Function(v) v Is edge.From)
            Dim j2 As Integer = polygon.Vertices.FindIndex(Function(v) v Is edge.To)
            polygon.Vertices.Insert(Math.Min(j1, j2) + 1, newVertex)
        End If

        RepaintPolygon()
    End Sub

    Private Function FindVertex(ByVal soughtVertex As Vertex) As (Vertex As Vertex, Polygon As Polygon)
        For Each polygon In polygons

            For Each vertex In polygon.Vertices
                If Me.BelongsToCircle(soughtVertex, vertex) Then Return (vertex, polygon)
            Next
        Next

        Return (Nothing, Nothing)
    End Function

    Private Function FindEdge(ByVal soughtEdge As Vertex) As (edge As Edge, Polygon As Polygon)
        For Each polygon In polygons

            For Each edge In polygon.Edges
                If Me.BelongsToSegment(soughtEdge, edge) Then Return (edge, polygon)
            Next
        Next

        Return (Nothing, Nothing)
    End Function

    Private Function FindEdgeWithIndex(ByVal soughtEdge As Vertex) As (Edge, Polygon, Integer)
        For Each polygon In polygons
            Dim i = 0

            For Each edge In polygon.Edges
                If Me.BelongsToSegment(soughtEdge, edge) Then Return (edge, polygon, i)
                i += 1
            Next
        Next

        Return (Nothing, Nothing, -1)
    End Function

    Private Function CorrectClockwise(ByVal e As Edge) As Boolean
        Dim edge = e
        Dim iter = 0

        While edge.Relation <> Relation.None AndAlso iter <= movingPolygon.Edges.Count

            If edge.Relation = Relation.Equality AndAlso Me.AreEqual(edge, edge.InRelation) Then
                Exit While
            ElseIf edge.Relation = Relation.Perpendicular AndAlso Me.ArePerpendicular(edge, edge.InRelation) Then
                Exit While
            End If

            Dim edgein = edge.From.GetInEdge().To.GetInEdge()

            If edge.Relation = Relation.Equality Then
                If edgein.Relation = Relation.Equality OrElse edgein.Relation = Relation.None Then
                    Me.EqualEdges(edge, edge.InRelation, edge.To)
                ElseIf edgein.Relation = Relation.Perpendicular Then
                    Me.EqualEdges(edge, edge.InRelation, edge.To)
                    Me.PerpendiculateEdges(edgein, edgein.InRelation, edgein.To)
                End If
            ElseIf edge.Relation = Relation.Perpendicular Then

                If edgein.Relation = Relation.None OrElse edgein.Relation = Relation.Perpendicular Then
                    Me.PerpendiculateEdges(edge, edge.InRelation, edge.To)
                Else
                    Me.PerpendiculateEdges(edge, edge.InRelation, edge.To)
                    Me.EqualEdges(edgein, edgein.InRelation, edgein.To)
                End If
            End If

            edge = edgein
            iter += 1
        End While

        Return If(iter = movingPolygon.Edges.Count + 1, False, True)
    End Function

    Private Function CorrectCounterclockwise(ByVal e As Edge) As Boolean
        Dim edge = e
        Dim iter = 0

        While edge.Relation <> Relation.None AndAlso iter <= movingPolygon.Edges.Count

            If edge.Relation = Relation.Equality AndAlso Me.AreEqual(edge, edge.InRelation) Then
                Exit While
            ElseIf edge.Relation = Relation.Perpendicular AndAlso Me.ArePerpendicular(edge, edge.InRelation) Then
                Exit While
            End If

            Dim edgeout = edge.From.GetOutEdge().To.GetOutEdge()

            If edge.Relation = Relation.Equality Then
                If edgeout.Relation = Relation.None OrElse edgeout.Relation = Relation.Equality Then
                    Me.EqualEdges(edge, edge.InRelation, edge.From)
                ElseIf edgeout.Relation = Relation.Perpendicular Then
                    Me.EqualEdges(edge, edge.InRelation, edge.From)
                    Me.PerpendiculateEdges(edgeout, edgeout.InRelation, edgeout.From)
                End If
            ElseIf edge.Relation = Relation.Perpendicular Then

                If edgeout.Relation = Relation.None OrElse edgeout.Relation = Relation.Perpendicular Then
                    Me.PerpendiculateEdges(edge, edge.InRelation, edge.From)
                Else
                    Me.PerpendiculateEdges(edge, edge.InRelation, edge.From)
                    Me.EqualEdges(edgeout, edgeout.InRelation, edgeout.From)
                End If
            End If

            edge = edgeout
            iter += 1
        End While

        Return If(iter = movingPolygon.Edges.Count + 1, False, True)
    End Function

    Private Sub MoveVertex(ByVal vertex As Vertex, ByVal x As Integer, ByVal y As Integer)
        Dim edge1 = vertex.GetInEdge()
        Dim edge2 = vertex.GetOutEdge()
        Dim oldLength1 = Me.CalculateLength(edge1)
        Dim oldLength1in As Integer = Me.CalculateLength(edge1.From.GetInEdge())
        Dim oldLength2 = Me.CalculateLength(edge2)
        Dim oldLength2Out As Integer = Me.CalculateLength(edge2.To.GetOutEdge())
        vertex.Coord = New Point(x, y)
        Dim correctedClockwise = True, correctedCounterclockwise = True
        If edge1.Relation <> Relation.None Then correctedClockwise = Me.CorrectClockwise(edge1)
        If edge2.Relation <> Relation.None Then correctedCounterclockwise = Me.CorrectCounterclockwise(edge2)
        If Not correctedClockwise AndAlso Not correctedCounterclockwise Then InvalidPolygonError()
        RepaintPolygon()
    End Sub

    Private Sub MoveEdge(ByVal edge As Edge, ByVal x As Integer, ByVal y As Integer)
        edge.From.Coord = New Point(edge.From.X + x, edge.From.Y + y)
        edge.To.Coord = New Point(edge.To.X + x, edge.To.Y + y)
        RepaintPolygon()
    End Sub

    Private Sub MovePolygon(ByVal vertices As List(Of Vertex), ByVal x As Integer, ByVal y As Integer)
        For Each vertex In vertices
            vertex.Coord = New Point(vertex.X + x, vertex.Y + y)
        Next

        RepaintPolygon()
    End Sub

    Private Sub OnBoadMouseClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles picCanvas.MouseClick
        If Not SelectPolygonMode Then
            Call clickGetPoint(e)
        Else
            Dim mouse As New Vertex(e.X, e.Y)

            If menuOption = MenuOption.AddVertex Then
                Me.AddVertex(mouse)
            ElseIf menuOption = MenuOption.DeleteVertex Then
                Me.RemoveVertex(mouse)
            ElseIf menuOption = MenuOption.AddRelation Then
                Dim edge As Edge

                With Me.FindEdge(mouse)
                    edge = .edge
                    movingPolygon = .Polygon
                End With

                If movingPolygon IsNot Nothing Then vertexCopy = movingPolygon.Vertices.[Select](Function(v) New Vertex(v.X, v.Y)).ToList()

                If edge IsNot Nothing Then
                    If clickedEdges.IndexOf(edge) = -1 AndAlso edge.Relation = Relation.None Then
                        If clickedEdges.Count = 0 Then
                            clickedEdges.Add(edge)
                        Else

                            If movingPolygon.HasEdge(clickedEdges(0)) Then
                                clickedEdges.Add(edge)
                            Else
                                MessageBox.Show("Cannot add relation between edges from" & " different polygons!")
                            End If
                        End If

                        RepaintPolygon()
                    End If

                    If clickedEdges.Count = 2 Then
                        Dim e1 As Edge = clickedEdges(0), e2 As Edge = clickedEdges(1)
                        Dim old As Point = New Point(e1.To.X, e1.To.Y)
                        Dim corrected = New Boolean(3) {}
                        edgesInRelation.Add((e1, e2))

                        If relation = Relation.Equality Then
                            Me.EqualEdges(e1, e2, e1.From)
                        ElseIf relation = Relation.Perpendicular Then
                            Me.PerpendiculateEdges(e1, e2, e1.From)
                        End If

                        corrected(0) = Me.CorrectClockwise(e1)
                        corrected(1) = Me.CorrectClockwise(e2)
                        corrected(2) = Me.CorrectCounterclockwise(e1.To.GetOutEdge())
                        corrected(3) = Me.CorrectCounterclockwise(e2.To.GetOutEdge())

                        If Not corrected(0) AndAlso Not corrected(1) AndAlso Not corrected(2) AndAlso Not corrected(3) Then
                            InvalidPolygonError()
                            RepaintPolygon()
                        End If

                        clickedEdges = New List(Of Edge)()
                    End If

                    Return
                End If
            ElseIf menuOption = MenuOption.RemovePolygon Then
                Dim polygonToRemove As Polygon = Me.FindVertex(CType(mouse, Vertex)).Polygon
                If polygonToRemove Is Nothing Then polygonToRemove = Me.FindEdge(CType(mouse, Vertex)).Polygon
                If polygonToRemove IsNot Nothing Then Me.RemovePolygon(polygonToRemove)
            ElseIf menuOption = MenuOption.RemoveRelation Then
                Dim edge As Edge = Me.FindEdge(CType(mouse, Vertex)).edge
                If edge IsNot Nothing AndAlso edge.Relation <> Relation.None Then Me.RemoveRelation(edge)
                RepaintPolygon()
            End If
        End If
    End Sub

    Private Sub OnBoadMouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles picCanvas.MouseDown
        If Not SelectPolygonMode Then
            Return
        End If

        mouse = New Vertex(e.X, e.Y)
        ismouseDown = True
        Dim edgeToHalve As Edge = Nothing, polygon As Polygon = Nothing, index As Integer = Nothing

        If menuOption = MenuOption.MoveComponent Then
            With Me.FindVertex(mouse)
                movingVertex = .Vertex
                movingPolygon = .Polygon
            End With

            If movingVertex Is Nothing Then
                With Me.FindEdge(mouse)
                    movingEdge = .edge
                    movingPolygon = .Polygon
                End With
            End If
            If movingPolygon IsNot Nothing Then vertexCopy = movingPolygon.Vertices.[Select](Function(v) New Vertex(v.X, v.Y)).ToList()
        ElseIf menuOption = MenuOption.MovePolygon Then
            movingPolygon = Me.FindEdge(CType(mouse, Vertex)).Polygon
            If movingPolygon Is Nothing Then movingPolygon = Me.FindVertex(CType(mouse, Vertex)).Polygon
        ElseIf menuOption = MenuOption.HalveEdge Then
            With Me.FindEdgeWithIndex(mouse)
                edgeToHalve = .Item1
                polygon = .Item2
                index = .Item3
            End With

            If edgeToHalve IsNot Nothing Then Me.HalveEdge(polygon, edgeToHalve, index)
        End If
    End Sub

    Private Sub OnBoardMouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles picCanvas.MouseMove
        If Not SelectPolygonMode Then
            Return
        End If

        If menuOption = MenuOption.MoveComponent Then
            If ismouseDown AndAlso movingVertex IsNot Nothing Then
                Me.MoveVertex(movingVertex, e.X, e.Y)
            ElseIf ismouseDown AndAlso movingEdge IsNot Nothing AndAlso Not mouse.IsEmpty() Then
                Dim old As Point = New Point(e.X, e.Y)
                Me.MoveEdge(movingEdge, e.X - mouse.Coord.X, e.Y - mouse.Y)
                Dim correctedCounterclockwise = False, correctedClockwise = False
                correctedCounterclockwise = Me.CorrectCounterclockwise(movingEdge.To.GetOutEdge())
                correctedClockwise = Me.CorrectClockwise(movingEdge.From.GetInEdge())

                If Not correctedClockwise AndAlso Not correctedCounterclockwise Then
                    InvalidPolygonError()
                    RepaintPolygon()
                End If
            End If
        ElseIf menuOption = MenuOption.MovePolygon AndAlso ismouseDown Then

            If movingPolygon IsNot Nothing AndAlso ismouseDown AndAlso Not mouse.IsEmpty() Then
                Dim diffx As Integer = e.X - mouse.Coord.X
                Dim diffy As Integer = e.Y - mouse.Y
                Me.MovePolygon(movingPolygon.Vertices, diffx, diffy)
            End If
        End If

        mouse = New Vertex(e.X, e.Y)
    End Sub

    Private Sub OnBoardMouseUp()
        ismouseDown = False
        movingVertex = Nothing
        movingEdge = Nothing
        movingPolygon = Nothing
        mouse = New Vertex()
    End Sub

    Public Property ShowPointInform As Boolean = True

    Private Sub OnBoardPaint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles picCanvas.Paint
        If polygons.Count = 0 Then
            Return
        End If

        Dim g = e.Graphics
        Dim radius As Single = 6

        For Each polygon In polygons

            For Each vertex In polygon.Vertices
                g.FillEllipse(Brushes.Red, vertex.X - radius, vertex.Y - radius, radius * 2, radius * 2)

                If ShowPointInform Then
                    g.DrawString($"({vertex.X}, {vertex.Y})", New Font("Arial", 10), Brushes.Black, vertex.X - 3, vertex.Y - 20)
                End If
            Next

            If polygon.Edges.Count = 0 Then Continue For

            For Each edge In polygon.Edges
                Dim from As Point = edge.From.Coord
                Dim [to] As Point = edge.To.Coord

                If Not from.IsEmpty AndAlso Not [to].IsEmpty Then
                    Dim brush = If(clickedEdges.IndexOf(edge) <> -1, Brushes.Aqua, Brushes.Black)

                    Select Case algorithmIndex
                        Case 0
                            Me.Bresenham(edge, g, brush)
                        Case 1
                            g.DrawLine(New Pen(brush, 5), from, [to])
                        Case 2
                            Me.AntialiasingWU(edge, g, If(brush Is Brushes.Black, Color.Black, Color.Aqua))
                        Case 3
                            Me.BresenhamSymmetric(edge, g, brush)
                    End Select
                End If
            Next
        Next

        Dim equal = 1, perpendicular = 1
        Dim e1 As Edge = Nothing, e2 As Edge = Nothing

        For Each t As (e1 As Edge, e2 As Edge) In edgesInRelation
            e1 = t.e1
            e2 = t.e2

            Dim x1 As Integer = (Math.Max(e1.From.Coord.X, e1.To.Coord.X) + Math.Min(e1.From.Coord.X, e1.To.Coord.X)) / 2
            Dim y1 As Integer = (Math.Max(e1.From.Y, e1.To.Y) + Math.Min(e1.From.Y, e1.To.Y)) / 2
            Dim x2 As Integer = (Math.Max(e2.From.Coord.X, e2.To.Coord.X) + Math.Min(e2.From.Coord.X, e2.To.Coord.X)) / 2
            Dim y2 As Integer = (Math.Max(e2.From.Y, e2.To.Y) + Math.Min(e2.From.Y, e2.To.Y)) / 2
            Dim bitmap As Bitmap

            If e1.Relation = Relation.Equality Then
                bitmap = New Bitmap(My.Resources.Equals_sign)
                g.DrawString($"({equal})", New Font("Arial", 8), Brushes.Black, x1 + 10, y1 - 8)
                g.DrawString($"({equal})", New Font("Arial", 8), Brushes.Black, x2 + 10, y2 - 8)
                equal += 1
            Else
                bitmap = New Bitmap(My.Resources.perpendicular_mathematical_symbol)
                g.DrawString($"({perpendicular})", New Font("Arial", 8), Brushes.Black, x1 + 10, y1 - 8)
                g.DrawString($"({perpendicular})", New Font("Arial", 8), Brushes.Black, x2 + 10, y2 - 8)
                perpendicular += 1
            End If

            g.DrawImage(bitmap, x1, y1 - 10, 10, 10)
            g.DrawImage(bitmap, x2, y2 - 10, 10, 10)
        Next
    End Sub

    Private Sub DrawSelectionBox(g As Graphics)
        If (endPoint.X < 0) Then endPoint.X = 0
        If (endPoint.X >= picCanvas.Width) Then endPoint.X = picCanvas.Width - 1
        If (endPoint.Y < 0) Then endPoint.Y = 0
        If (endPoint.Y >= picCanvas.Height) Then endPoint.Y = picCanvas.Height - 1

        ' Draw the selection area.
        Dim x = Math.Min(startPoint.X, endPoint.X)
        Dim y = Math.Min(startPoint.Y, endPoint.Y)
        Dim width = Math.Abs(startPoint.X - endPoint.X)
        Dim height = Math.Abs(startPoint.Y - endPoint.Y)

        g.DrawRectangle(Pens.Red, x, y, width, height)
    End Sub

    ''' <summary>
    ''' Draw the area selected.
    ''' </summary>
    ''' <param name="end_point"></param>
    Private Sub DrawSelectionBox(end_point As Point)
        ' Save the end point.
        endPoint = end_point
        picCanvas.Invalidate()
    End Sub

    Private Sub RepaintPolygon()
        picCanvas.Invalidate()
    End Sub

    Private Function IsLastPolygonCorrect() As Boolean
        If polygons.Count > 0 AndAlso polygons(polygons.Count - 1).Vertices.Count < 3 AndAlso polygons(polygons.Count - 1).Vertices.Count > 0 Then
            MessageBox.Show("A figure has to have more than 2 vertices - deleting last polygon")
            polygons.RemoveAt(polygons.Count - 1)
            RepaintPolygon()
            Dim vertices As List(Of Vertex) = New List(Of Vertex)()
            Dim edges As List(Of Edge) = New List(Of Edge)()
            polygons.Add(New Polygon(vertices, edges))
            Return False
        End If

        Return True
    End Function

    Private Sub InvalidPolygonError()
        MessageBox.Show("Couldn't preserve relations while moving polygon - " & "restoring last correct position")
        ismouseDown = False
        movingVertex = Nothing
        movingEdge = Nothing
        Dim i = 0

        For Each vertex In movingPolygon.Vertices
            vertex.Coord = New Point(vertexCopy(i).X, vertexCopy(i).Y)
            i += 1
        Next

        vertexCopy = New List(Of Vertex)()
        movingPolygon = Nothing
        mouse = New Vertex()
    End Sub

    Private Function BelongsToCircle(ByVal vertex As Vertex, ByVal circle As Vertex) As Boolean
        Dim d = Math.Sqrt((vertex.X - circle.Coord.X) * (vertex.X - circle.Coord.X) + (vertex.Y - circle.Y) * (vertex.Y - circle.Y))
        If d <= 3 Then Return True
        Return False
    End Function

    Private Function BelongsToSegment(ByVal vertex As Vertex, ByVal segment As Edge) As Boolean
        Dim x As Integer = vertex.Coord.X
        Dim y As Integer = vertex.Y
        Dim ax As Integer = segment.From.Coord.X
        Dim ay As Integer = segment.From.Y
        Dim bx As Integer = segment.To.Coord.X
        Dim by As Integer = segment.To.Y
        Dim dx As Double = bx - ax
        Dim dy As Double = by - ay
        Dim len = Math.Sqrt(dx * dx + dy * dy)
        Dim d = (dy * (y - ay) - dx * (x - ax)) / len
        If Math.Abs(d) < 4 AndAlso x >= Math.Min(ax, bx) AndAlso x <= Math.Max(ax, bx) AndAlso y >= Math.Min(ay, by) AndAlso y <= Math.Max(ay, by) Then Return True
        Dim u = ((bx - ax) * (x - ax) + (by - ay) * (y - ay)) / ((bx - ax) * (bx - ax) + (by - ay) * (by - ay))
        Dim x3 As Integer = ax + u * (bx - ax)
        Dim y3 As Integer = ay + u * (by - ay)
        If Me.CalculateLength(New Edge(New Vertex(x3, y3), New Vertex(x, y))) < 3 AndAlso x >= Math.Min(ax, bx) - 1 AndAlso x <= Math.Max(ax, bx) + 1 AndAlso y >= Math.Min(ay, by) - 1 AndAlso y <= Math.Max(ay, by) + 1 Then Return True
        Return False
    End Function

    Private Function ArePerpendicular(ByVal edge1 As Edge, ByVal edge2 As Edge) As Boolean
        Dim a1, a2 As Double

        If Me.CalculateLength(edge1) = 0 Then
            edge1.From.X += 1
            edge1.To.Y += 1
        End If

        If Me.CalculateLength(edge2) = 0 Then
            edge2.From.X += 1
            edge2.To.Y += 1
        End If

        If edge1.To.X - edge1.From.X = 0 AndAlso edge2.To.X - edge2.From.X = 0 Then
            edge2.To.X += 1
            edge2.To.Y += 1
        End If

        If edge1.To.X - edge1.From.X = 0 Then
            a2 = edge2.To.Y - edge2.From.Y

            If a2 = 0 Then
                Return True
            Else
                Return False
            End If
        ElseIf edge2.To.X - edge2.From.X = 0 Then
            a1 = edge1.To.Y - edge1.From.Y

            If a1 = 0 Then
                Return True
            Else
                Return False
            End If
        Else
            a2 = CDbl(edge2.To.Y - edge2.From.Y) / (edge2.To.X - edge2.From.X)
            a1 = CDbl(edge1.To.Y - edge1.From.Y) / (edge1.To.X - edge1.From.X)

            If a1 * a2 = -1 Then
                Return True
            Else
                Return False
            End If
        End If
    End Function

    Private Function AreEqual(ByVal edge1 As Edge, ByVal edge2 As Edge) As Boolean
        If Me.CalculateLength(edge1) = 0 Then
            edge1.From.X += 1
            edge1.To.Y += 1
        End If

        If Me.CalculateLength(edge2) = 0 Then
            edge2.From.X += 1
            edge1.To.Y += 1
        End If

        Return Math.Abs(Me.CalculateLength(edge1) - Me.CalculateLength(edge2)) <= 1
    End Function

    Private Function CalculateLength(ByVal e As Edge) As Integer
        Return Math.Sqrt(Math.Abs(e.To.Y - e.From.Y) * Math.Abs(e.To.Y - e.From.Y) + Math.Abs(e.To.X - e.From.Coord.X) * Math.Abs(e.To.Coord.X - e.From.Coord.X))
    End Function

    Private Sub Bresenham(ByVal edge As Edge, ByVal graphics As Graphics, ByVal brush As Brush)
        Dim x0 As Integer = edge.From.X
        Dim y0 As Integer = edge.From.Y
        Dim x1 As Integer = edge.To.X
        Dim y1 As Integer = edge.To.Y
        Dim dx = Math.Abs(x1 - x0)
        Dim dy = Math.Abs(y1 - y0)
        Dim kx = If(x0 < x1, 1, -1)
        Dim ky = If(y0 < y1, 1, -1)
        Dim err As Integer = If(dx > dy, dx, -1 * dy) / 2
        Dim e2 As Integer

        While x0 <> x1 OrElse y0 <> y1
            graphics.FillRectangle(brush, x0, y0, 1, 1)
            e2 = err

            If e2 + dx > 0 Then
                err -= dy
                x0 += kx
            End If

            If e2 - dy < 0 Then
                err += dx
                y0 += ky
            End If
        End While

        graphics.FillRectangle(brush, x0, y0, 1, 1)
    End Sub

    ''' <summary>
    ''' change polygon algorithm
    ''' </summary>
    Public Sub toolStripMenuItem1_Click()
        If Not IsLastPolygonCorrect() Then Return
        algorithmIndex = (algorithmIndex + 1) Mod 4

        Select Case algorithmIndex
            Case 0
                MessageBox.Show("Drawing with Bresenham algorithm")
            Case 1
                MessageBox.Show("Drawing with library algorithm")
            Case 2
                MessageBox.Show("Drawing with antialiasing algorithm")
            Case 3
                MessageBox.Show("Drawing with Symmetric Bresenham algorithm")
        End Select

        RepaintPolygon()
    End Sub

    Private Function Ipart(ByVal x As Double) As Integer
        Return x
    End Function

    Private Function Round(ByVal x As Double) As Integer
        Return Ipart(x + 0.5)
    End Function

    Public Shared Function Frac(ByVal x As Double) As Double
        If x < 0 Then Return 1 - (x - Math.Floor(x))
        Return x - Math.Floor(x)
    End Function

    Private Function RFrac(ByVal x As Double) As Double
        Return 1 - Frac(x)
    End Function

    Private Sub Plot(ByVal g As Graphics, ByVal x As Double, ByVal y As Double, ByVal c As Double, ByVal main As Color)
        Dim alpha As Integer = c * 255
        If alpha > 255 Then alpha = 255
        If alpha < 0 Then alpha = 0
        Dim color As Color = Color.FromArgb(alpha, main)
        g.FillRectangle(New SolidBrush(color), CInt(x), CInt(y), 1, 1)
    End Sub

    Private Sub AntialiasingWU(ByVal edge As Edge, ByVal graphics As Graphics, ByVal color As Color)
        Dim x0 As Double = edge.From.X
        Dim y0 As Double = edge.From.Y
        Dim x1 As Double = edge.To.X
        Dim y1 As Double = edge.To.Y
        Dim dx = Math.Abs(x1 - x0)
        Dim dy = Math.Abs(y1 - y0)
        Dim steep = dy > dx

        If steep Then
            x0.Swap(y0)
            x1.Swap(y1)
        End If

        If x0 > x1 Then
            x0.Swap(x1)
            y0.Swap(y1)
        End If

        dx = x1 - x0
        dy = y1 - y0
        Dim gradient = dy / dx
        Dim xend As Double = Round(x0)
        Dim yend = y0 + gradient * (xend - x0)
        Dim xgap = RFrac(x0 + 0.5)
        Dim xpxl1 = xend
        Dim ypxl1 As Double = Ipart(yend)

        If steep Then
            Plot(graphics, ypxl1, xpxl1, RFrac(yend) * xgap, color)
            Me.Plot(graphics, ypxl1 + 1, xpxl1, Frac(yend) * xgap, color)
        Else
            Plot(graphics, xpxl1, ypxl1, RFrac(yend) * xgap, color)
            Me.Plot(graphics, xpxl1, ypxl1 + 1, Frac(yend) * xgap, color)
        End If

        Dim intery = yend + gradient
        xend = Round(x1)
        yend = y1 + gradient * (xend - x1)
        xgap = RFrac(x1 + 0.5)
        Dim xpxl2 = xend
        Dim ypxl2 As Double = Ipart(yend)

        If steep Then
            Plot(graphics, ypxl2, xpxl2, RFrac(yend) * xgap, color)
            Me.Plot(graphics, ypxl2 + 1, xpxl2, Frac(yend) * xgap, color)
        Else
            Plot(graphics, xpxl2, ypxl2, RFrac(yend) * xgap, color)
            Me.Plot(graphics, xpxl2, ypxl2 + 1, Frac(yend) * xgap, color)
        End If

        If steep Then
            For x As Integer = xpxl1 + 1 To xpxl2 - 1
                Plot(graphics, Ipart(intery), x, RFrac(intery), color)
                Me.Plot(graphics, Ipart(intery) + 1, x, Frac(intery), color)
                intery += gradient
            Next
        Else

            For x As Integer = xpxl1 + 1 To xpxl2 - 1
                Plot(graphics, x, Ipart(intery), RFrac(intery), color)
                Me.Plot(graphics, x, Ipart(intery) + 1, Frac(intery), color)
                intery += gradient
            Next
        End If
    End Sub

    Private Sub BresenhamSymmetric(ByVal edge As Edge, ByVal graphics As Graphics, ByVal brush As Brush)
        Dim x0 As Integer = edge.From.X
        Dim y0 As Integer = edge.From.Y
        Dim x1 As Integer = edge.To.X
        Dim y1 As Integer = edge.To.Y
        Dim dx = Math.Abs(x1 - x0)
        Dim dy = Math.Abs(y1 - y0)
        Dim kx = If(x0 < x1, 1, -1)
        Dim ky = If(y0 < y1, 1, -1)
        Dim err As Integer = If(dx > dy, dx, -1 * dy) / 2
        Dim e2 As Integer
        Dim xf = x0
        Dim yf = y0
        Dim xb = x1
        Dim yb = y1

        While True
            graphics.FillRectangle(brush, xf, yf, 1, 1)
            graphics.FillRectangle(brush, xb, yb, 1, 1)
            e2 = err
            If Math.Abs(xf - xb) <= 1 AndAlso Math.Abs(yf - yb) <= 1 Then Exit While

            If e2 + dx > 0 Then
                err -= dy
                xf += kx
                xb -= kx
            End If

            If e2 - dy < 0 Then
                err += dx
                yf += ky
                yb -= ky
            End If
        End While
    End Sub

#End Region

    Dim orginal_imageSize As Size
    Dim orginal_image As Image
    Dim dimension As Size
    Dim range As Double()
    Dim mapLevels As Integer

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MSImage As Image
        Get
            Return picCanvas.BackgroundImage
        End Get
    End Property

    Public Sub SetColorMapVisible(visible As Boolean)
        If range.IsNullOrEmpty AndAlso mapLevels = 0 Then
            ColorScaleMap1.Visible = False
        Else
            ColorScaleMap1.Visible = visible
        End If
    End Sub

    Public Sub SetMsImagingOutput(value As Image, pixel_size As Size, colorMap As ScalerPalette, range As Double(), mapLevels As Integer)
        Me.dimension = pixel_size
        Me.orginal_image = value
        Me.range = range
        Me.mapLevels = mapLevels

        If range.IsNullOrEmpty AndAlso mapLevels = 0 Then
            ColorScaleMap1.Visible = False
        Else
            ColorScaleMap1.Visible = True
            ColorScaleMap1.colorMap = colorMap
            ColorScaleMap1.range = range
            ColorScaleMap1.mapLevels = mapLevels
        End If

        If value IsNot Nothing AndAlso (dimension.Width = 0 OrElse dimension.Height = 0) Then
            Throw New InvalidExpressionException("dimension size can not be ZERO!")
        End If

        If value Is Nothing Then
            orginal_imageSize = Nothing
        Else
            orginal_imageSize = value.Size
            orginal_imageSize = New Size With {
                .Width = orginal_imageSize.Width / dimension.Width,
                .Height = orginal_imageSize.Height / dimension.Height
            }
        End If

        Call renderWithLegend(orginal_image.Clone, oldBackColor)
    End Sub

    Private Sub renderWithLegend(image As Image, backColor As Color)
        'If Not colorLegend Is Nothing Then
        '    Using g As Graphics = Graphics.FromImage(image)
        '        Dim size As New Size(image.Width / 8, image.Height / 2)
        '        Dim pos As New Point(image.Width - size.Width, 25)

        '        Call g.DrawImage(colorLegend, New Rectangle(pos, size))
        '    End Using
        'End If

        picCanvas.BackgroundImage = image
        Me.oldBackColor = Me.BackColor
        Me.BackColor = backColor
    End Sub

    Dim oldBackColor As Color = Color.White
    Dim oldMessage As String = "MSI Viewer"

    Public Sub ShowMessage(text As String)
        ToolStripStatusLabel1.Text = text
    End Sub

    Dim drawing As Boolean = False
    Dim startPoint, endPoint As Point
    Dim rangeStart As Point
    Dim rangeEnd As Point

    Public ReadOnly Property HasRegionSelection As Boolean
        Get
            Return Not startPoint.IsEmpty OrElse endPoint.IsEmpty
        End Get
    End Property

    Public ReadOnly Property RegionSelectin As Rectangle
        Get
            Dim left As Integer = stdNum.Min(startPoint.X, endPoint.X)
            Dim top As Integer = stdNum.Min(startPoint.Y, endPoint.Y)
            Dim right As Integer = stdNum.Max(startPoint.X, endPoint.X)
            Dim bottom As Integer = stdNum.Max(startPoint.Y, endPoint.Y)

            Return New Rectangle(left, top, right - left, bottom - top)
        End Get
    End Property

    Public Property SelectPolygonMode As Boolean = False

    Public Sub ClearSelection()
        startPoint = Nothing
        endPoint = Nothing
    End Sub

    Sub canvasMouseDown(sender As Object, e As MouseEventArgs) Handles picCanvas.MouseDown
        If e.Button <> MouseButtons.Left Then
            If e.Button = MouseButtons.Right Then
                drawing = False
            End If

            Return
        End If

        Dim xpoint = 0
        Dim ypoint = 0

        drawing = True
        startPoint = e.Location

        getPoint(e, xpoint, ypoint)
        DrawSelectionBox(startPoint)

        rangeStart = New Point(xpoint, ypoint)

        oldMessage = ToolStripStatusLabel1.Text
        ShowMessage("Select Pixels By Range.")
    End Sub

    Private Sub picCanvas_MouseMove(sender As Object, e As MouseEventArgs) Handles picCanvas.MouseMove
        If Not picCanvas.BackgroundImage Is Nothing Then
            Dim xpoint = 0
            Dim ypoint = 0

            getPoint(e, xpoint, ypoint)
            ToolStripStatusLabel2.Text = $"[{xpoint}, {ypoint}]"
        End If

        If Not drawing Then
            Return
        End If
        If SelectPolygonMode Then
            Return
        End If

        DrawSelectionBox(e.Location)
    End Sub

    Private Sub getPoint(e As MouseEventArgs, ByRef xpoint As Integer, ByRef ypoint As Integer)
        Dim Pic_width = orginal_imageSize.Width / picCanvas.Width
        Dim Pic_height = orginal_imageSize.Height / picCanvas.Height

        ' 得到图片上的坐标点
        xpoint = e.X * Pic_width
        ypoint = e.Y * Pic_height
    End Sub

    Private Sub picCanvas_MouseUp(sender As Object, e As MouseEventArgs) Handles picCanvas.MouseUp
        If SelectPolygonMode Then
            Call OnBoardMouseUp()
        ElseIf Not drawing Then
            Return
        Else
            Dim xpoint = 0
            Dim ypoint = 0

            getPoint(e, xpoint, ypoint)
            ShowMessage(oldMessage)

            rangeEnd = New Point(xpoint, ypoint)
            drawing = False

            RaiseEvent SelectPixelRegion(RegionSelectin)
        End If
    End Sub

    Public ReadOnly Property Pixel As Point

    Private Sub clickGetPoint(e As MouseEventArgs)
        Dim xpoint = 0
        Dim ypoint = 0

        Call getPoint(e, xpoint, ypoint)

        _Pixel = New Point(xpoint, ypoint)

        If e.Button <> MouseButtons.Left Then
            Return
        Else
            drawing = False
            RaiseEvent SelectPixel(xpoint, ypoint)
        End If
    End Sub

    Private Sub PixelSelector_Load(sender As Object, e As EventArgs) Handles Me.Load
        Timer1.Enabled = True
        Timer1.Start()

        Call SetColorMapVisible(visible:=False)
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If HasRegionSelection Then
            ' DrawSelectionBox(endPoint, False)
        End If
    End Sub

    Public cancelBlur As Boolean = False

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="level"></param>
    ''' <param name="progress">[0,1]</param>
    Public Sub doGauss(level As Integer, contrast As Double, progress As Action(Of Double))
        If orginal_image Is Nothing Then
            Return
        End If

        Dim bmp As New Bitmap(orginal_image)
        Dim color As Color = Me.oldBackColor

        cancelBlur = False

        If contrast <> 0 Then
            Call BitmapScale.AdjustContrast(bmp, contrast)
        End If

        If level > 0 Then
            color = Color.Black
            renderWithLegend(orginal_image, color)

            For i As Integer = 0 To level
                bmp = GaussBlur.GaussBlur(bmp)
                progress(i / level * 100)
                picCanvas.BackgroundImage = bmp

                If cancelBlur Then
                    Exit For
                End If

                Call Application.DoEvents()
            Next
        End If

        Call renderWithLegend(bmp, color)
    End Sub
End Class
