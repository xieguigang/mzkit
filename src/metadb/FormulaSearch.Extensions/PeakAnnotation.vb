#Region "Microsoft.VisualBasic::4926bef7c7024c1bcba3eb1137544512, src\metadb\FormulaSearch.Extensions\PeakAnnotation.vb"

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

    ' Class PeakAnnotation
    ' 
    '     Function: MatchElementGroups, MeasureFormula, MeasureIsotopePeaks, RunAnnotation
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports stdNum = System.Math

Public Class PeakAnnotation

    ''' <summary>
    ''' Run annotation of the ms2 product ions
    ''' </summary>
    ''' <param name="parentMz"></param>
    ''' <param name="products"></param>
    ''' <returns></returns>
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

            If FormulaSearch.PPM(products(i).mz, parentMz) <= 30 Then
                products(i).Annotation = "M"
            Else
                For isotope As Integer = -3 To 3
                    If stdNum.Abs(isotope - delta) <= 0.05 Then
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
