#Region "Microsoft.VisualBasic::b7c2150cdfd428d1c6209782046f41fe, mzkit\src\assembly\Comprehensive\MsImaging\MsImagingRaw.vb"

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

'   Total Lines: 180
'    Code Lines: 133
' Comment Lines: 25
'   Blank Lines: 22
'     File Size: 7.52 KB


'     Module MsImagingRaw
' 
'         Function: GetMSIMetadata, MeasureRow, MSICombineRowScans, ParseRowNumber
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports Microsoft.VisualBasic.Scripting.Expressions

Namespace MsImaging

    ''' <summary>
    ''' raw data file reader helper code
    ''' </summary>
    Public Module MsImagingRaw

        <Extension>
        Public Function Summary(msidata As mzPack, Optional filter As Func(Of Integer, Integer, Boolean) = Nothing) As IEnumerable(Of iPixelIntensity)
            If filter Is Nothing Then
                filter = Function(x, y) True
            End If

            Return From p As ScanMS1
                   In msidata.MS.AsParallel
                   Let xi As Integer = Integer.Parse(p.meta("x"))
                   Let yi As Integer = Integer.Parse(p.meta("y"))
                   Where Not p.into.IsNullOrEmpty
                   Where filter(xi, yi)
                   Select New iPixelIntensity With {
                       .x = xi,
                       .y = yi,
                       .average = p.into.Average,
                       .basePeakIntensity = p.into.Max,
                       .totalIon = p.into.Sum,
                       .basePeakMz = p.mz(which.Max(p.into)),
                       .numIons = p.size,
                       .min = p.into.Min,
                       .median = p.into.Median
                   }
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <returns>
        ''' if the metadata is missing from the mzpack, then function will create new
        ''' </returns>
        <Extension>
        Public Function GetMSIMetadata(raw As mzPack) As Metadata
            Dim src As Dictionary(Of String, String) = raw.metadata
            Dim polygon As New Polygon2D(raw.MS.Select(Function(scan) scan.GetMSIPixel))
            Dim dims As New Size With {
                .Width = polygon.xpoints.Max,
                .Height = polygon.ypoints.Max
            }
            Dim mass = raw.MS _
                .Select(Function(scan) scan.mz) _
                .IteratesALL _
                .ToArray

            If src Is Nothing Then
                src = New Dictionary(Of String, String)
            End If

            Dim metadata As New Metadata With {
                .scan_x = Val(src.TryGetValue("width", [default]:=dims.Width)),
                .scan_y = Val(src.TryGetValue("height", [default]:=dims.Height)),
                .resolution = Val(src.TryGetValue("resolution", [default]:=17)),
                .mass_range = New DoubleRange(mass),
                .[class] = raw.Application.ToString
            }

            Return metadata
        End Function

        ''' <summary>
        ''' the y axis row id is measured via the <see cref="mzPack.source"/>.
        ''' </summary>
        ''' <param name="src">
        ''' the property data of <see cref="mzPack.source"/> can
        ''' not be null or empty string.
        ''' </param>
        ''' <param name="correction"></param>
        ''' <param name="intocutoff"></param>
        ''' <param name="yscale"></param>
        ''' <param name="progress"></param>
        ''' <returns></returns>
        <Extension>
        Public Function MSICombineRowScans(src As IEnumerable(Of mzPack),
                                           correction As Correction,
                                           Optional intocutoff As Double = 0.0,
                                           Optional yscale As Double = 1,
                                           Optional sumNorm As Boolean = True,
                                           Optional labelPrefix As String = Nothing,
                                           Optional progress As RunSlavePipeline.SetMessageEventHandler = Nothing) As mzPack

            Dim pixels As New List(Of ScanMS1)
            Dim cutoff As New RelativeIntensityCutoff(intocutoff)
            Dim metadata As New Metadata
            Dim mzmin As New List(Of Double)
            Dim mzmax As New List(Of Double)
            Dim mzvals As Double()

            If progress Is Nothing Then
                progress = Sub(msg)
                               ' do nothing
                           End Sub
            End If

            ' each row is a small sample in current sample batch
            For Each row As mzPack In src
                pixels += row.MeasureRow(yscale, correction, cutoff, sumNorm, labelPrefix, progress)
                mzvals = row.MS.Select(Function(a) a.mz).IteratesALL.ToArray

                If mzvals.Length > 0 Then
                    mzmin.Add(mzvals.Min)
                    mzmax.Add(mzvals.Max)
                End If
            Next

            Dim polygon As New Polygon2D(pixels.Select(Function(scan) scan.GetMSIPixel))

            metadata.scan_x = polygon.xpoints.Max
            metadata.scan_y = polygon.ypoints.Max
            metadata.mass_range = New DoubleRange(mzmin.Min, mzmax.Max)

            Return New mzPack With {
                .MS = pixels.ToArray,
                .Application = FileApplicationClass.MSImaging,
                .source = Strings _
                    .Trim(labelPrefix) _
                    .Trim("-"c, " "c, CChar(vbTab), "_"c),
                .metadata = metadata.GetMetadata
            }
        End Function

        Private Function ParseRowNumber(sourceTag As String, labelPrefix As String) As Integer
            If labelPrefix.StringEmpty Then
                Return sourceTag _
                    .Match("\d+") _
                    .DoCall(AddressOf Integer.Parse)
            Else
                Return sourceTag _
                    .Replace(labelPrefix, "") _
                    .Match("\d+") _
                    .DoCall(AddressOf Integer.Parse)
            End If
        End Function

        <Extension>
        Private Iterator Function MeasureRow(row As mzPack,
                                             yscale As Double,
                                             correction As Correction,
                                             cutoff As RelativeIntensityCutoff,
                                             sumNorm As Boolean,
                                             labelPrefix As String,
                                             progress As RunSlavePipeline.SetMessageEventHandler) As IEnumerable(Of ScanMS1)
            If row?.source.StringEmpty Then
                Call progress("[warning] source file is empty!")
                Return
            End If

            Dim i As i32 = 1
            Dim y As Integer = ParseRowNumber(row.source, labelPrefix) * yscale
            Dim TIC As Double = Aggregate scan As ScanMS1
                                In row.MS
                                Let total As Double = scan.into.Sum
                                Into Sum(total)

            Call progress($"append_row_data: {row.source} [y={y}]...")

            If TypeOf correction Is ScanMs2Correction Then
                Call DirectCast(correction, ScanMs2Correction).SetMs1Scans(row.MS)
            End If

            For Each scan As ScanMS1 In row.MS
                Dim x As Integer = If(correction Is Nothing, ++i, correction.GetPixelRowX(scan))
                Dim ms As ms2() = cutoff.Trim(scan.GetMs)
                Dim mz As Double() = ms.Select(Function(m) m.mz).ToArray
                Dim into As Double() = ms.Select(Function(m) m.intensity).ToArray

                If sumNorm Then
                    ' normalized intensity data for each pixel
                    into = New Vector(into) / TIC * 10 ^ 8
                End If

                Yield New ScanMS1 With {
                    .BPC = scan.BPC,
                    .into = into,
                    .mz = mz,
                    .meta = New Dictionary(Of String, String) From {{NameOf(x), x}, {NameOf(y), y}},
                    .rt = scan.rt,
                    .scan_id = $"[{row.source}] {scan.scan_id}",
                    .TIC = scan.TIC,
                    .products = scan.products
                }
            Next
        End Function
    End Module
End Namespace
