#Region "Microsoft.VisualBasic::9f5598011eb8d7e13ef3930db8a62dea, mzmath\GCxGC\GCxGCPeakDetection.vb"

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

    '   Total Lines: 61
    '    Code Lines: 53 (86.89%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 8 (13.11%)
    '     File Size: 2.72 KB


    ' Module GCxGCPeakDetection
    ' 
    '     Function: Extract2DFeatures, ExtractDimension2Features
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Public Module GCxGCPeakDetection

    <Extension>
    Public Iterator Function Extract2DFeatures(gcxgc As IEnumerable(Of DimensionalSpectrum), rt_win As DoubleRange,
                                               Optional dt1 As Double = 3,
                                               Optional dt2 As Double = 0.5) As IEnumerable(Of EIPeak(Of Peak2D))
        Dim dims2ROI() = gcxgc.AsParallel _
            .Select(Function(d1)
                        Return d1.ExtractDimension2Features(rt_win).Select(Function(d2) (d2, rt1:=d1.rt1))
                    End Function) _
            .IteratesALL _
            .ToArray

        For Each d1_rt In dims2ROI.GroupBy(Function(a) a.rt1, offsets:=dt1)
            Dim rt1 As Double = Val(d1_rt.name)
            Dim rtmin1 As Double = d1_rt.Select(Function(a) a.rt1).Min
            Dim rtmax1 As Double = d1_rt.Select(Function(a) a.rt1).Max

            For Each d2_rt In d1_rt.Select(Function(a) a.d2).GroupBy(Function(a) a.peak.rt, offsets:=dt2)
                Dim rt2 As Double = Val(d2_rt.name)
                Dim max As Double = d2_rt.Select(Function(a) a.peak.maxIntensity).Max
                Dim vol As Double = d2_rt.Select(Function(a) a.peak.area).Sum
                Dim sn As Double = d2_rt.Select(Function(a) a.peak.sn).Average
                Dim peak2d As New Peak2D With {
                    .rt1 = rt1,
                    .rt2 = rt2,
                    .rtmax1 = rtmax1,
                    .rtmin1 = rtmin1,
                    .rtmin2 = d2_rt.Select(Function(a) a.peak.rt).Min,
                    .rtmax2 = d2_rt.Select(Function(a) a.peak.rt).Max,
                    .maxIntensity = max,
                    .volumn = vol,
                    .sn = sn
                }

                Yield New EIPeak(Of Peak2D) With {
                    .peak = peak2d,
                    .spectrum = d2_rt _
                        .Select(Function(a) a.spectrum) _
                        .IteratesALL _
                        .ToArray
                }
            Next
        Next
    End Function

    <Extension>
    Private Function ExtractDimension2Features(d1 As DimensionalSpectrum, rt_win As DoubleRange) As IEnumerable(Of EIPeak(Of GCMSPeak1D))
        Dim dimension2 As PeakMs2() = d1.ms2
        Dim features As IEnumerable(Of EIPeak(Of GCMSPeak1D)) = GCMSPeak1D.ExtractPeaks(dimension2, rt_win)

        Return features
    End Function

End Module

