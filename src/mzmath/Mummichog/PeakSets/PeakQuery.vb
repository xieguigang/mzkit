Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq

Public Class PeakQuery(Of T As IMS1Annotation)

    Public Property exactMass As Double
    Public Property peaks As T()

    Public ReadOnly Property size As Integer
        Get
            Return peaks.TryCount
        End Get
    End Property

    Public ReadOnly Property adducts As String()
        Get
            Return peaks _
                .Select(Function(p) p.precursor_type) _
                .Distinct _
                .ToArray
        End Get
    End Property

    Public ReadOnly Property id_group As String()
        Get
            Return peaks _
                .Select(Function(p) p.id) _
                .ToArray
        End Get
    End Property

    ''' <summary>
    ''' returns the rt range in [rtmin, rtmax]
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property rt As DoubleRange
        Get
            Return New DoubleRange(From peak As T
                                   In peaks
                                   Let t = peak.rt
                                   Select t)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{exactMass.ToString("F4")}@{rt}s has {peaks.Length} peaks: {adducts.JoinBy("; ")}"
    End Function

End Class