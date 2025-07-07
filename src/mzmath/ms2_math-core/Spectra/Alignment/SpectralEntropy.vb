﻿#Region "Microsoft.VisualBasic::9c312c8495db2126fc36a8db54b2ebbf, mzmath\ms2_math-core\Spectra\Alignment\SpectralEntropy.vb"

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

    '   Total Lines: 192
    '    Code Lines: 125 (65.10%)
    ' Comment Lines: 37 (19.27%)
    '    - Xml Docs: 89.19%
    ' 
    '   Blank Lines: 30 (15.62%)
    '     File Size: 7.82 KB


    '     Module SpectralEntropy
    ' 
    '         Function: _entropy_similarity, _get_entropy_and_weighted_intensity, (+3 Overloads) calculate_entropy_similarity, Entropy, entropy_distance
    '                   StandardizeAlignment, (+2 Overloads) StandardizeSpectrum, unweighted_entropy_distance, WeightIntensityByEntropy
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.Math.Information
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports std = System.Math

Namespace Spectra

    ''' <summary>
    ''' The similarity score for spectral comparison
    ''' 
    ''' https://github.com/YuanyueLi/SpectralEntropy/blob/master/spectral_entropy/math_distance.py
    ''' </summary>
    Public Module SpectralEntropy

        ''' <summary>
        ''' Unweighted entropy distance:
        ''' 
        ''' ```
        ''' -\frac{2\times S_{PQ}-S_P-S_Q} {ln(4)}, S_I=\sum_{i} {I_i ln(I_i)}
        ''' ```
        ''' </summary>
        ''' <param name="p"></param>
        ''' <param name="q"></param>
        ''' <returns></returns>
        Public Function unweighted_entropy_distance(p As Vector, q As Vector) As Double
            Dim merged As Vector = p + q
            Dim entropy_increase = 2 * merged.ShannonEntropy - p.ShannonEntropy - q.ShannonEntropy

            Return entropy_increase
        End Function

        ''' <summary>
        ''' Entropy distance:
        ''' 
        ''' ```
        ''' -\frac{2\times S_{PQ}^{'}-S_P^{'}-S_Q^{'}} {ln(4)}, S_I^{'}=\sum_{i} {I_i^{'} ln(I_i^{'})}, I^{'}=I^{w}, with\ w=0.25+S\times 0.5\ (S&lt;1.5)
        ''' ```
        ''' </summary>
        ''' <param name="p"></param>
        ''' <param name="q"></param>
        ''' <returns></returns>
        Public Function entropy_distance(p As Vector, q As Vector) As Double
            p = WeightIntensityByEntropy(p)
            q = WeightIntensityByEntropy(q)

            Return unweighted_entropy_distance(p, q)
        End Function

        Public Function WeightIntensityByEntropy(x As Vector,
                                                 Optional WEIGHT_START As Double = 0.25,
                                                 Optional ENTROPY_CUTOFF As Double = 3) As Vector

            Dim weight_slope = (1 - WEIGHT_START) / ENTROPY_CUTOFF

            If x.Sum > 0 Then
                Dim entropy_x = x.ShannonEntropy

                If entropy_x < ENTROPY_CUTOFF Then
                    Dim weight = WEIGHT_START + weight_slope * entropy_x
                    x = x ^ weight
                    Dim x_sum = x.Sum
                    x = x / x_sum
                End If
            End If

            Return x
        End Function

        ''' <summary>
        ''' 因为计算熵需要概率向量总合为1，所以在这里应该是使用总离子做归一化，而非使用最大值做归一化
        ''' </summary>
        ''' <param name="ms"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function StandardizeSpectrum(ms As LibraryMatrix) As LibraryMatrix
            Return ms / ms.intensity.Sum
        End Function

        <Extension>
        Public Function StandardizeSpectrum(ms As ms2()) As ms2()
            Dim sum As Double = ms.Sum(Function(a) a.intensity)
            Dim norm = ms _
                .Select(Function(a)
                            Return New ms2(a.mz, a.intensity / sum) With {
                                .Annotation = a.Annotation
                            }
                        End Function) _
                .ToArray

            Return norm
        End Function

        <Extension>
        Public Iterator Function StandardizeAlignment(align As SSM2MatrixFragment()) As IEnumerable(Of SSM2MatrixFragment)
            Dim qsum As Double = align.Sum(Function(a) a.query)
            Dim rsum As Double = align.Sum(Function(a) a.ref)

            For Each fragment As SSM2MatrixFragment In align
                Yield New SSM2MatrixFragment With {
                    .da = fragment.da,
                    .mz = fragment.mz,
                    .query = fragment.query / qsum,
                    .ref = fragment.ref / rsum
                }
            Next
        End Function

        <Extension>
        Public Function Entropy(ms As PeakMs2) As Double
            Dim msms As New LibraryMatrix With {.ms2 = ms.mzInto}
            Dim ent As Double = StandardizeSpectrum(msms).intensity.ShannonEntropy
            Return ent
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="alignment">
        ''' the intensity value must be standardized
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function calculate_entropy_similarity(alignment As SSM2MatrixFragment()) As Double
            Dim p As New Vector(From mzi In alignment Select mzi.query)
            Dim q As New Vector(From mzi In alignment Select mzi.ref)

            Return _entropy_similarity(p, q)
        End Function

        Public Function calculate_entropy_similarity(spectrum_a As ms2(),
                                                     spectrum_b As ms2(),
                                                     tolerance As Tolerance) As Double
            Return GlobalAlignment _
                .CreateAlignment(
                    query:=StandardizeSpectrum(New LibraryMatrix(spectrum_a)).ms2,
                    refrs:=StandardizeSpectrum(New LibraryMatrix(spectrum_b)).ms2,
                    tolerance:=tolerance
                ) _
                .ToArray _
                .calculate_entropy_similarity
        End Function

        Public Function calculate_entropy_similarity(spectrum_a As ms2(),
                                                     spectrum_b As ms2(),
                                                     Optional ms2_da As Double = 0.3) As Double
            Return GlobalAlignment _
                .CreateAlignment(
                    query:=StandardizeSpectrum(New LibraryMatrix(spectrum_a)).ms2,
                    refrs:=StandardizeSpectrum(New LibraryMatrix(spectrum_b)).ms2,
                    tolerance:=Tolerance.DeltaMass(ms2_da)
                ) _
                .ToArray _
                .calculate_entropy_similarity
        End Function

        Private Function _entropy_similarity(a As Vector, b As Vector) As Double
            Dim ia = _get_entropy_and_weighted_intensity(a)
            Dim ib = _get_entropy_and_weighted_intensity(b)
            Dim merged As Vector = ia.intensity + ib.intensity
            Dim entropy_merged = (merged / merged.Sum).ShannonEntropy

            Static log4 As Double = std.Log(4)

            Dim similarity As Double = 1 - (2 * entropy_merged - ia.spectral_entropy - ib.spectral_entropy) / log4

            similarity = If(similarity < 0.0, 0.0, similarity)
            similarity = If(similarity > 1.0, 1.0, similarity)

            Return similarity
        End Function

        Private Function _get_entropy_and_weighted_intensity(intensity As Vector) As (spectral_entropy As Double, intensity As Vector)
            Dim spectral_entropy As Double = intensity.ShannonEntropy

            If spectral_entropy < 3 Then
                Dim weight = 0.25 + 0.25 * spectral_entropy
                Dim weighted_intensity As Vector = intensity ^ weight
                Dim intensity_sum = weighted_intensity.Sum

                weighted_intensity /= intensity_sum
                spectral_entropy = weighted_intensity.ShannonEntropy

                Return (spectral_entropy, weighted_intensity)
            Else
                Return (spectral_entropy, intensity)
            End If
        End Function
    End Module
End Namespace
