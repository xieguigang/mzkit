Imports System.Drawing
Imports System.Runtime.CompilerServices
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

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Shared Function HeatmapBlending(channel As Func(Of Integer, Integer, Byte), x As Integer, y As Integer) As Byte
            Return New Integer() {
                channel(x - 1, y - 1), channel(x, y - 1), channel(x + 1, y - 1),
                channel(x - 1, y), channel(x, y), channel(x + 1, y),
                channel(x - 1, y + 1), channel(x, y + 1), channel(x + 1, y + 1)
            }.Average
        End Function
    End Class
End Namespace