﻿#Region "Microsoft.VisualBasic::a1afe4e61a43abfbdc1bdaad3a19eaee, mzmath\TargetedMetabolomics\GCMS\QuantifyAnalysis\QuantifyIonExtract.vb"

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

    '   Total Lines: 166
    '    Code Lines: 129 (77.71%)
    ' Comment Lines: 9 (5.42%)
    '    - Xml Docs: 33.33%
    ' 
    '   Blank Lines: 28 (16.87%)
    '     File Size: 7.32 KB


    '     Class QuantifyIonExtract
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) FindIon, GetAllFeatures, GetMsScan, GetSamplePeaks, GetTargetPeak
    '                   LoadSamples
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports std = System.Math

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

        Public Function FindIon(tmin As Double, tmax As Double) As QuantifyIon
            Dim rtmin As Vector = ions.Select(Function(i) std.Abs(i.rt.Min - tmin)).AsVector
            Dim rtmax As Vector = ions.Select(Function(i) std.Abs(i.rt.Max - tmax)).AsVector
            Dim zero As Vector = Double.MinValue
            Dim ion As QuantifyIon

            rtmin(rtmin > rtshift) = zero
            rtmax(rtmax > rtshift) = zero

            If ((rtmin < 0.0) & (rtmax < 0.0)).Sum = ions.Length Then
                ' 不存在
                ion = New QuantifyIon With {
                    .id = $"{tmin}/{tmax}",
                    .rt = {tmin, tmax},
                    .ms = {},
                    .name = .id
                }
            Else
                ion = ions(which.Max(rtmin + rtmax))
            End If

            Return ion
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function FindIon(ROI As ROI) As QuantifyIon
            Return FindIon(ROI.time.Min, ROI.time.Max)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetAllFeatures(sample As Raw) As IEnumerable(Of ROI)
            ' 20210321 信噪比的计算还有点问题
            ' 信噪比过高会丢失很多数据
            ' 在这里设置为默认的较低的1值
            Return GetTICPeaks(sample.GetTIC, sn:=0, baselineQuantile:=baselineQuantile)
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
                                Return std.Min(.forward, .reverse)
                            End With
                        End Function) _
                .ToArray

            rtminScore = rtminScore.Max / rtminScore - 1
            rtmaxScore = rtmaxScore.Max / rtmaxScore - 1

            Dim scores As Vector = (rtminScore + rtmaxScore) * cos

            If scores.All(Function(xi) xi = 0.0) Then
                Return Nothing
            Else
                feature = ROI(which.Max(scores))
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

        Public Iterator Function LoadSamples(files As IEnumerable(Of NamedValue(Of String)), qIon As QuantifyIon, keyByName As Boolean) As IEnumerable(Of TargetPeakPoint)
            Dim peakTicks As New Value(Of TargetPeakPoint)
            Dim raw As Raw

            For Each file As NamedValue(Of String) In files
                If file.Value.ExtensionSuffix("cdf") Then
                    raw = netCDFReader.Open(file).ReadData(showSummary:=False)
                Else
                    raw = mzMLReader.LoadFile(file)
                End If

                raw.fileName = file.Value

                ' get all features
                Dim ROI As ROI() = raw.DoCall(AddressOf GetAllFeatures).ToArray
                Dim rtmin As Vector = ROI.Select(Function(r) r.time.Min).ToArray
                Dim rtmax As Vector = ROI.Select(Function(r) r.time.Max).ToArray
                Dim ms As ms2()() = ROI.Select(Function(r) GetMsScan(raw, r.time)).ToArray

                If Not peakTicks = GetTargetPeak(raw, ROI, ms, rtmin, rtmax, qIon) Is Nothing Then
                    If keyByName Then
                        peakTicks.Value.Name = qIon.name
                    End If

                    Yield CType(peakTicks, TargetPeakPoint)
                End If
            Next
        End Function
    End Class
End Namespace
