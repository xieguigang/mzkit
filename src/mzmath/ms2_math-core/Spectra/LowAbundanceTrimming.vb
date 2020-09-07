Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Math.Quantile

Public MustInherit Class LowAbundanceTrimming

    Protected ReadOnly threshold As Double

    Sub New(cutoff As Double)
        If cutoff > 1 Then
            threshold = cutoff / 100
        Else
            threshold = cutoff
        End If

        If cutoff <= 0 Then
            Call $"the threshold value for trimming low abundance fragment is ZERO or negative value, no item will be trimmed!".Warning
        End If
    End Sub

    Public Shared ReadOnly Property intoCutff As New RelativeIntensityCutoff(0.05)
    Public Shared ReadOnly Property quantCutoff As New QuantileIntensityCutoff(0.65)

    Public Function Trim(spectrum As IEnumerable(Of ms2)) As ms2()
        If threshold <= 0 Then
            Return spectrum.ToArray
        Else
            Return lowAbundanceTrimming(spectrum.ToArray)
        End If
    End Function

    Protected MustOverride Function lowAbundanceTrimming(spectrum As ms2()) As ms2()
    Public MustOverride Overrides Function ToString() As String

End Class

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

        Return spectrum.Where(Function(a) a.quantity >= threshold).ToArray
    End Function

    Public Overrides Function ToString() As String
        Return $"relative_intensity >= {threshold * 100}%"
    End Function

End Class

Public Class QuantileIntensityCutoff : Inherits LowAbundanceTrimming

    Public Sub New(cutoff As Double)
        MyBase.New(cutoff)
    End Sub

    Protected Overrides Function lowAbundanceTrimming(spectrum() As ms2) As ms2()
        Dim quantile = spectrum.Select(Function(a) a.intensity).GKQuantile
        Dim threshold As Double = quantile.Query(Me.threshold)

        Return spectrum.Where(Function(a) a.intensity >= threshold).ToArray
    End Function

    Public Overrides Function ToString() As String
        Return $"intensity_quantile >= {threshold}"
    End Function

End Class