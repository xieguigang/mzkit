Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Distributions
Imports std = System.Math

Namespace Blender.Scaler

    Public Class ZScoreScaler : Inherits Scaler

        Public Overrides Function DoIntensityScale(into() As Double) As Double()
            If into.Any Then
                Dim z As Double() = into.Z
                Dim min As Double = std.Abs(z.Min)

                ' zscore(v) = [-x1,x1]
                ' zscore_norm = z + abs(x1)
                into = SIMD.Add.f64_op_add_f64_scalar(z, min)
            End If

            Return into
        End Function

        Public Overrides Function ToScript() As String
            Return "zscore_norm()"
        End Function
    End Class
End Namespace