
Namespace Blender.Scaler

    Public Class DenoiseScaler : Inherits Scaler

        ReadOnly q As Double

        Sub New(Optional q As Double = 0.01)
            Me.q = q
        End Sub

        Public Overrides Function DoIntensityScale(layer As SingleIonLayer) As SingleIonLayer
            Dim denoise As PixelData() = layer.MSILayer _
                .DensityCut(q) _
                .ToArray

            Return New SingleIonLayer With {
                .MSILayer = denoise,
                .DimensionSize = layer.DimensionSize,
                .IonMz = layer.IonMz
            }
        End Function
    End Class
End Namespace