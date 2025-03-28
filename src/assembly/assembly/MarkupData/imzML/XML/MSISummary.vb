﻿#Region "Microsoft.VisualBasic::ba41d4dfb296494a7243b055f8a9da22, assembly\assembly\MarkupData\imzML\XML\MSISummary.vb"

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

    '   Total Lines: 144
    '    Code Lines: 100 (69.44%)
    ' Comment Lines: 26 (18.06%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 18 (12.50%)
    '     File Size: 5.37 KB


    '     Class MSISummary
    ' 
    '         Properties: rowScans, size, x, y
    ' 
    '         Function: FromPixels, GetBasePeakMz, GetLayer, GetPixel, GetRowScan
    '                   ToArray, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq

Namespace MarkupData.imzML

    ''' <summary>
    ''' total ions/base peak/average intensity
    ''' </summary>
    ''' <remarks>
    ''' spatial heatmap object consist with a collection of the <see cref="iPixelIntensity"/> 
    ''' spot data in rows.
    ''' </remarks>
    Public Class MSISummary

        ''' <summary>
        ''' [x,y], each row array may be in different size!
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' data is group by y at first and then order by x
        ''' </remarks>
        Public Property rowScans As iPixelIntensity()()
        ''' <summary>
        ''' the MALDI scan dimension size
        ''' </summary>
        ''' <returns></returns>
        Public Property size As Size

        ''' <summary>
        ''' the range of the x axis: [min, max]
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property x As Double()
            Get
                Return New DoubleRange(rowScans.IteratesALL.Select(Function(p) p.x)).MinMax
            End Get
        End Property

        ''' <summary>
        ''' the range of the y axis: [min, max]
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property y As Double()
            Get
                Return New DoubleRange(rowScans.Select(Function(r) r(0).y)).MinMax
            End Get
        End Property

        Public Function GetPixel(x As Integer, y As Integer) As iPixelIntensity
            Dim yscan = rowScans.Where(Function(r) r(0).y = y).FirstOrDefault

            If yscan Is Nothing Then
                Return Nothing
            Else
                Return yscan.Where(Function(p) p.x = x).FirstOrDefault
            End If
        End Function

        Public Shared Function FromPixels(pixels As IEnumerable(Of iPixelIntensity), Optional dims As Size? = Nothing) As MSISummary
            Dim matrix2D As iPixelIntensity()() = pixels _
                .GroupBy(Function(p) p.y) _
                .OrderBy(Function(p) p.Key) _
                .Select(Function(row)
                            Return row.OrderBy(Function(p) p.x).ToArray
                        End Function) _
                .ToArray

            If dims Is Nothing Then
                Dim points = matrix2D _
                    .Select(Function(p)
                                Return p.Select(Function(pi) pi.GetPoint)
                            End Function) _
                    .IteratesALL
                Dim poly As New Polygon2D(points)

                dims = New Size(poly.xpoints.Max, poly.ypoints.Max)
            End If

            Return New MSISummary With {
                .rowScans = matrix2D,
                .size = dims
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ToArray() As iPixelIntensity()
            Return rowScans.IteratesALL.ToArray
        End Function

        Public Function GetBasePeakMz() As LibraryMatrix
            Return New LibraryMatrix With {
                .centroid = False,
                .name = "basePeak M/z",
                .ms2 = rowScans _
                    .IteratesALL _
                    .Select(Function(p)
                                Return New ms2 With {
                                    .mz = p.basePeakMz,
                                    .intensity = p.basePeakIntensity,
                                    .Annotation = $"{p.x},{p.y}"
                                }
                            End Function) _
                    .ToArray
            }
        End Function

        Private Iterator Function GetRowScan(row As iPixelIntensity(), summary As IntensitySummary) As IEnumerable(Of PixelScanIntensity)
            For Each p As iPixelIntensity In row
                Dim into As Double

                Select Case summary
                    Case IntensitySummary.Average : into = p.average
                    Case IntensitySummary.BasePeak : into = p.basePeakIntensity
                    Case IntensitySummary.Total : into = p.totalIon
                    Case IntensitySummary.Median : into = p.median
                    Case Else
                        Throw New NotImplementedException
                End Select

                Yield New PixelScanIntensity With {
                    .totalIon = into,
                    .x = p.x,
                    .y = p.y
                }
            Next
        End Function

        Public Function GetLayer(summary As IntensitySummary) As IEnumerable(Of PixelScanIntensity)
            Return rowScans _
                .Select(Function(row)
                            Return GetRowScan(row, summary)
                        End Function) _
                .IteratesALL
        End Function

        Public Overrides Function ToString() As String
            Return $"[width:={size.Width}, height:={size.Height}]"
        End Function
    End Class
End Namespace
