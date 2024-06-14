#Region "Microsoft.VisualBasic::1d75fc1921301017ea396f507d769026, visualize\MsImaging\Layer\PixelsCDF.vb"

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

    '   Total Lines: 220
    '    Code Lines: 163 (74.09%)
    ' Comment Lines: 27 (12.27%)
    '    - Xml Docs: 92.59%
    ' 
    '   Blank Lines: 30 (13.64%)
    '     File Size: 8.51 KB


    ' Module PixelsCDF
    ' 
    '     Function: (+2 Overloads) CreateMs1, (+2 Overloads) CreatePixelReader, GetMsiDimension, GetMzTolerance, LoadPixelsData
    ' 
    '     Sub: CreateCDF
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.DataStorage.netCDF.Components
Imports Microsoft.VisualBasic.DataStorage.netCDF.Data
Imports Microsoft.VisualBasic.DataStorage.netCDF.DataVector
Imports Microsoft.VisualBasic.Language
Imports std = System.Math

Public Module PixelsCDF

    ''' <summary>
    ''' Write image layers
    ''' </summary>
    ''' <param name="loadedPixels"></param>
    ''' <param name="file"></param>
    ''' <param name="dimension"></param>
    ''' <param name="tolerance"></param>
    <Extension>
    Public Sub CreateCDF(loadedPixels As PixelData(), file As Stream, dimension As Size, tolerance As Tolerance, Optional rgb As RGBConfigs = Nothing)
        Using matrix As New CDFWriter(file)
            Dim mz As New List(Of Double)
            Dim intensity As New List(Of Double)
            Dim x As New List(Of Integer)
            Dim y As New List(Of Integer)

            ' 下面的4个向量等长
            For Each p As PixelData In loadedPixels
                mz.Add(p.mz)
                intensity.Add(p.intensity)
                x.Add(p.x)
                y.Add(p.y)
            Next

            matrix.GlobalAttributes(New attribute With {.name = "width", .value = dimension.Width, .type = CDFDataTypes.NC_INT})
            matrix.GlobalAttributes(New attribute With {.name = "height", .value = dimension.Height, .type = CDFDataTypes.NC_INT})
            matrix.GlobalAttributes(New attribute With {.name = "program", .value = "mzkit_win32", .type = CDFDataTypes.NC_CHAR})
            matrix.GlobalAttributes(New attribute With {.name = "github", .value = "https://github.com/xieguigang/mzkit", .type = CDFDataTypes.NC_CHAR})
            matrix.GlobalAttributes(New attribute With {.name = "time", .value = Now.ToString, .type = CDFDataTypes.NC_CHAR})
            matrix.Dimensions(New Dimension("pixels", loadedPixels.Length))

            Dim mzErr As New attribute With {
                .name = "tolerance",
                .type = CDFDataTypes.NC_CHAR,
                .value = tolerance.GetScript
            }

            matrix.AddVariable("mz", New doubles(mz), "pixels", mzErr)
            matrix.AddVariable("intensity", New doubles(intensity), "pixels")
            matrix.AddVariable("x", New integers(x), "pixels")
            matrix.AddVariable("y", New integers(y), "pixels")

            If Not rgb Is Nothing Then
                Dim configJSON As String = rgb.GetJSON
                Dim chrs As New chars(configJSON)
                Dim jsonDims As New Dimension(chrs) With {.name = "rgb_configs"}

                Call matrix.AddVariable("rgb", chrs, jsonDims)
            End If
        End Using
    End Sub

    <Extension>
    Public Function GetMzTolerance(cdf As netCDFReader) As Tolerance
        Dim mz As variable = cdf.getDataVariableEntry("mz")
        Dim errStr As String = mz.FindAttribute("tolerance")?.value

        Return Tolerance.ParseScript(errStr)
    End Function

    <Extension>
    Public Function GetMsiDimension(cdf As netCDFReader) As Size
        Dim w As Integer = CType(cdf!width, Integer)
        Dim h As Integer = CType(cdf!height, Integer)

        Return New Size With {
            .Width = w,
            .Height = h
        }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="cdf">
    ''' should contains ``mz``, ``intensity``, ``x`` and ``y``
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function LoadPixelsData(cdf As netCDFReader) As IEnumerable(Of PixelData)
        Dim mz As doubles = cdf.getDataVariable("mz")
        Dim intensity As doubles = cdf.getDataVariable("intensity")
        Dim x As integers = cdf.getDataVariable("x")
        Dim y As integers = cdf.getDataVariable("y")

        For i As Integer = 0 To mz.Length - 1
            Yield New PixelData With {
                .mz = mz(i),
                .intensity = intensity(i),
                .x = x(i),
                .y = y(i)
            }
        Next
    End Function

    <Extension>
    Public Iterator Function CreateMs1(cdf As netCDFReader) As IEnumerable(Of ScanMS1)
        Dim xy = cdf.LoadPixelsData.GroupBy(Function(p) p.x).ToArray
        Dim scan As ScanMS1

        For Each x In xy
            Dim ylist = x.GroupBy(Function(p) p.y).ToArray

            For Each y In ylist
                scan = New ScanMS1 With {
                    .mz = y.Select(Function(m) m.mz).ToArray,
                    .into = y.Select(Function(m) m.intensity).ToArray,
                    .meta = New Dictionary(Of String, String) From {
                        {"x", x.Key},
                        {"y", y.Key}
                    },
                    .scan_id = $"[{x.Key},{y.Key}]"
                }

                Yield scan
            Next
        Next
    End Function

    <Extension>
    Public Function CreateMs1(pixel As PixelScan,
                              Optional excludesMz As Double() = Nothing,
                              Optional mzdiff As Double = 0.05) As ScanMS1

        Dim matrix As ms2() = pixel.GetMs

        If Not excludesMz.IsNullOrEmpty Then
            matrix = matrix _
                .Where(Function(mzi)
                           Return Not excludesMz.Any(Function(d) std.Abs(d - mzi.mz) <= mzdiff)
                       End Function) _
                .ToArray
        End If

        Return New ScanMS1 With {
            .mz = matrix.Select(Function(m) m.mz).ToArray,
            .into = matrix.Select(Function(m) m.intensity).ToArray,
            .meta = New Dictionary(Of String, String) From {
                {"x", pixel.X},
                {"y", pixel.Y}
            },
            .scan_id = pixel.scanId
        }
    End Function

    ''' <summary>
    ''' convert the layer matrix as the MSimaging pixel data reader
    ''' </summary>
    ''' <param name="cdf"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CreatePixelReader(cdf As netCDFReader) As ReadRawPack
        Dim size As Size = cdf.GetMsiDimension
        Dim pixels As New List(Of mzPackPixel)

        For Each scan As ScanMS1 In cdf.CreateMs1
            With scan.GetMSIPixel
                pixels += New mzPackPixel(scan, .X, .Y)
            End With
        Next

        Return New ReadRawPack(pixels, size, resolution:=17)
    End Function

    ''' <summary>
    ''' create data reader from a given collection of the pixel data
    ''' </summary>
    ''' <param name="allPixels">
    ''' maybe a subset of the pixel data
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function CreatePixelReader(allPixels As PixelScan(),
                                      Optional offsetX As Integer = 0,
                                      Optional offsetY As Integer = 0,
                                      Optional excludesMz As Double() = Nothing,
                                      Optional mzdiff As Double = 0.05,
                                      Optional verbose As Boolean = True) As ReadRawPack

        Dim w = Aggregate i In allPixels Into Max(i.X)
        Dim h = Aggregate i In allPixels Into Max(i.Y)
        Dim size As New Size With {
           .Width = w + offsetX,
           .Height = h + offsetY
        }
        Dim pixels As New List(Of mzPackPixel)
        Dim point As mzPackPixel

        For Each scan As PixelScan In allPixels
            point = New mzPackPixel(
                scan:=scan.CreateMs1(excludesMz, mzdiff),
                x:=scan.X + offsetX,
                y:=scan.Y + offsetY
            )

            If point.mz.Length > 0 Then
                pixels += point
            End If
        Next

        Return New ReadRawPack(pixels, size, resolution:=17, verbose:=verbose)
    End Function
End Module
