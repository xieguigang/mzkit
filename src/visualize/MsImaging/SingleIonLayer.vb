Imports System.Drawing
Imports Microsoft.VisualBasic.Math.Quantile

Public Class SingleIonLayer

    Public Property IonMz As Double
    Public Property MSILayer As PixelData()
    Public Property DimensionSize As Size

    Public Function GetIntensity() As Double()
        Return MSILayer.Select(Function(p) p.intensity).ToArray
    End Function

    Public Function GetQuartile() As DataQuartile
        Return GetIntensity.Quartile
    End Function

End Class
