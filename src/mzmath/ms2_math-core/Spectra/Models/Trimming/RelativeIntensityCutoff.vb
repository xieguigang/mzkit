
Namespace Spectra

    Public Class RelativeIntensityCutoff : Inherits LowAbundanceTrimming

        Public Sub New(cutoff As Double)
            MyBase.New(cutoff)
        End Sub

        Protected Overrides Function lowAbundanceTrimming(spectrum() As ms2) As ms2()
            Dim maxInto As Double = -999

            For Each fragment As ms2 In spectrum
                If fragment.intensity > maxInto Then
                    maxInto = fragment.intensity
                End If
            Next

            For Each fragment As ms2 In spectrum
                fragment.quantity = fragment.intensity / maxInto
            Next

            Return spectrum.Where(Function(a) a.quantity >= m_threshold).ToArray
        End Function

        Public Overrides Function ToString() As String
            Return $"relative_intensity >= {m_threshold * 100}%"
        End Function

    End Class
End Namespace