Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq

Public Class LayerRender

    Public Property alphaLevel As Double = 0.8
    Public Property dotSize As New Size(3, 3)

    ''' <summary>
    ''' a list of the region labels for highlights
    ''' </summary>
    ''' <returns></returns>
    Public Property highlights As String()

    Public Sub Rendering(g As Graphics, regions As IEnumerable(Of TissueRegion))
        Dim highlights As Index(Of String) = Me.highlights.SafeQuery.Distinct.Indexing

        For Each region As TissueRegion In regions
            Dim fill As New SolidBrush(region.color.Alpha(255 * alphaLevel))

            If region.label Like highlights Then
                fill = New SolidBrush(region.color.Darken(0.25))
            End If

            For Each p As Point In region.points
                p = New Point(p.X * dotSize.Width, p.Y * dotSize.Height)
                g.FillRectangle(fill, New Rectangle(p, dotSize))
            Next
        Next
    End Sub

    Public Shared Function Draw(regions As IEnumerable(Of TissueRegion),
                                layerSize As Size,
                                Optional alphaLevel As Double = 0.8,
                                Optional dotSize As Integer = 3,
                                Optional highlights As String() = Nothing) As Bitmap

        Dim layer As New Bitmap(
            width:=layerSize.Width * dotSize,
            height:=layerSize.Height * dotSize,
            format:=PixelFormat.Format32bppArgb
        )
        Dim g As Graphics = Graphics.FromImage(layer)
        Dim blender As New LayerRender With {
            .alphaLevel = alphaLevel,
            .dotSize = New Size(dotSize, dotSize),
            .highlights = highlights
        }

        g.CompositingQuality = CompositingQuality.HighQuality
        g.InterpolationMode = InterpolationMode.HighQualityBilinear
        g.Clear(Color.Transparent)
        blender.Rendering(g, regions)
        g.Flush()
        g.Dispose()

        Return layer
    End Function
End Class
