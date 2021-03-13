#Region "Microsoft.VisualBasic::e769f1be6d4157fb2108b3ceb9253d1d, TargetedMetabolomics\GCMS\QuantifyAnalysis\QuantifyIonExtract.vb"

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

'     Class QuantifyIonExtract
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: GetMsScan, GetSamplePeaks
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports stdNum = System.Math

Namespace GCMS.QuantifyAnalysis

    Public MustInherit Class QuantifyIonExtract : Inherits FeatureExtract(Of Raw)

        Protected Friend ReadOnly ions As QuantifyIon()
        ''' <summary>
        ''' apply for convert to centroid data
        ''' </summary>
        Protected Friend ReadOnly ms1ppm As Tolerance
        Protected Friend ReadOnly dadot3 As Tolerance = Tolerance.DeltaMass(0.3)
        Protected Friend ReadOnly rtshift As Double
        Protected Friend ReadOnly baselineQuantile As Double

        Protected Sub New(ions As IEnumerable(Of QuantifyIon),
                          peakwidth As DoubleRange,
                          centroid As Tolerance,
                          rtshift As Double,
                          baselineQuantile As Double)

            Call MyBase.New(peakwidth)

            Me.ms1ppm = centroid
            Me.ions = ions.ToArray
            Me.rtshift = rtshift
            Me.baselineQuantile = baselineQuantile
        End Sub

        Public Function FindIon(ROI As ROI) As QuantifyIon
            Return ions _
                .OrderByDescending(Function(i)
                                       Dim rtmin = stdNum.Abs(i.rt.Min - ROI.time.Min)
                                       Dim rtmax = stdNum.Abs(i.rt.Max - ROI.time.Max)

                                       If rtmin > rtshift Then
                                           rtmin = 0
                                       End If
                                       If rtmax > rtshift Then
                                           rtmax = 0
                                       End If

                                       Return 1 / rtmin + 1 / rtmax
                                   End Function) _
                .First
        End Function

        Public Function GetAllFeatures(sample As Raw) As IEnumerable(Of ROI)
            Return GetTICPeaks(sample.GetTIC, sn:=5, baselineQuantile:=baselineQuantile)
        End Function

        Public Overrides Iterator Function GetSamplePeaks(sample As Raw) As IEnumerable(Of TargetPeakPoint)
            ' get all features
            Dim ROI As ROI() = sample.DoCall(AddressOf GetAllFeatures).ToArray
            Dim rtmin As Vector = ROI.Select(Function(r) r.time.Min).ToArray
            Dim rtmax As Vector = ROI.Select(Function(r) r.time.Max).ToArray
            Dim ms As ms2()() = ROI.Select(Function(r) GetMsScan(sample, r.time)).ToArray
            Dim peak As New Value(Of TargetPeakPoint)

            For Each ion As QuantifyIon In ions
                If Not peak = GetTargetPeak(sample, ROI, ms, rtmin, rtmax, ion) Is Nothing Then
                    Yield CType(peak, TargetPeakPoint)
                End If
            Next
        End Function

        Private Function GetTargetPeak(sample As Raw, ROI As ROI(), MS As ms2()(), rtmin As Vector, rtmax As Vector, ion As QuantifyIon) As TargetPeakPoint
            Dim rtminScore As Vector = (ion.rt.Min - rtmin).Abs.Select(Function(dt) If(dt >= rtshift, 99999999, dt)).AsVector
            Dim rtmaxScore As Vector = (ion.rt.Max - rtmax).Abs.Select(Function(dt) If(dt >= rtshift, 99999999, dt)).AsVector
            Dim feature As ROI
            Dim cos As Vector = MS _
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
                Return Nothing
            Else
                feature = ROI(Which.Max(scores))
            End If

            Return GetPeak(ion.id, feature.time, sample)
        End Function

        Protected Function GetMsScan(sample As Raw, rt As DoubleRange) As ms2()
            Dim ms_scan As ms1_scan() = sample.GetMsScan(rt)
            Dim spectra As ms2() = ms_scan _
                .Select(Function(scan)
                            Return New ms2 With {
                                .mz = scan.mz,
                                .intensity = scan.intensity
                            }
                        End Function) _
                .ToArray _
                .Centroid(ms1ppm, LowAbundanceTrimming.Default) _
                .ToArray

            Return spectra
        End Function

        Protected MustOverride Function GetPeak(ion_id As String, rt As DoubleRange, sample As Raw) As TargetPeakPoint

        Public Iterator Function LoadSamples(files As IEnumerable(Of NamedValue(Of String)), qIon As QuantifyIon) As IEnumerable(Of TargetPeakPoint)
            Dim peakTicks As New Value(Of TargetPeakPoint)
            Dim raw As Raw

            For Each file As NamedValue(Of String) In files
                If file.Value.ExtensionSuffix("cdf") Then
                    raw = netCDFReader.Open(file).ReadData(showSummary:=False)
                Else
                    raw = mzMLReader.LoadFile(file)
                End If

                ' get all features
                Dim ROI As ROI() = raw.DoCall(AddressOf GetAllFeatures).ToArray
                Dim rtmin As Vector = ROI.Select(Function(r) r.time.Min).ToArray
                Dim rtmax As Vector = ROI.Select(Function(r) r.time.Max).ToArray
                Dim ms As ms2()() = ROI.Select(Function(r) GetMsScan(raw, r.time)).ToArray

                If Not peakTicks = GetTargetPeak(raw, ROI, ms, rtmin, rtmax, qIon) Is Nothing Then
                    Yield CType(peakTicks, TargetPeakPoint)
                End If
            Next
        End Function
    End Class
End Namespace
