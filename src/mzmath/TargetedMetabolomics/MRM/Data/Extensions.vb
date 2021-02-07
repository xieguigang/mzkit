#Region "Microsoft.VisualBasic::104b672764f28979871657fd6f3bff0f, TargetedMetabolomics\MRM\Data\Extensions.vb"

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

    '     Module Extensions
    ' 
    '         Function: GetAllFeatures, MRMSelector, PopulatePeaks
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Scripting
Imports chromatogramTicks = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.chromatogram

Namespace MRM.Data

    <HideModuleName>
    Public Module Extensions

        <Extension>
        Public Function GetAllFeatures(files As IEnumerable(Of String)) As IonPair()
            Dim ions As IonPair() = files _
                .Select(AddressOf mzML.LoadChromatogramList) _
                .IteratesALL _
                .Where(Function(chr) Not chr.id Like NotMRMSelectors) _
                .Select(Function(chr)
                            Return New IonPair With {
                                .precursor = chr.precursor.MRMTargetMz,
                                .product = chr.product.MRMTargetMz
                            }
                        End Function) _
                .GroupBy(Function(ion)
                             Return $"{ion.precursor.ToString("F1")}-{ion.product.ToString("F1")}"
                         End Function) _
                .Select(Function(ion) ion.First) _
                .ToArray

            Return ions
        End Function

        <Extension>
        Public Function PopulatePeaks(ionPairs As IonPair(), raw$, tolerance As Tolerance, Optional baselineQuantile# = 0.65) As (ion As IsomerismIonPairs, peak As ROIPeak)()
            Dim ionData = LoadChromatogramList(path:=raw) _
                .MRMSelector(IonPair.GetIsomerism(ionPairs, tolerance), tolerance) _
                .Where(Function(ion) Not ion.chromatogram Is Nothing) _
                .Select(Function(ion)
                            Dim vector As IVector(Of ChromatogramTick) = ion.chromatogram.Ticks.Shadows
                            Dim peak = vector.MRMPeak(baselineQuantile:=baselineQuantile)
                            Dim peakTicks = vector.PickArea(range:=peak)
                            Dim mrm As New ROIPeak With {
                                .window = peak,
                                .base = vector.Baseline(baselineQuantile),
                                .ticks = peakTicks,
                                .peakHeight = Aggregate t In .ticks Into Max(t.Intensity)
                            }

                            Return (ion.ion, mrm)
                        End Function) _
                .ToArray

            Return ionData
        End Function

        ''' <summary>
        ''' BPC, TIC, etc
        ''' </summary>
        ReadOnly NotMRMSelectors As Index(Of String) = {"BPC", "TIC"}

        ''' <summary>
        ''' MRM ion selector based on the precursor ion m/z and the product ion m/z value.
        ''' </summary>
        ''' <param name="chromatograms"></param>
        ''' <param name="ionPairs"></param>
        ''' <returns>Nothing for ion not found</returns>
        <Extension>
        Public Function MRMSelector(chromatograms As IEnumerable(Of chromatogramTicks),
                                    ionPairs As IEnumerable(Of IsomerismIonPairs),
                                    tolerance As Tolerance) As IEnumerable(Of (ion As IsomerismIonPairs, chromatogram As chromatogramTicks))

            With chromatograms.ToArray
                Return ionPairs _
                    .Select(Function(ion)
                                Dim chromatogram =
                                    .Where(Function(c)
                                               Return (Not c.id Like NotMRMSelectors) AndAlso ion.target.Assert(c, tolerance)
                                           End Function) _
                                    .FirstOrDefault

                                If chromatogram Is Nothing Then
                                    Call $"missing {ion.ToString}, please consider check your ion pair data or increase the m/z tolerance value...".Warning
                                End If

                                Return (ion, chromatogram)
                            End Function)
            End With
        End Function
    End Module
End Namespace
