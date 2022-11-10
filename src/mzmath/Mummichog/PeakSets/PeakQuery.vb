Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq

Public Class PeakQuery

    Public Property exactMass As Double
    Public Property peaks As Peaktable()

    Public ReadOnly Property size As Integer
        Get
            Return peaks.TryCount
        End Get
    End Property

    Public ReadOnly Property adducts As String()
        Get
            Return peaks _
                .Select(Function(p) p.annotation) _
                .Distinct _
                .ToArray
        End Get
    End Property

    Public ReadOnly Property id_group As String()
        Get
            Return peaks _
                .Select(Function(p) p.name) _
                .ToArray
        End Get
    End Property

    Public ReadOnly Property rt As DoubleRange
        Get
            Return New DoubleRange(From peak As Peaktable
                                   In peaks
                                   Let t = peak.rt
                                   Select t)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{exactMass.ToString("F4")}@{rt}s has {peaks.Length} peaks: {adducts.JoinBy("; ")}"
    End Function

End Class