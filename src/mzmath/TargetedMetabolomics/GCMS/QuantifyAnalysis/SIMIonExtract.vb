#Region "Microsoft.VisualBasic::6c0e21c80c28b1ee6f2ae7fc903f141c, TargetedMetabolomics\GCMS\QuantifyAnalysis\SIMIonExtract.vb"

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
    '         Function: GetPeak
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace GCMS.QuantifyAnalysis

    Public Class SIMIonExtract : Inherits QuantifyIonExtract

        Public Sub New(ions As IEnumerable(Of QuantifyIon),
                       peakwidth As DoubleRange,
                       centroid As Tolerance,
                       rtshift As Double,
                       baselineQuantile As Double)

            Call MyBase.New(ions, peakwidth, centroid, rtshift, baselineQuantile)
        End Sub

        Protected Overrides Function GetPeak(ion_id As String, rt As DoubleRange, sample As Raw) As TargetPeakPoint
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
