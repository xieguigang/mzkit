Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports stdNum = System.Math

Public Class PeakAnnotation

    Public Function RunAnnotation(parentMz#, products As ms2()) As Annotation
        products = MeasureIsotopePeaks(parentMz, products)
        products = MatchElementGroups(parentMz, products)

        Return New Annotation(MeasureFormula(parentMz, products), products)
    End Function

    Private Function MeasureFormula(parentMz#, products As ms2()) As FormulaComposition
        Dim counts As New Dictionary(Of String, Integer)

        Return New FormulaComposition(counts)
    End Function

    Private Shared Function MeasureIsotopePeaks(parentMz#, products As ms2()) As ms2()
        Dim delta As Double

        For i As Integer = 0 To products.Length - 1
            delta = (products(i).mz - parentMz) / Element.H

            If FormulaSearch.PPM(products(i).mz, parentMz) <= 20 Then
                products(i).Annotation = "M"
            Else
                For isotope As Integer = -3 To 3
                    If stdNum.Abs(isotope - delta) <= 0.01 Then
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

    Private Shared Function MatchElementGroups(parentMz#, products As ms2()) As ms2()
        Dim group As NamedValue(Of Formula)
        Dim delta As Integer = 0

        For Each element As ms2 In products
            group = AtomGroupHandler.GetByMass(element.mz)

            If Not group.IsEmpty Then
                If element.Annotation.StringEmpty Then
                    element.Annotation = $"[{group.Value.EmpiricalFormula}]{group.Name}"
                Else
                    element.Annotation = $"{element.Annotation} ([{group.Value.EmpiricalFormula}]{group.Name})"
                End If
            Else
                group = AtomGroupHandler.FindDelta(parentMz, element.mz, delta)

                If Not group.IsEmpty Then
                    Dim deltaStr As String

                    If delta = 1 Then
                        deltaStr = $"[M+{group.Name}]"
                    Else
                        deltaStr = $"[M-{group.Name}]"
                    End If

                    If element.Annotation.StringEmpty Then
                        element.Annotation = deltaStr
                    Else
                        element.Annotation = $"{element.Annotation} ({deltaStr})"
                    End If
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
                            .intensity = a.intensity,
                            .Annotation = If(a.Annotation.IsInteger, "", a.Annotation),
                            .quantity = a.quantity
                        }
                    End Function) _
            .ToArray
    End Sub

    Public Overrides Function ToString() As String
        Return formula.ToString
    End Function
End Class