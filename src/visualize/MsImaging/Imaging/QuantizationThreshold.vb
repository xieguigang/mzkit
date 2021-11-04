Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.Quantile

Namespace Imaging

    Public MustInherit Class QuantizationThreshold

        ''' <summary>
        ''' auto check for intensity cut threshold value
        ''' </summary>
        ''' <param name="intensity"></param>
        ''' <returns>
        ''' percentage cutoff value
        ''' </returns>
        Public MustOverride Function ThresholdValue(intensity As Double(), q As Double) As Double

        Public Shared Function RankQuantileThreshold(intensity As Double(), qcut As Double) As Double
            Static q As New RankQuantileThreshold
            Return q.ThresholdValue(intensity, qcut)
        End Function

        Public Shared Function TrIQThreshold(intensity As Double(), qcut As Double) As Double
            Static q As New TrIQThreshold
            Return q.ThresholdValue(intensity, qcut)
        End Function
    End Class

    Public Class RankQuantileThreshold : Inherits QuantizationThreshold

        ''' <summary>
        ''' auto check for intensity cut threshold value
        ''' </summary>
        ''' <param name="intensity"></param>
        ''' <returns>
        ''' percentage cutoff value
        ''' </returns>
        Public Overrides Function ThresholdValue(intensity As Double(), qcut As Double) As Double
            If intensity.IsNullOrEmpty Then
                Return 0
            Else
                Dim maxBin As Double() = intensity.TabulateBin(topBin:=True, bags:=5)
                Dim qtile As Double = New FastRankQuantile(maxBin).Query(qcut)
                Dim per As Double = qtile / intensity.Max

                Return per
            End If
        End Function
    End Class

    Public Class TrIQThreshold : Inherits QuantizationThreshold

        Public Overrides Function ThresholdValue(intensity() As Double, qcut As Double) As Double
            If intensity.IsNullOrEmpty Then
                Return 0
            Else
                Return intensity.FindThreshold(qcut, N:=64) / intensity.Max
            End If
        End Function
    End Class
End Namespace