#Region "Microsoft.VisualBasic::0a936a21e23a9a24c205ac848cfa51a4, metadb\FormulaSearch.Extensions\PeakAnnotation.vb"

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

    '   Total Lines: 142
    '    Code Lines: 95 (66.90%)
    ' Comment Lines: 34 (23.94%)
    '    - Xml Docs: 91.18%
    ' 
    '   Blank Lines: 13 (9.15%)
    '     File Size: 5.61 KB


    ' Class PeakAnnotation
    ' 
    '     Properties: formula, precursor_matched, products
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: DoPeakAnnotation, GetAnnotatedPeaks, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports BioNovoGene.BioDeep.MSFinder
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

''' <summary>
''' Do formula search and peak annotation result
''' </summary>
Public Class PeakAnnotation

    ''' <summary>
    ''' the product peak annotation result dataset, this array contains all spectrum peaks, includes
    ''' peak has been annotated or peaks has no annotation data.
    ''' </summary>
    ''' <returns></returns>
    Public Property products As ms2()
    ''' <summary>
    ''' the target metabolite formula source data, this could be parsed from
    ''' the database of the know metabolite or the de-novo formula prediction
    ''' result based on the algorithm 
    ''' </summary>
    ''' <returns></returns>
    Public Property formula As FormulaComposition
    Public Property precursor_matched As Boolean = False

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(formula As FormulaComposition, products As IEnumerable(Of ProductIon))
        Call Me.New(
            formula:=formula,
            products:=products _
                .Select(Function(i)
                            Return New ms2 With {
                                .mz = i.Mass,
                                .intensity = i.Intensity,
                                .Annotation = i.Name
                            }
                        End Function)
        )
    End Sub

    Sub New(formula As FormulaComposition, products As IEnumerable(Of ms2))
        Me.formula = formula
        Me.products = products _
            .Select(Function(a)
                        Return New ms2 With {
                            .mz = a.mz,
                            .intensity = a.intensity,
                            .Annotation = If(a.Annotation.IsInteger, "", a.Annotation)
                        }
                    End Function) _
            .ToArray
    End Sub

    ''' <summary>
    ''' Filter and get a peak list that with <see cref="ms2.Annotation"/> not empty.
    ''' </summary>
    ''' <returns>
    ''' a collection of the peaks data with annotation data
    ''' </returns>
    Public Function GetAnnotatedPeaks() As ms2()
        Return products _
            .Where(Function(i) Not i.Annotation.StringEmpty(, True)) _
            .ToArray
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return formula.ToString
    End Function

    ''' <summary>
    ''' do score evaluation of the spectrum associated with the assembled formula composition 
    ''' </summary>
    ''' <param name="peaks"></param>
    ''' <param name="adduct"></param>
    ''' <param name="formula"></param>
    ''' <param name="da">
    ''' the mass tolerance error for matches the ms2 spectrum peaks, 
    ''' usually be the tolerance error for make spectrum centroid 
    ''' process
    ''' </param>
    ''' <returns></returns>
    Public Shared Function DoPeakAnnotation(peaks As ISpectrum, adduct As MzCalculator, formula As Formula,
                                            Optional da As Double = 0.1) As PeakAnnotation

        Dim assign As New FragmentAssigner(da)
        Dim exactMass As Double = formula.ExactMass
        Dim precursorMz As Double = adduct.CalcMZ(exactMass)
        Dim precursorHit As Boolean = False
        Dim peaksData As SpectrumPeak() = peaks.GetIons _
            .Select(Function(m)
                        If std.Abs(m.mz - exactMass) <= da Then
                            m.Annotation = "M"
                            precursorHit = True
                        ElseIf std.Abs(m.mz - precursorMz) <= da Then
                            m.Annotation = adduct.ToString
                            precursorHit = True
                        End If

                        Return New SpectrumPeak(m)
                    End Function) _
            .ToArray
        Dim adductInfo As New AdductIon(adduct)
        Dim result = assign.FastFragmnetAssigner(peaksData.AsList, formula, adductInfo)
        Dim fcom As New FormulaComposition(formula.CountsByElement, formula.ToString) With {
            .charge = adduct.charge,
            .massdiff = 0,
            .ppm = 0
        }

        ' 2024-08-29
        ' make union of the annotated products and
        ' un-annotated fragments
        Dim union As ms2() = result _
            .Select(Function(i)
                        Return New ms2 With {
                            .mz = i.Mass,
                            .intensity = i.Intensity,
                            .Annotation = i.Name
                        }
                    End Function) _
            .JoinIterates(peaks.GetIons) _
            .ToArray _
            .Centroid(Tolerance.DeltaMass(0.1), New RelativeIntensityCutoff(0.01)) _
            .ToArray

        Return New PeakAnnotation(fcom, union) With {
            .precursor_matched = precursorHit
        }
    End Function

    ''' <summary>
    ''' get the spectrum peak annotation result outputs
    ''' </summary>
    ''' <param name="pa"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Narrowing Operator CType(pa As PeakAnnotation) As ms2()
        Return pa.products
    End Operator

End Class
