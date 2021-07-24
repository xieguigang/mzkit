Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging.BitmapImage

Namespace MarkupData.imzML

    ''' <summary>
    ''' 在这里必须要假设每一个像素点的扫描时间是等长的
    ''' </summary>
    Public Class Correction

        Public ReadOnly Property totalTime As Double
        ''' <summary>
        ''' pixels in row or total pixels by width times height
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property pixels As Integer
        Public ReadOnly Property pixelsTime As Double

        Sub New(totalTime As Double, pixels As Integer)
            Me.totalTime = totalTime
            Me.pixels = pixels
            Me.pixelsTime = totalTime / pixels
        End Sub

        ''' <summary>
        ''' if the raw data file is row scans
        ''' </summary>
        ''' <param name="rt"></param>
        ''' <returns>X of the point</returns>
        Public Function GetPixelRow(rt As Double) As Integer
            Return 1 + CInt(rt / pixelsTime)
        End Function

        ''' <summary>
        ''' if the raw data file is 2D scans
        ''' </summary>
        ''' <param name="rt"></param>
        ''' <returns>[x, y]</returns>
        Public Function GetPixelPoint(rt As Double) As Point
            ' 在这个二维扫描之中，已经有了n个像素点了
            Dim n As Integer = GetPixelRow(rt)
            Dim pt As Point = BitmapBuffer.ToPixel2D(n, width:=pixels, channels:=1)

            Return pt
        End Function

    End Class
End Namespace