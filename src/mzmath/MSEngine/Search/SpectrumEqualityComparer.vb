Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports std = System.Math

Public Class SpectrumEqualityComparer
    Implements IEqualityComparer(Of SpectrumPeak)
    Private Shared ReadOnly EPS As Double = 1000000.0
    Public Overloads Function Equals(x As SpectrumPeak, y As SpectrumPeak) As Boolean Implements IEqualityComparer(Of SpectrumPeak).Equals
        Return std.Abs(x.mz - y.mz) <= EPS
    End Function

    Public Overloads Function GetHashCode(obj As SpectrumPeak) As Integer Implements IEqualityComparer(Of SpectrumPeak).GetHashCode
        Return std.Round(obj.mz, 6).GetHashCode()
    End Function
End Class
