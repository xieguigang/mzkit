Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.Quantile

Namespace Imaging

    Public Delegate Function IThreshold(intensity As Double(), q As Double) As Double
    Public Delegate Function IQuantizationThreshold(intensity As Double()) As Double

    Public MustInherit Class QuantizationThreshold

        Public Property qcut As Double = 0.65

        ''' <summary>
        ''' auto check for intensity cut threshold value
        ''' </summary>
        ''' <param name="intensity"></param>
        ''' <returns>
        ''' percentage cutoff value
        ''' </returns>
        Public MustOverride Function ThresholdValue(intensity As Double(), q As Double) As Double

        Public Function ThresholdValue(intensity As Double()) As Double
            Return ThresholdValue(intensity, qcut)
        End Function

        Public Shared Function RankQuantileThreshold(intensity As Double(), qcut As Double) As Double
            Static q As New RankQuantileThreshold
            Return q.ThresholdValue(intensity, qcut)
        End Function

        Public Shared Function TrIQThreshold(intensity As Double(), qcut As Double) As Double
            Static q As New TrIQThreshold
            Return q.ThresholdValue(intensity, qcut)
        End Function

        'Public Shared Narrowing Operator CType(q As QuantizationThreshold) As IQuantizationThreshold
        '    Return AddressOf q.ThresholdValue
        'End Operator
    End Class

    Public Class RankQuantileThreshold : Inherits QuantizationThreshold

        Sub New()
        End Sub

        Sub New(q As Double)
            qcut = q
        End Sub

        Public Overrides Function ToString() As String
            If qcut > 0 Then
                Return $"Quantile({qcut})"
            Else
                Return "Quantile"
            End If
        End Function

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

        Sub New()
        End Sub

        Sub New(q As Double)
            qcut = q
        End Sub

        Public Overrides Function ThresholdValue(intensity() As Double, qcut As Double) As Double
            If intensity.IsNullOrEmpty Then
                Return 0
            Else
                Return intensity.FindThreshold(qcut, N:=64) / intensity.Max
            End If
        End Function

        Public Overrides Function ToString() As String
            If qcut > 0 Then
                Return $"TrIQ({qcut})"
            Else
                Return "TrIQ"
            End If
        End Function
    End Class
End Namespace