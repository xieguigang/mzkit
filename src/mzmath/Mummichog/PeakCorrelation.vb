Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Public Class PeakCorrelation

    ReadOnly adducts As MzCalculator()
    ReadOnly isotopicMax As Integer

    Sub New(precursors As IEnumerable(Of MzCalculator), Optional isotopicMax As Integer = 5)
        Me.adducts = precursors.ToArray
        Me.isotopicMax = isotopicMax
    End Sub

    ''' <summary>
    ''' find exact mass group
    ''' </summary>
    ''' <param name="peaktable"></param>
    ''' <param name="deltaRt"></param>
    ''' <param name="mzdiff"></param>
    ''' <returns></returns>
    Public Iterator Function FindExactMass(peaktable As IEnumerable(Of Peaktable),
                                           Optional deltaRt As Double = 3,
                                           Optional mzdiff As Double = 0.01) As IEnumerable(Of PeakQuery)

        Dim peakList As Peaktable() = (From d As Peaktable
                                       In peaktable
                                       Where d.mz > 0
                                       Order By d.mz).ToArray
        Dim isotopics As List(Of PeakQuery) = peakList _
            .AsParallel _
            .Select(Function(peak) IsotopicAnnotation(peak, max:=isotopicMax)) _
            .IteratesALL _
            .AsList
        Dim adducts As PeakQuery() = peakList _
            .AsParallel _
            .Select(Function(peak) AdductsAnnotation(peak, Me.adducts)) _
            .IteratesALL _
            .ToArray
        Dim union = isotopics + adducts
        Dim mass_group = union _
            .GroupBy(Function(t) t.exactMass, offsets:=mzdiff) _
            .ToArray

        For Each mass As NamedCollection(Of PeakQuery) In mass_group
            Dim time_group = mass _
                .GroupBy(Function(t) t.peaks(Scan0).rt, offsets:=deltaRt) _
                .ToArray

            For Each peak As NamedCollection(Of PeakQuery) In time_group
                Yield New PeakQuery With {
                    .exactMass = Val(mass.name),
                    .peaks = peak _
                        .Select(Function(a) a.peaks) _
                        .IteratesALL _
                        .ToArray
                }
            Next
        Next
    End Function

    Public Shared Iterator Function AdductsAnnotation(peak As Peaktable, adducts As MzCalculator()) As IEnumerable(Of PeakQuery)
        For Each adduct As MzCalculator In adducts
            Dim exact_mass As Double = adduct.CalcMass(precursorMZ:=peak.mz)
            Dim q As New PeakQuery With {
                .exactMass = exact_mass,
                .peaks = {
                    New Peaktable With {
                        .annotation = adduct.ToString,
                        .energy = peak.energy,
                        .index = peak.index,
                        .intb = peak.intb,
                        .into = peak.into,
                        .ionization = peak.ionization,
                        .maxo = peak.maxo,
                        .mz = peak.mz,
                        .mzmax = peak.mzmax,
                        .mzmin = peak.mzmin,
                        .name = peak.name,
                        .rt = peak.rt,
                        .rtmax = peak.rtmax,
                        .rtmin = peak.rtmin,
                        .sample = peak.sample,
                        .scan = peak.scan,
                        .sn = peak.sn
                    }
                }
            }

            Yield q
        Next
    End Function

    Public Shared Iterator Function IsotopicAnnotation(peak As Peaktable, max As Integer) As IEnumerable(Of PeakQuery)
        For i As Integer = 1 To max
            Dim exact_mass As Double = peak.mz - Element.H * i
            Dim q As New PeakQuery With {
                .exactMass = exact_mass,
                .peaks = {
                    New Peaktable With {
                        .annotation = $"isotopic[M+{i}]",
                        .energy = peak.energy,
                        .index = peak.index,
                        .intb = peak.intb,
                        .into = peak.into,
                        .ionization = peak.ionization,
                        .maxo = peak.maxo,
                        .mz = peak.mz,
                        .mzmax = peak.mzmax,
                        .mzmin = peak.mzmin,
                        .name = peak.name,
                        .rt = peak.rt,
                        .rtmax = peak.rtmax,
                        .rtmin = peak.rtmin,
                        .sample = peak.sample,
                        .scan = peak.scan,
                        .sn = peak.sn
                    }
                }
            }

            Yield q
        Next
    End Function

End Class

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