#Region "Microsoft.VisualBasic::5a8dead4d309d0f9984217ad8a00834d, src\mzmath\TargetedMetabolomics\GCMS\QuantifyAnalysis\ScanIonExtract.vb"

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

    '     Class ScanIonExtract
    ' 
    '         Constructor: (+2 Overloads) Sub New
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
Imports stdNum = System.Math

Namespace GCMS.QuantifyAnalysis

    Public Class ScanIonExtract : Inherits QuantifyIonExtract

        Public Sub New(ions As IEnumerable(Of QuantifyIon), peakwidth As DoubleRange, centroid As Tolerance, rtshift As Double, baselineQuantile As Double)
            Call MyBase.New(ions, peakwidth, centroid, rtshift, baselineQuantile)
        End Sub

        Sub New(base As QuantifyIonExtract)
            Call MyBase.New(base.ions, base.peakwidth, base.ms1ppm, base.rtshift, base.baselineQuantile)
        End Sub

        Protected Overrides Function GetPeak(ion_id As String, rt As DoubleRange, sample As Raw) As TargetPeakPoint
            Dim sampleName As String = sample.fileName.BaseName
            Dim spectra As ms1_scan() = sample.GetMsScan(rt)
            ' Dim maxMz As Double = Me.ions _
            '    .Where(Function(i) i.id = ion_id) _
            '    .First.ms _
            '    .OrderByDescending(Function(mz) mz.intensity) _
            '    .First.mz
            Dim maxMz As Double = spectra.OrderByDescending(Function(mz) mz.intensity).First.mz
            Dim tick As ChromatogramTick() = spectra _
                .Where(Function(scan) stdNum.Abs(scan.mz - maxMz) <= 0.3) _
                .Select(Function(mzi)
                            Return New ChromatogramTick With {
                                .Time = mzi.scan_time,
                                .Intensity = mzi.intensity
                            }
                        End Function) _
                .ToArray
            Dim q As Quantile() = tick.Summary.ToArray
            Dim maxInto As Double = spectra.Select(Function(mz) mz.intensity).Max

            Return New TargetPeakPoint With {
                .Name = ion_id,
                .SampleName = sampleName,
                .Peak = New ROIPeak With {
                    .base = 0,
                    .peakHeight = maxInto,
                    .ticks = tick,
                    .window = New DoubleRange(rt)
                },
                .ChromatogramSummary = q
            }
        End Function
    End Class
End Namespace
