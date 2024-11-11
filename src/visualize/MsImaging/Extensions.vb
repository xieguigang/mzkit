#Region "Microsoft.VisualBasic::ddfaaf622294d243d7e93bba787158c0, visualize\MsImaging\Extensions.vb"

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

    '   Total Lines: 281
    '    Code Lines: 196 (69.75%)
    ' Comment Lines: 53 (18.86%)
    '    - Xml Docs: 98.11%
    ' 
    '   Blank Lines: 32 (11.39%)
    '     File Size: 10.24 KB


    ' Module Extensions
    ' 
    '     Function: (+2 Overloads) AsRaster, DensityCut, GetPixelKeys, (+2 Overloads) PixelScanPadding, Reset
    '               ScanMeltdown, Shape
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DataMining.DensityQuery
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Point = System.Drawing.Point

<HideModuleName>
Public Module Extensions

    ''' <summary>
    ''' get pixels boundary of the MSImaging
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <returns>
    ''' return a bounding box of current slide sample, includes
    ''' location and size
    ''' </returns>
    <Extension>
    Public Function Shape(raw As IMZPack) As RectangleF
        Dim allPixels As Point() = raw.MS.Select(Function(scan) scan.GetMSIPixel).ToArray
        Dim polygonShape As New Polygon2D(allPixels)
        Dim rect = polygonShape.GetRectangle

        Return rect
    End Function

    ''' <summary>
    ''' parse pixel mapping from 
    ''' </summary>
    ''' <returns>
    ''' [xy => index]
    ''' </returns>
    <Extension>
    Public Function GetPixelKeys(raw As IMzPackReader) As Dictionary(Of String, String())
        Return raw.EnumerateIndex _
            .Select(Function(id)
                        Dim meta = raw.GetMetadata(id)
                        Dim pxy = $"{meta!x},{meta!y}"

                        Return (id, pxy)
                    End Function) _
            .GroupBy(Function(t) t.pxy) _
            .ToDictionary(Function(t) t.Key,
                            Function(t)
                                Return t.Select(Function(i) i.id).ToArray
                            End Function)
    End Function

    <Extension>
    Public Iterator Function PixelScanPadding(raw As IEnumerable(Of ScanMS1),
                                              padding As Padding,
                                              dims As Size) As IEnumerable(Of ScanMS1)

        Dim marginRight As Integer = dims.Width - padding.Right
        Dim marginLeft As Integer = padding.Left
        Dim marginTop As Integer = padding.Top
        Dim marginBottom As Integer = dims.Height - padding.Bottom

        For Each sample As ScanMS1 In raw
            Dim pxy As Point = sample.GetMSIPixel

            If pxy.X < marginLeft Then
                Continue For
            ElseIf pxy.X > marginRight Then
                Continue For
            ElseIf pxy.Y < marginTop Then
                Continue For
            ElseIf pxy.Y > marginBottom Then
                Continue For
            End If

            Yield sample
        Next
    End Function

    ''' <summary>
    ''' removes the pixel points by the average density cutoff
    ''' </summary>
    ''' <param name="layer"></param>
    ''' <param name="qcut"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function DensityCut(layer As IEnumerable(Of PixelData), Optional qcut As Double = 0.1) As IEnumerable(Of PixelData)
        Dim raw As PixelData() = layer.ToArray
        Dim densityList = raw _
            .Density(Function(p) $"{p.x},{p.y}", Function(p) p.x, Function(p) p.y, New Size(5, 5)) _
            .OrderByDescending(Function(d)
                                   Return d.Value
                               End Function) _
            .ToArray
        Dim q As New FastRankQuantile(densityList.Select(Function(d) d.Value))

        qcut = q.Query(qcut)

        Dim pid As Index(Of String) = (From d As NamedValue(Of Double)
                                       In densityList
                                       Where d.Value >= qcut
                                       Select d.Name).Indexing

        For Each point As PixelData In raw
            If $"{point.x},{point.y}" Like pid Then
                Yield point
            End If
        Next
    End Function

    ''' <summary>
    ''' cast ion layer data to raster matrix object
    ''' </summary>
    ''' <param name="layer"></param>
    ''' <returns></returns>
    <Extension>
    Public Function AsRaster(layer As SingleIonLayer) As RasterMatrix
        Dim dims As Size = layer.DimensionSize
        Dim matrix As RasterMatrix = RasterMatrix.CreateDenseMatrix(layer.MSILayer, dims.Width, dims.Height)
        Return matrix
    End Function

    ''' <summary>
    ''' cast ion layer data to raster matrix object
    ''' </summary>
    ''' <param name="layer"></param>
    ''' <param name="kind"></param>
    ''' <returns></returns>
    <Extension>
    Public Function AsRaster(layer As MSISummary, kind As IntensitySummary) As RasterMatrix
        Dim heatmap As IEnumerable(Of PixelScanIntensity) = layer.GetLayer(summary:=kind)
        Dim dims As Size = layer.size
        Dim matrix As RasterMatrix = RasterMatrix.CreateDenseMatrix(heatmap, dims.Width, dims.Height)
        Return matrix
    End Function
End Module
