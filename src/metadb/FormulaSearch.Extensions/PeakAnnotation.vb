Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports stdNum = System.Math

Public Class PeakAnnotation

    Public Function RunAnnotation(products As ms2()) As Annotation
        Dim isotope As ms2() = MeasureIsotopePeaks(products)

    End Function

    Private Shared Function MeasureIsotopePeaks(products As ms2()) As ms2()
        Dim desc = products.OrderByDescending(Function(mz) mz.mz).ToArray
        Dim isotope As Integer() = New Integer(desc.Length - 1) {}
        Dim max As ms2 = desc(0)

        For i As Integer = 1 To desc.Length - 1
            If desc(i) - max <= 0.0001 Then
                isotope(i - 1) += 1
            End If
        Next

        Call Array.Reverse(isotope)
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