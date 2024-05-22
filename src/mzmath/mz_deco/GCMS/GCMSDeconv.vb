#Region "Microsoft.VisualBasic::5945c96bbbdda86aea65ab8ebb3248b9, mzmath\mz_deco\GCMS\GCMSDeconv.vb"

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

    '   Total Lines: 56
    '    Code Lines: 37 (66.07%)
    ' Comment Lines: 9 (16.07%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (17.86%)
    '     File Size: 2.28 KB


    ' Module GCMSDeconv
    ' 
    '     Function: DeconvGCMSRawdata
    ' 
    ' Class GCMSPeak
    ' 
    '     Properties: Spectrum
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' tools for decode GC-MS raw data
''' </summary>
Public Module GCMSDeconv

    ''' <summary>
    ''' this function deconvolution of the gc-ms TIC data, extract the corresponding peaks,
    ''' and then extract the ms1 spectrum fro each detected peaks data.
    ''' </summary>
    ''' <param name="gcms">should be a collection of the ms1 scan data which is extract from the rawdata files.</param>
    ''' <returns></returns>
    Public Iterator Function DeconvGCMSRawdata(gcms As IEnumerable(Of PeakMs2), peakwidth As DoubleRange,
                                               Optional quantile# = 0.65,
                                               Optional sn_threshold As Double = 3,
                                               Optional joint As Boolean = True) As IEnumerable(Of GCMSPeak)

        Dim pool As PeakMs2() = gcms.SafeQuery.ToArray
        Dim TIC As ChromatogramTick() = pool.ToChromatogram.OrderBy(Function(t) t.Time).ToArray
        Dim peaks = TIC.DeconvPeakGroups(peakwidth, quantile, sn_threshold, joint).ToArray
        Dim da As Tolerance = Tolerance.DeltaMass(0.3)
        Dim cutoff As New RelativeIntensityCutoff(0.05)

        For Each peak As PeakFeature In peaks
            Dim spectrumMs1 = (From scan As PeakMs2
                               In pool.AsParallel
                               Where scan.rt >= peak.rtmin AndAlso scan.rt <= peak.rtmax).ToArray
            Dim union As ms2() = spectrumMs1 _
                .Select(Function(p) p.mzInto) _
                .IteratesALL _
                .ToArray _
                .CentroidSum(da, cutoff) _
                .ToArray

            Yield New GCMSPeak(peak) With {.Spectrum = union}
        Next
    End Function
End Module

Public Class GCMSPeak : Inherits PeakFeature

    Public Property Spectrum As ms2()

    Sub New()
    End Sub

    Sub New(peakdata As PeakFeature)
        Call MyBase.New(peakdata)
    End Sub

End Class
