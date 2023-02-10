#Region "Microsoft.VisualBasic::a1eb5507f8b0b0579b6648b25be9e7ea, mzkit\src\metadb\FormulaSearch.Extensions\PeakAnnotation.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 201
'    Code Lines: 140
' Comment Lines: 31
'   Blank Lines: 30
'     File Size: 6.58 KB


' Class PeakAnnotation
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: MatchElementGroups, MeasureFormula, (+2 Overloads) MeasureIsotopePeaks, MeasureProductIsotopePeaks, RunAnnotation
'               UnionPeak
' 
'     Sub: FragmentAnnotation
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.AtomGroups
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports stdNum = System.Math

Public Class PeakAnnotation

    ReadOnly massDelta As Double
    ReadOnly isotopeFirst As Boolean = True
    ReadOnly adducts As MzCalculator()

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="massDelta">
    ''' mass tolerance error in delta dalton
    ''' </param>
    Sub New(massDelta As Double, isotopeFirst As Boolean, Optional adducts As MzCalculator() = Nothing)
        Me.massDelta = massDelta
        Me.isotopeFirst = isotopeFirst
        Me.adducts = adducts
    End Sub

    ''' <summary>
    ''' Run annotation of the ms2 product ions
    ''' </summary>
    ''' <param name="parentMz"></param>
    ''' <param name="products"></param>
    ''' <param name="formula">
    ''' Apply for atom group validation, used for create machine learning dataset
    ''' </param>
    ''' <returns></returns>
    Public Function RunAnnotation(parentMz#, products As ms2(), Optional formula As String = Nothing) As Annotation
        Dim parent As New ParentValue(parentMz, formula)

        products = MeasureIsotopePeaks(parentMz, products)
        products = MatchElementGroups(parent, products)
        products = MeasureProductIsotopePeaks(products)
        products = (From mzi As ms2
                    In products
                    Group By mzi.Annotation Into Group
                    Let peak = UnionPeak(Annotation, Group.ToArray)
                    Select peak
                    Order By peak.mz).ToArray

        Return New Annotation(MeasureFormula(parentMz, products), products)
    End Function

    ''' <summary>
    ''' union the duplicated peak fragments
    ''' </summary>
    ''' <param name="name"></param>
    ''' <param name="group"></param>
    ''' <returns></returns>
    Private Shared Function UnionPeak(name As String, group As ms2()) As ms2
        If group.Length = 1 Then
            Return group(Scan0)
        Else
            Return New ms2 With {
                .Annotation = name,
                .mz = group _
                    .OrderByDescending(Function(i) i.intensity) _
                    .First _
                    .mz,
                .intensity = group _
                    .Select(Function(i) i.intensity) _
                    .Average
            }
        End If
    End Function

    Private Function MeasureProductIsotopePeaks(products As ms2()) As ms2()
        products = products _
            .OrderByDescending(Function(i) i.mz) _
            .ToArray

        For i As Integer = 0 To products.Length - 2
            Dim large As Double = products(i).mz
            Dim small As Double = products(i + 1).mz

            ' 20220501
            '
            '   parent + adduct = product
            '     |        |        |
            '   small[isotopic] = large
            '
            Dim label As String = MeasureIsotopePeaks(parentMz:=small, product:=large)

            If Not label Is Nothing Then
                If products(i + 1).Annotation.StringEmpty Then
                    label = $"{products(i + 1).mz.ToString("F3")} {label}"
                Else
                    label = $"{products(i + 1).Annotation} {label}"
                End If

                If products(i).Annotation.StringEmpty Then
                    products(i).Annotation = label
                ElseIf isotopeFirst Then
                    products(i).Annotation = label
                End If
            End If
        Next

        Return products
    End Function

    ''' <summary>
    ''' assemble chemical formula from the ms2 
    ''' feature annotation result.
    ''' </summary>
    ''' <param name="parentMz#"></param>
    ''' <param name="products"></param>
    ''' <returns></returns>
    Private Function MeasureFormula(parentMz#, products As ms2()) As FormulaComposition
        Dim counts As New Dictionary(Of String, Integer)

        Return New FormulaComposition(counts)
    End Function

    Private Function MeasureIsotopePeaks(parentMz#, products As ms2()) As ms2()
        For i As Integer = 0 To products.Length - 1
            Dim tag As String = MeasureIsotopePeaks(parentMz, products(i).mz)

            If Not tag Is Nothing Then
                If products(i).Annotation.StringEmpty Then
                    products(i).Annotation = tag
                ElseIf isotopeFirst Then
                    products(i).Annotation = tag
                End If
            End If
        Next

        Return products
    End Function

    Private Function MeasureIsotopePeaks(parentMz#, product As Double) As String
        Dim delta As Double = (product - parentMz) / Element.H

        If FormulaSearch.PPM(product, parentMz) <= 30 Then
            Return "M"
        End If

        For isotope As Integer = -3 To 3
            If isotope = 0 Then
                Continue For
            End If

            If stdNum.Abs(isotope - delta) <= 0.05 Then
                If isotope < 0 Then
                    Return $"[M{isotope}]"
                Else
                    Return $"[M+{isotope}]isotope"
                End If

                Exit For
            End If
        Next

        Return Nothing
    End Function

    Private Function MatchElementGroups(parent As ParentValue, products As ms2()) As ms2()
        For Each element As ms2 In products
            Call FragmentAnnotation(element, parent)
        Next

        Return products
    End Function

    Private Sub FragmentAnnotation(element As ms2, parent As ParentValue)
        Dim group As FragmentAnnotationHolder
        Dim delta As Integer = 0
        Dim q As New AtomGroupQuery(element.mz, massDelta, adducts)

        If parent.formula Is Nothing Then
            group = q.GetByMass
        Else
            group = parent.GetFragment(q.FilterByMass)
        End If

        If Not group Is Nothing Then
            If element.Annotation.StringEmpty Then
                element.Annotation = group.name
            Else
                element.Annotation = $"{element.Annotation} ({group.name})"
            End If
        Else
            group = AtomGroupHandler.FindDelta(
                mz1:=parent.parentMz,
                mz2:=element.mz,
                delta:=delta,
                da:=massDelta,
                adducts:=adducts
            )

            If Not group Is Nothing Then
                Dim deltaStr As String

                If delta = -1 Then
                    deltaStr = $"[M+{group.name}]"
                Else
                    deltaStr = $"[M-{group.name}]"
                End If

                If element.Annotation.StringEmpty Then
                    element.Annotation = deltaStr
                Else
                    element.Annotation = $"{element.Annotation} ({deltaStr})"
                End If
            End If
        End If
    End Sub
End Class
