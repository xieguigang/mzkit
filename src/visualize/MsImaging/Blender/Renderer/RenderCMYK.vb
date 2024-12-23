Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Namespace Blender

    Public Class RenderCMYK : Inherits CompositionBlender

        ''' <summary>
        ''' Cyan
        ''' </summary>
        ''' <returns></returns>
        Public Property Cchannel As Func(Of Integer, Integer, Byte)
        ''' <summary>
        ''' Magenta
        ''' </summary>
        ''' <returns></returns>
        Public Property Mchannel As Func(Of Integer, Integer, Byte)
        ''' <summary>
        ''' Yellow
        ''' </summary>
        ''' <returns></returns>
        Public Property Ychannel As Func(Of Integer, Integer, Byte)
        ''' <summary>
        ''' Key/Black
        ''' </summary>
        ''' <returns></returns>
        Public Property Kchannel As Func(Of Integer, Integer, Byte)

        Public Sub New(defaultBackground As Color, Optional heatmapMode As Boolean = False)
            MyBase.New(defaultBackground, heatmapMode)
        End Sub

        Public Overrides Sub Render(ByRef g As IGraphics, region As GraphicsRegion)

        End Sub
    End Class
End Namespace