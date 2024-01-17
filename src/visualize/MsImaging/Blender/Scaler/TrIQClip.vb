Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler

Namespace Blender.Scaler

    ''' <summary>
    ''' removes the spot where it has intensity value greater than the TrIQ threshold
    ''' </summary>
    Public Class TrIQClip : Inherits Scaler

        Public Property threshold As Double
        Public Property N As Integer

        Sub New(threshold As Double, N As Integer)
            Me.threshold = threshold
            Me.N = N
        End Sub

        Sub New()
            Call Me.New(0.8, 100)
        End Sub

        Public Overrides Function DoIntensityScale(layer As SingleIonLayer) As SingleIonLayer
            Dim q As Double = TrIQ.FindThreshold(layer.GetIntensity, threshold, N:=N)
            Dim hard_clipping As PixelData() = layer.MSILayer _
                .Where(Function(s) s.intensity < q) _
                .ToArray

            Return New SingleIonLayer With {
                .MSILayer = hard_clipping,
                .DimensionSize = layer.DimensionSize,
                .IonMz = layer.IonMz
            }
        End Function

        Public Overrides Function ToScript() As String
            Return $"TrIQ_clip({threshold},{N})"
        End Function
    End Class
End Namespace