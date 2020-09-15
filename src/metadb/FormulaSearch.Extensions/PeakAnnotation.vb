Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports stdNum = System.Math

Public Class PeakAnnotation

    ReadOnly searchTypes As Dictionary(Of String, String())
    ReadOnly profile As SearchOption

    Sub New()
        searchTypes = New Dictionary(Of String, String()) From {
            {"+", {"M", "M+H"}},
            {"-", {"M", "M-H"}}
        }
        profile = SearchOption.SmallMolecule(DNPOrWileyType.DNP, common:=True).AdjustPpm(30)
    End Sub

    Public Iterator Function RunAnnotations(parent As Double, products As IEnumerable(Of ms2), charge As Double) As IEnumerable(Of Annotation)
        Dim precursor_types As String() = If(charge > 0, searchTypes("+"), searchTypes("-"))
        Dim formulas = New PrecursorIonSearch(profile) _
            .AddPrecursorTypeRanges(precursor_types) _
            .SearchByPrecursorMz(
                mz:=parent,
                charge:=charge,
                ionMode:=stdNum.Sign(charge)
            ) _
            .ToArray

        For Each candidate As PrecursorIonComposition In formulas
            Yield DoCandidateAnnotation(candidate, precursor_types)
        Next
    End Function

    Private Function DoCandidateAnnotation(formula As PrecursorIonComposition, precursor_types As String()) As Annotation

    End Function
End Class

Public Class Annotation

End Class