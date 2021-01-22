Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports stdNum = System.Math

Public Class PeakAnnotation

    Public Function RunAnnotation(parentMz#, products As ms2()) As Annotation
        Dim isotope As ms2() = MeasureIsotopePeaks(parentMz, products)

        isotope = MatchElementGroups(isotope)

    End Function

    Private Shared Function MeasureIsotopePeaks(parentMz#, products As ms2()) As ms2()
        Dim delta As Double

        For i As Integer = 0 To products.Length - 1
            delta = (products(i).mz - parentMz) / Element.H

            If stdNum.Abs(delta) <= 0.00001 Then
                products(i).Annotation = "M"
            Else
                For isotope As Integer = -3 To 3
                    If stdNum.Abs(isotope - delta) <= 0.00001 Then
                        If isotope < 0 Then
                            products(i).Annotation = $"[M{isotope}]"
                        Else
                            products(i).Annotation = $"[M+{isotope}]isotope"
                        End If

                        Exit For
                    End If
                Next
            End If
        Next

        Return products
    End Function

    Private Shared Function MatchElementGroups(products As ms2()) As ms2()
        Dim group As NamedValue(Of Formula)

        For Each element As ms2 In products
            group = AtomGroupHandler.GetByMass(element.mz)

            If Not group.IsEmpty Then
                If element.Annotation.StringEmpty Then
                    element.Annotation = $"[{group.Value.EmpiricalFormula}]{group.Name}"
                Else
                    element.Annotation = $"{element.Annotation} ([{group.Value.EmpiricalFormula}]{group.Name})"
                End If
            End If
        Next

        Return products
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