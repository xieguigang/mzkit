#Region "Microsoft.VisualBasic::585a9b5d16d18b56cb87c2480091089f, src\mzmath\TargetedMetabolomics\GCMS\ReaderExtensions.vb"

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

'     Module ReaderExtensions
' 
'         Function: CreateMatrix, ExportROI, GetROITable, (+2 Overloads) ReadData, ToTable
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS.Vendors
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text

Namespace GCMS

    <HideModuleName>
    Public Module ReaderExtensions

        <Extension>
        Public Function MzList(raw As ms1_scan()()) As Double()
            Return raw _
                .IteratesALL _
                .Select(Function(scan) scan.mz) _
                .GroupBy(offsets:=0.01) _
                .Select(Function(c) Val(c.name)) _
                .ToArray
        End Function

        <Extension>
        Public Function XIC(raw As Raw, Optional minPoints As Integer = 3) As IEnumerable(Of NamedCollection(Of ms1_scan))
            Dim allScans As New List(Of ms1_scan)
            Dim times As Double() = raw.times

            For i As Integer = 0 To raw.ms.Length - 1
                Dim time As Double = times(i)

                For Each tick As ms1_scan In raw.ms(i)
                    tick.scan_time = time
                Next

                allScans.AddRange(raw.ms(i))
            Next

            Return allScans _
                .GroupBy(Function(tick) tick.mz, offsets:=0.01) _
                .Where(Function(mz)
                           Return mz.Length >= minPoints
                       End Function)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ExportROI(gcms As Raw, angle#, peakwidth As DoubleRange, Optional sn_threshold As Double = 3) As ROI()
            Return gcms.GetTIC _
                .Shadows _
                .PopulateROI(
                    angleThreshold:=angle,
                    peakwidth:=peakwidth,
                    snThreshold:=sn_threshold
                ) _
                .ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension> Public Function CreateMatrix(roi As ROITable) As LibraryMatrix
            Return roi.mass_spectra _
                .DecodeBase64 _
                .Split(ASCII.TAB) _
                .Select(Function(f)
                            Return f.Split _
                                .Select(Function(s) Val(s)) _
                                .ToArray
                        End Function) _
                .Select(Function(t)
                            Return New ms2 With {
                                .mz = t(0),
                                .intensity = t(1),
                                .quantity = .intensity
                            }
                        End Function) _
                .ToArray
        End Function

        ''' <summary>
        ''' 读取CDF文件然后读取原始数据
        ''' </summary>
        ''' <param name="cdfPath"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ReadData(cdfPath$, Optional vendor$ = "agilentGCMS", Optional showSummary As Boolean = True) As Raw
            Return New netCDFReader(cdfPath).ReadData(vendor, showSummary)
        End Function

        <Extension>
        Public Function ReadData(cdf As netCDFReader, Optional vendor$ = "agilentGCMS", Optional showSummary As Boolean = True) As Raw
            If showSummary Then
                Call Console.WriteLine(cdf.ToString)
            End If

            Select Case vendor
                Case "agilentGCMS" : Return agilentGCMS.Read(cdf)
                Case Else
                    Throw New NotImplementedException(vendor)
            End Select
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetROITable(ROIobj As ROI, Optional getTitle As Func(Of ROI, String) = Nothing) As ROITable
            Static defaultRtTitle As New [Default](Of Func(Of ROI, String))(
               Function(roi)
                   Return $"[{roi.time.Min.ToString("F0")},{roi.time.Max.ToString("F0")}]"
               End Function)

            Return New ROITable With {
                .baseline = ROIobj.baseline,
                .ID = (getTitle Or defaultRtTitle)(ROIobj),
                .integration = ROIobj.integration,
                .maxInto = ROIobj.maxInto,
                .rtmax = ROIobj.time.Max,
                .rtmin = ROIobj.time.Min,
                .rt = ROIobj.rt,
                .sn = ROIobj.snRatio
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ToTable(ROIlist As IEnumerable(Of ROI), Optional getTitle As Func(Of ROI, String) = Nothing) As ROITable()
            Return ROIlist _
                .SafeQuery _
                .Select(Function(ROI, i)
                            Return ROI _
                                .GetROITable(Function(region)
                                                 If getTitle Is Nothing Then
                                                     Return "#" & (i + 1)
                                                 Else
                                                     Return getTitle(region)
                                                 End If
                                             End Function)
                        End Function) _
                .ToArray
        End Function
    End Module
End Namespace
