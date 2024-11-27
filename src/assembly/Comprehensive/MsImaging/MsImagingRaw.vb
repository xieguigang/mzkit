#Region "Microsoft.VisualBasic::8bff2f83e4973a82b08baa9f0fbf8b40, assembly\Comprehensive\MsImaging\MsImagingRaw.vb"

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

    '   Total Lines: 346
    '    Code Lines: 259 (74.86%)
    ' Comment Lines: 50 (14.45%)
    '    - Xml Docs: 90.00%
    ' 
    '   Blank Lines: 37 (10.69%)
    '     File Size: 14.53 KB


    '     Module MsImagingRaw
    ' 
    '         Function: (+3 Overloads) GetMSIMetadata, MeasureRow, MSICombineRowScans, ParseRowNumber, PixelScanPadding
    '                   Reset, Summary, WriteRegionPoints
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Scripting.Expressions

Namespace MsImaging

    ''' <summary>
    ''' raw data file reader helper code
    ''' </summary>
    Public Module MsImagingRaw

        ''' <summary>
        ''' Try to save the tissue region data into mzPack rawdata file
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="index"></param>
        ''' <returns></returns>
        <Extension>
        Public Function WriteRegionPoints(raw As mzPack, index As IEnumerable(Of NamedValue(Of Point))) As mzPack
            Dim dimensionSize As Size = GetDimensionSize(raw)
            Dim evalIndex As Func(Of Point, Integer) = Function(i) BitmapBuffer.GetIndex(i.X, i.Y, dimensionSize.Width, channels:=1)
            Dim regions As Dictionary(Of String, Point()) = index _
            .GroupBy(Function(i) i.Name) _
            .ToDictionary(Function(region) region.Key,
                          Function(region)
                              Return region.Select(Function(i) i.Value).ToArray
                          End Function)

            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="slide"></param>
        ''' <param name="padding"></param>
        ''' <returns></returns>
        <Extension>
        Public Function PixelScanPadding(slide As mzPack, padding As Padding) As mzPack
            Dim dims As Size = PixelReader.ReadDimensions(slide.MS.Select(Function(scan) scan.GetMSIPixel))
            Dim paddingData As ScanMS1() = slide.MS.PixelScanPadding(padding, dims).ToArray

            Return New mzPack With {
            .MS = paddingData,
            .Application = FileApplicationClass.MSImaging,
            .Chromatogram = slide.Chromatogram,
            .Scanners = slide.Scanners,
            .source = slide.source,
            .Thumbnail = slide.Thumbnail,
            .metadata = slide.metadata,
            .Annotations = slide.Annotations
        }
        End Function

        ''' <summary>
        ''' reset sample position
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="padding">
        ''' Add padding around the slide sample data
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function Reset(raw As mzPack, padding As Padding) As mzPack
            Dim rect As RectangleF = raw.Shape
            Dim scans As New List(Of ScanMS1)
            Dim pos As Point
            Dim meta As Dictionary(Of String, String)

            For Each scan As ScanMS1 In raw.MS
                pos = scan.GetMSIPixel
                pos = New Point With {
                    .X = pos.X - rect.Left + padding.Left,
                    .Y = pos.Y - rect.Top + padding.Top
                }
                meta = New Dictionary(Of String, String)(scan.meta)
                meta("x") = pos.X.ToString
                meta("y") = pos.Y.ToString

                scans += New ScanMS1 With {
                .BPC = scan.BPC,
                .into = scan.into,
                .mz = scan.mz,
                .products = scan.products,
                .rt = scan.rt,
                .TIC = scan.TIC,
                .scan_id = scan.scan_id,
                .meta = meta
            }
            Next

            meta = raw.metadata
            meta("width") = rect.Width + padding.Left + padding.Right
            meta("height") = rect.Height + padding.Top + padding.Bottom

            Return New mzPack With {
                .Application = FileApplicationClass.MSImaging,
                .Chromatogram = raw.Chromatogram,
                .MS = scans.ToArray,
                .Scanners = raw.Scanners,
                .source = $"reset({raw.source})",
                .Thumbnail = Nothing,
                .Annotations = raw.Annotations,
                .metadata = meta
            }
        End Function

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
        ''' measure the ms-imaging metadata from an aligned processed matrix object
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GetMSIMetadata(raw As MzMatrix) As Metadata
            Dim dims As New Size With {
               .Width = raw.matrix.Select(Function(i) i.X).Max,
               .Height = raw.matrix.Select(Function(i) i.Y).Max
            }

            Return New Metadata With {
                .[class] = raw.matrixType.ToString,
                .mass_range = New DoubleRange(raw.mz),
                .resolution = 13,
                .scan_x = dims.Width,
                .scan_y = dims.Height
            }
        End Function

        <Extension>
        Public Function GetMSIMetadata(raw As MassSpectrometry.SingleCells.File.MatrixReader) As Metadata
            If raw.matrixType <> FileApplicationClass.MSImaging Then
                Return New Metadata With {.[class] = raw.matrixType.ToString}
            Else
                Dim dims = raw.dim_size

                Return New Metadata With {
                    .[class] = raw.matrixType.ToString,
                    .mass_range = New DoubleRange(raw.ionSet),
                    .resolution = 13,
                    .scan_x = dims.Width,
                    .scan_y = dims.Height
                }
            End If
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
