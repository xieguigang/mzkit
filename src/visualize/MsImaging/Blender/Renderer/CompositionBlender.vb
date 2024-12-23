Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Namespace Blender

    Public MustInherit Class CompositionBlender

        Protected ReadOnly defaultBackground As Color
        Protected ReadOnly heatmapMode As Boolean

        Public Property dimension As Size

        Sub New(defaultBackground As Color, Optional heatmapMode As Boolean = False)
            Me.heatmapMode = heatmapMode
            Me.defaultBackground = defaultBackground
        End Sub

        ''' <summary>
        ''' do imaging heatmap rendering
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="region"></param>
        Public MustOverride Sub Render(ByRef g As IGraphics, region As GraphicsRegion)

    End Class
End Namespace