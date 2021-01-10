Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports stdNum = System.Math

''' <summary>
''' ROI Peak data of the given ion
''' </summary>
Public Class IonTPA

    Public Property name As String
    Public Property rt As Double
    Public Property peakROI As DoubleRange
    Public Property area As Double
    Public Property baseline As Double
    Public Property maxPeakHeight As Double

    Public Overrides Function ToString() As String
        Return $"{name}[{peakROI.Min}, {stdNum.Round(peakROI.Max)}] = {area}"
    End Function

End Class