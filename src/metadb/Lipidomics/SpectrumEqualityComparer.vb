Imports CompMs.Common.Components
Imports System
Imports System.Collections.Generic

Friend Class SpectrumEqualityComparer
    Implements IEqualityComparer(Of SpectrumPeak)
    Private Shared ReadOnly EPS As Double = 1000000.0
    Public Function Equals(x As SpectrumPeak, y As SpectrumPeak) As Boolean Implements IEqualityComparer(Of SpectrumPeak).Equals
        Return Math.Abs(x.Mass - y.Mass) <= EPS
    End Function

    Public Function GetHashCode(obj As SpectrumPeak) As Integer Implements IEqualityComparer(Of SpectrumPeak).GetHashCode
        Return Math.Round(obj.Mass, 6).GetHashCode()
    End Function
End Class
