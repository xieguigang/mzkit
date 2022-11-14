Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Public Class PeakCorrelation

    ReadOnly adducts As MzCalculator()
    ReadOnly ms1_isotopic As IsotopicAnnotation(Of Peaktable)
    ReadOnly ms1_adducts As AdductsAnnotation(Of Peaktable)
    ReadOnly ms2_isotopic As IsotopicAnnotation(Of PeakMs2)
    ReadOnly ms2_adducts As AdductsAnnotation(Of PeakMs2)

    Sub New(precursors As IEnumerable(Of MzCalculator), Optional isotopicMax As Integer = 5)
        Me.adducts = precursors.ToArray
        Me.ms1_isotopic = New IsotopicAnnotation(Of Peaktable)(
            max:=isotopicMax,
            activator:=AddressOf peakMs1Activator
        )
        Me.ms1_adducts = New AdductsAnnotation(Of Peaktable)(
            adducts:=Me.adducts,
            activator:=AddressOf peakMs1Activator
        )
        Me.ms2_isotopic = New IsotopicAnnotation(Of PeakMs2)(
            max:=isotopicMax,
            activator:=AddressOf peakMs2Activator
        )
        Me.ms2_adducts = New AdductsAnnotation(Of PeakMs2)(
            adducts:=Me.adducts,
            activator:=AddressOf peakMs2Activator
        )
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function FindExactMass(peaks As PeakMs2(),
                                  Optional deltaRt As Double = 6,
                                  Optional mzdiff As Double = 0.6) As IEnumerable(Of PeakQuery(Of PeakMs2))

        Return FindExactMass(peaks, ms2_isotopic, ms2_adducts, deltaRt, mzdiff)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Shared Function peakMs2Activator(peak As PeakMs2, anno As String) As PeakMs2
        Return New PeakMs2 With {
            .activation = peak.activation,
            .collisionEnergy = peak.collisionEnergy,
            .file = peak.file,
            .intensity = peak.intensity,
            .lib_guid = peak.lib_guid,
            .meta = New Dictionary(Of String, String)(peak.meta),
            .mz = peak.mz,
            .mzInto = peak.mzInto.ToArray,
            .precursor_type = anno,
            .rt = peak.rt,
            .scan = peak.scan
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Shared Function peakMs1Activator(peak As Peaktable, anno As String) As Peaktable
        Return New Peaktable With {
            .annotation = anno,
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
    End Function

    Public Iterator Function FindExactMass(Of T As IMS1Annotation)(PeakList As T(),
                                                                   isotopic As IsotopicAnnotation(Of T),
                                                                   adductAnno As AdductsAnnotation(Of T),
                                                                   Optional deltaRt As Double = 3,
                                                                   Optional mzdiff As Double = 0.01) As IEnumerable(Of IMS1Annotation)

        Dim isotopics As List(Of PeakQuery(Of T)) = PeakList _
            .AsParallel _
            .Select(Function(peak) isotopic.IsotopicAnnotation(peak)) _
            .IteratesALL _
            .AsList
        Dim adducts As PeakQuery(Of T)() = PeakList _
            .AsParallel _
            .Select(Function(peak) adductAnno.AdductsAnnotation(peak)) _
            .IteratesALL _
            .ToArray
        Dim union = isotopics + adducts
        Dim mass_group = union _
            .GroupBy(Function(g) g.exactMass, offsets:=mzdiff) _
            .ToArray

        For Each mass As NamedCollection(Of PeakQuery(Of T)) In mass_group
            Dim time_group = mass _
                .GroupBy(Function(g) g.peaks(Scan0).rt, offsets:=deltaRt) _
                .ToArray

            For Each peak As NamedCollection(Of PeakQuery(Of T)) In time_group
                Yield New PeakQuery(Of T) With {
                    .exactMass = Val(mass.name),
                    .peaks = peak _
                        .Select(Function(a) a.peaks) _
                        .IteratesALL _
                        .ToArray
                }
            Next
        Next
    End Function

    ''' <summary>
    ''' find exact mass group
    ''' </summary>
    ''' <param name="peaktable"></param>
    ''' <param name="deltaRt"></param>
    ''' <param name="mzdiff"></param>
    ''' <returns></returns>
    Public Function FindExactMass(peaktable As IEnumerable(Of Peaktable),
                                  Optional deltaRt As Double = 3,
                                  Optional mzdiff As Double = 0.01) As IEnumerable(Of PeakQuery(Of Peaktable))

        Dim peakList As Peaktable() = (From d As Peaktable
                                       In peaktable
                                       Where d.mz > 0
                                       Order By d.mz).ToArray

        Return FindExactMass(peakList, ms1_isotopic, ms1_adducts, deltaRt, mzdiff)
    End Function
End Class

Public Class AdductsAnnotation(Of T As IMS1Annotation)

    ReadOnly adducts As MzCalculator()
    ReadOnly activator As Func(Of T, String, T)

    Sub New(adducts As MzCalculator(), activator As Func(Of T, String, T))
        Me.adducts = adducts
        Me.adducts = adducts
    End Sub

    Public Iterator Function AdductsAnnotation(peak As T) As IEnumerable(Of PeakQuery(Of T))
        For Each adduct As MzCalculator In adducts
            Dim exact_mass As Double = adduct.CalcMass(precursorMZ:=peak.mz)
            Dim q As New PeakQuery(Of T) With {
                .exactMass = exact_mass,
                .peaks = {activator(peak, adduct.ToString)}
            }

            Yield q
        Next
    End Function

End Class

Public Class IsotopicAnnotation(Of T As IMS1Annotation)

    ReadOnly activator As Func(Of T, String, T)
    ReadOnly max As Integer

    Sub New(max As Integer, activator As Func(Of T, String, T))
        Me.max = max
        Me.activator = activator
    End Sub

    Public Iterator Function IsotopicAnnotation(peak As T) As IEnumerable(Of PeakQuery(Of T))
        For i As Integer = 1 To max
            Dim exact_mass As Double = peak.mz - Element.H * i
            Dim q As New PeakQuery(Of T) With {
                .exactMass = exact_mass,
                .peaks = {activator(peak, $"isotopic[M+{i}]")}
            }

            Yield q
        Next
    End Function
End Class