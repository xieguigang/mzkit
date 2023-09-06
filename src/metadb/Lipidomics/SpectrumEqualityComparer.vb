Imports CompMs.Common.Components
Imports System
Imports System.Collections.Generic

Namespace CompMs.Common.Lipidomics
    Friend Class SpectrumEqualityComparer
        Implements IEqualityComparer(Of SpectrumPeak)
        Private Shared ReadOnly EPS As Double = 1e6
        Public Function Equals(ByVal x As SpectrumPeak, ByVal y As SpectrumPeak) As Boolean Implements IEqualityComparer(Of SpectrumPeak).Equals
            Return Math.Abs(x.Mass - y.Mass) <= EPS
        End Function

        Public Function GetHashCode(ByVal obj As SpectrumPeak) As Integer Implements IEqualityComparer(Of SpectrumPeak).GetHashCode
            Return Math.Round(obj.Mass, 6).GetHashCode()
        End Function
    End Class
End Namespace
