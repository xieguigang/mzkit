Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Class TrIQScaler : Inherits Scaler

    ReadOnly threshold As Double

    Sub New(Optional threshold As Double = 0.999)
        Me.threshold = threshold
    End Sub

    Protected Overrides Function DoIntensityScale(into() As Double) As Double()
        Dim q As Double = TrIQ.FindThreshold(into, threshold)
        Dim v As New Vector(into)

        v(v > q) = Vector.Scalar(q)

        Return v
    End Function
End Class
