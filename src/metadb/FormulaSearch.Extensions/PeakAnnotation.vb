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

    Public Iterator Function RunAnnotations(parent#, products As IEnumerable(Of ms2), charge#) As IEnumerable(Of Annotation)
        Dim precursor_types As String() = If(charge > 0, searchTypes("+"), searchTypes("-"))
        Dim formulas = New PrecursorIonSearch(profile) _
            .AddPrecursorTypeRanges(precursor_types) _
            .SearchByPrecursorMz(
                mz:=parent,
                charge:=charge,
                ionMode:=stdNum.Sign(charge)
            ) _
            .Where(Function(a) a.GetAtomCount("C") > 0) _
            .ToArray
        Dim raw As ms2() = products.ToArray

        For Each candidate As PrecursorIonComposition In formulas
            Yield DoCandidateAnnotation(candidate, raw, precursor_types)
        Next
    End Function

    Private Function DoCandidateAnnotation(formula As PrecursorIonComposition, products As ms2(), precursor_types$()) As Annotation
        Dim anno As New Annotation(formula, products)

        Return anno
    End Function
End Class

Public Class Annotation

    Public Property products As ms2()
    Public Property formula As FormulaComposition

    Sub New(formula As FormulaComposition, products As ms2())
        Me.formula = formula
        Me.products = products _
            .Select(Function(a)
                        Return New ms2 With {
                            .mz = a.mz,
                            .intensity = a.intensity
                        }
                    End Function) _
            .ToArray
    End Sub

    Public Overrides Function ToString() As String
        Return formula.ToString
    End Function
End Class