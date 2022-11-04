
Imports System.Runtime.CompilerServices

Namespace Blender.Scaler

    Public Class KNNScaler : Inherits Scaler

        ReadOnly k As Integer
        ReadOnly q As Double

        Public Sub New(Optional k As Integer = 3, Optional q As Double = 0.65)
            Me.k = k
            Me.q = q
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function DoIntensityScale(layer As SingleIonLayer) As SingleIonLayer
            Return layer.KnnFill(k, k, q)
        End Function
    End Class
End Namespace