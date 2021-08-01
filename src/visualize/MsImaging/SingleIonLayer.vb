Imports System.Drawing

Public Class SingleIonLayer

    Public Property IonMz As Double
    Public Property MSILayer As PixelData()
    Public Property DimensionSize As Size

    Public Function GetIntensity() As Double()
        Return MSILayer.Select(Function(p) p.intensity).ToArray
    End Function

End Class
