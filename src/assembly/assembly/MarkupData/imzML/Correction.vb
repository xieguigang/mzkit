Imports System.Drawing

Namespace MarkupData.imzML

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
        ''' <returns></returns>
        Public Function GetPixelRow(rt As Double) As Integer
            Return 1 + CInt(rt / pixelsTime)
        End Function

        ''' <summary>
        ''' if the raw data file is 2D scans
        ''' </summary>
        ''' <param name="rt"></param>
        ''' <returns></returns>
        Public Function GetPixelPoint(rt As Double, width As Integer, height As Integer) As Point
            Dim n As Integer = GetPixelRow(rt)

            Throw New NotImplementedException
        End Function

    End Class
End Namespace