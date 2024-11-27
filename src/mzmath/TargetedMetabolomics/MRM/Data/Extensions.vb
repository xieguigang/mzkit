#Region "Microsoft.VisualBasic::81b4bf44c48f9fdc7a1f4dbe44d2fd4f, mzmath\TargetedMetabolomics\MRM\Data\Extensions.vb"

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

    '   Total Lines: 130
    '    Code Lines: 89 (68.46%)
    ' Comment Lines: 25 (19.23%)
    '    - Xml Docs: 96.00%
    ' 
    '   Blank Lines: 16 (12.31%)
    '     File Size: 6.01 KB


    '     Module Extensions
    ' 
    '         Function: GetAllFeatures, (+2 Overloads) MRMSelector, PopulatePeaks
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports chromatogramTicks = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.chromatogram

Namespace MRM.Data

    <HideModuleName>
    Public Module Extensions

        ''' <summary>
        ''' enumerate all of the ion pair features in the given mzML file list.
        ''' </summary>
        ''' <param name="files"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GetAllFeatures(files As IEnumerable(Of String)) As IonPair()
            Dim ions As IonPair() = files _
                .Select(AddressOf mzML.LoadChromatogramList) _
                .IteratesALL _
                .Where(Function(chr) Not chr.id Like NotMRMSelectors) _
                .Select(Function(chr)
                            Dim q1 = chr.precursor.MRMTargetMz
                            Dim q3 = chr.product.MRMTargetMz
                            Dim ms As New LibraryMatrix({
                                New ms2(q1, 1),
                                New ms2(q3, 1)
                            })

                            Return New IonPair With {
                                .precursor = q1,
                                .product = q3,
                                .accession = ms.MsSplashId,
                                .name = $"{q1.ToString("F2")}/{q3.ToString("F2")}"
                            }
                        End Function) _
                .GroupBy(Function(ion)
                             ' Isomerism
                             Return $"{ion.precursor.ToString("F1")}-{ion.product.ToString("F1")}"
                         End Function) _
                .Select(Function(ion) ion.First) _
                .ToArray

            Return ions
        End Function

        <Extension>
        Public Function PopulatePeaks(ionPairs As IonPair(), raw$, tolerance As Tolerance, Optional baselineQuantile# = 0.65) As (ion As IsomerismIonPairs, peak As ROIPeak)()
            Dim args As MRMArguments = MRMArguments.GetDefaultArguments

            args.tolerance = tolerance
            args.baselineQuantile = baselineQuantile

            Dim ionData = LoadChromatogramList(path:=raw) _
                .MRMSelector(IonPair.GetIsomerism(ionPairs, tolerance), tolerance) _
                .Where(Function(ion) Not ion.chromatogram Is Nothing) _
                .Select(Function(ion)
                            Dim mrm As ROIPeak = MRMIonExtract.GetTargetROIPeak(ion.ion.target, ion.chromatogram, args)

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

        ''' <summary>
        ''' Filter a single XIC data object via a given MRM ion pair data 
        ''' </summary>
        ''' <param name="chromatograms"></param>
        ''' <param name="ion"></param>
        ''' <param name="tolerance"></param>
        ''' <returns>
        ''' this function returns nothing if the given ion pair is not found 
        ''' from the givne <paramref name="chromatograms"/> data collection. 
        ''' </returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function MRMSelector(chromatograms As IEnumerable(Of chromatogramTicks),
                                    ion As IonPair,
                                    tolerance As Tolerance) As chromatogramTicks
            Return chromatograms _
                .Where(Function(c)
                           Return (Not c.id Like NotMRMSelectors) AndAlso ion.Assert(c, tolerance)
                       End Function) _
                .FirstOrDefault
        End Function
    End Module
End Namespace
