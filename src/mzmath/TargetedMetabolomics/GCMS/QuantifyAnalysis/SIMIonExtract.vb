#Region "Microsoft.VisualBasic::be765077e96a093ac338bdaf31f023ca, TargetedMetabolomics\GCMS\QuantifyAnalysis\SIMIonExtract.vb"

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

    '     Class SIMIonExtract
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetMsScan, GetPeak, GetSamplePeaks
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports stdNum = System.Math

Namespace GCMS.QuantifyAnalysis

    Public Class SIMIonExtract : Inherits QuantifyIonExtract

        Public Sub New(ions As IEnumerable(Of QuantifyIon), peakwidth As DoubleRange, centroid As Tolerance)
            Call MyBase.New(ions, peakwidth, centroid)
        End Sub

        Public Overrides Iterator Function GetSamplePeaks(sample As Raw) As IEnumerable(Of TargetPeakPoint)
            Dim ROI As ROI() = GetTICPeaks(sample.GetTIC).ToArray
            Dim rtmin As Vector = ROI.Select(Function(r) r.time.Min).ToArray
            Dim rtmax As Vector = ROI.Select(Function(r) r.time.Max).ToArray
            Dim ms As ms2()() = ROI.Select(Function(r) GetMsScan(sample, r.time)).ToArray
            Dim feature As ROI

            For Each ion As QuantifyIon In ions
                Dim rtminScore As Vector = (ion.rt.Min - rtmin).Abs
                Dim rtmaxScore As Vector = (ion.rt.Max - rtmax).Abs
                Dim cos As Vector = ms _
                    .Select(Function(spectra)
                                With GlobalAlignment.TwoDirectionSSM(ion.ms, spectra, dadot3)
                                    Return stdNum.Min(.forward, .reverse)
                                End With
                            End Function) _
                    .ToArray

                rtminScore = rtminScore.Max / rtminScore - 1
                rtmaxScore = rtmaxScore.Max / rtmaxScore - 1

                Dim scores As Vector = (rtminScore + rtmaxScore) * cos

                If scores.All(Function(xi) xi = 0.0) Then
                    Continue For
                Else
                    feature = ROI(Which.Max(scores))
                End If

                Yield GetPeak(ion.id, feature.time, sample)
            Next
        End Function

        Private Function GetPeak(ion_id As String, rt As DoubleRange, sample As Raw) As TargetPeakPoint
            Dim sampleName As String = sample.fileName.BaseName
            Dim spectra As ms2() = GetMsScan(sample, rt)
            ' SIM模式下只使用响应度最高的碎片做定量计算
            Dim maxInto As Double = spectra.Select(Function(mz) mz.intensity).Max
            Dim tick As New ChromatogramTick With {.Intensity = maxInto, .Time = rt.Average}
            Dim q As New Quantile With {.Quantile = 1, .Percentage = 1}

            Return New TargetPeakPoint With {
                .Name = ion_id,
                .SampleName = sampleName,
                .Peak = New ROIPeak With {
                    .base = 0,
                    .peakHeight = maxInto,
                    .ticks = {tick},
                    .window = New DoubleRange(rt)
                },
                .ChromatogramSummary = {q}
            }
        End Function
    End Class
End Namespace
