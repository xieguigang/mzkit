#Region "Microsoft.VisualBasic::4edf8936c8ccbfc10e6f5eba3ef7da74, mzmath\mz_deco\GCMS\GCMSDeconv.vb"

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
    '     File Size: 2.31 KB


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
                .Centroid(da, cutoff, aggregate:=Function(x) x.Sum) _
                .ToArray

            Yield New GCMSPeak(peak) With {.Spectrum = union}
        Next
    End Function
End Module

''' <summary>
''' A gcms peak feature
''' </summary>
Public Class GCMSPeak : Inherits PeakFeature

    ''' <summary>
    ''' the average spectrum of current GCMS peak ROI
    ''' </summary>
    ''' <returns></returns>
    Public Property Spectrum As ms2()

    Sub New()
    End Sub

    Sub New(peakdata As PeakFeature)
        Call MyBase.New(peakdata)
    End Sub

    ''' <summary>
    ''' Create peak for a single peak feature data
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="peak"></param>
    ''' <param name="rtwin"></param>
    ''' <returns></returns>
    Public Shared Function CreateFeature(raw As IEnumerable(Of PeakMs2), peak As xcms2, Optional rtwin As Double = 1.5) As GCMSPeak
        Dim rtmin = peak.rtmin
        Dim rtmax = peak.rtmax

        If rtmin = 0.0 AndAlso rtmax = 0.0 Then
            rtmin = peak.rt - rtwin
            rtmax = peak.rt + rtwin
        End If

        Dim rt_filter = raw _
            .Where(Function(p) p.rt >= rtmin AndAlso p.rt <= rtmax) _
            .ToArray
        Dim mean = SpectraEncoder.SpectrumSum(rt_filter, average:=True)
        Dim spectrum As ms2()

        ' 20241208 mean may be nothing if there is no spectrum matched in current sample
        If mean Is Nothing Then
            spectrum = {}
        Else
            spectrum = mean.Array
        End If

        Return New GCMSPeak With {
            .rt = peak.rt,
            .area = peak.Properties.Values.Sum,
            .baseline = 1,
            .integration = 1,
            .maxInto = peak.Properties.Values.Max,
            .mz = peak.mz,
            .noise = 1,
            .nticks = 1,
            .rawfile = "",
            .RI = peak.RI,
            .rtmax = rtmax,
            .rtmin = rtmin,
            .Spectrum = spectrum,
            .xcms_id = peak.ID
        }
    End Function

End Class
