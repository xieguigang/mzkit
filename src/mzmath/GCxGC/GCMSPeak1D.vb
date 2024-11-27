#Region "Microsoft.VisualBasic::fab66a228dfa38c981c10a5c20c2e7fe, mzmath\GCxGC\GCMSPeak1D.vb"

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

    '   Total Lines: 63
    '    Code Lines: 45 (71.43%)
    ' Comment Lines: 12 (19.05%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (9.52%)
    '     File Size: 2.56 KB


    ' Class GCMSPeak1D
    ' 
    '     Properties: area, id, maxIntensity, rt, rtmax
    '                 rtmin, sn
    ' 
    '     Function: ExtractPeaks
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

''' <summary>
''' A 1-d gcms peak
''' </summary>
Public Class GCMSPeak1D

    Public Property id As String
    Public Property rt As Double
    Public Property rtmin As Double
    Public Property rtmax As Double
    Public Property area As Double
    Public Property maxIntensity As Double
    Public Property sn As Double

    ''' <summary>
    ''' Extract the gcms peaks from the sample data
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="ms1">
    ''' the sample data
    ''' </param>
    ''' <param name="peakwidth"></param>
    ''' <returns></returns>
    Public Shared Iterator Function ExtractPeaks(Of T As {New, ISpectrum, IRetentionTime})(ms1 As IEnumerable(Of T), peakwidth As DoubleRange) As IEnumerable(Of EIPeak(Of GCMSPeak1D))
        Dim rawdata As T() = ms1.OrderBy(Function(a) a.rt).ToArray
        Dim tic As ChromatogramTick() = rawdata.Select(Function(a) New ChromatogramTick(a.rt, a.GetIons.Sum(Function(i) i.intensity))).ToArray
        Dim index As New BlockSearchFunction(Of T)(rawdata, Function(x) x.rt, tolerance:=5, fuzzy:=True)

        For Each peak As PeakFeature In Deconvolution.DeconvPeakGroups(tic, peakwidth)
            Dim rt_win As New DoubleRange(peak.rtmin, peak.rtmax)
            Dim q = index.Search(New T With {.rt = peak.rt}) _
                .Where(Function(a)
                           Return rt_win.IsInside(a.rt)
                       End Function) _
                .ToArray
            Dim peak1d As New GCMSPeak1D With {
                .rt = peak.rt,
                .rtmin = peak.rtmin,
                .rtmax = peak.rtmax,
                .maxIntensity = peak.maxInto,
                .area = peak.area,
                .id = peak.xcms_id,
                .sn = peak.snRatio
            }

            Yield New EIPeak(Of GCMSPeak1D) With {
                .peak = peak1d,
                .spectrum = q _
                    .Select(Function(a)
                                Return New LibraryMatrix(a.GetIons)
                            End Function) _
                    .ToArray
            }
        Next
    End Function

End Class
