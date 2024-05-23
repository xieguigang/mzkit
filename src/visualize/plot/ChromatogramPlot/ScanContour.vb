#Region "Microsoft.VisualBasic::2c76f6084776b79f26a21e812270b8cd, visualize\plot\ChromatogramPlot\ScanContour.vb"

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

    '   Total Lines: 106
    '    Code Lines: 86 (81.13%)
    ' Comment Lines: 4 (3.77%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 16 (15.09%)
    '     File Size: 3.88 KB


    ' Class ScanContour
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: CreateMatrix, Plot
    ' 
    '     Sub: PlotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Contour
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports stdNum = System.Math

Public Class ScanContour : Inherits Plot

    Dim scans As ms1_scan()
    Dim mzRange As DoubleRange
    Dim rtRange As DoubleRange
    Dim xsteps As Double
    Dim ysteps As Double

    Public Sub New(scans As ms1_scan(), theme As Theme)
        MyBase.New(theme)

        Me.scans = scans
        Me.mzRange = scans.Select(Function(x) x.mz).CreateAxisTicks
        Me.rtRange = scans.Select(Function(x) x.scan_time).CreateAxisTicks
        Me.xsteps = (rtRange.Max - rtRange.Min) / 800
        Me.ysteps = (mzRange.Max - mzRange.Min) / 800
    End Sub

    ''' <summary>
    ''' group by mz/rt
    ''' </summary>
    ''' <returns></returns>
    Private Iterator Function CreateMatrix() As IEnumerable(Of MeasureData)
        Dim rtLeft As Double = 0
        Dim mzLeft As Double = 0
        Dim mzi As Double

        For rt As Double = rtRange.Min To rtRange.Max Step xsteps
            Dim rti As Double = rt
            Dim rtRow As ms1_scan() = scans _
                .Where(Function(x) x.scan_time >= rtLeft AndAlso x.scan_time < rti) _
                .ToArray

            If rtRow.Length = 0 Then
                Continue For
            End If

            For mz As Double = mzRange.Min To mzRange.Max Step ysteps
                mzi = mz

                With rtRow _
                    .Where(Function(x)
                               Return x.mz >= mzLeft AndAlso x.mz < mzi
                           End Function) _
                    .ToArray

                    If .Length = 0 Then
                        Yield New MeasureData(
                            rt, mz,
                            z:= .OrderByDescending(Function(x) x.intensity) _
                                .First _
                                .intensity _
                                .DoCall(AddressOf stdNum.Log10)
                        )
                    End If
                End With

                mzLeft = mz
            Next

            mzLeft = 0
            rtLeft = rt
        Next
    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim matrix As MeasureData() = CreateMatrix.ToArray
        Dim app As New ContourPlot(matrix, False, theme) With {
            .legendTitle = "Scale Intensity"
        }

        Call app.Plot(g, canvas)
    End Sub

    Public Overloads Shared Function Plot(data As IEnumerable(Of ms1_scan),
                                          Optional size$ = "3600,2400",
                                          Optional padding$ = "padding: 100px 900px 250px 300px;",
                                          Optional bg$ = "white",
                                          Optional colorSet$ = "darkblue,blue,skyblue,green,orange,red,darkred") As GraphicsData
        Dim theme As New Theme With {
            .colorSet = colorSet,
            .padding = padding,
            .background = bg
        }
        Dim app As New ScanContour(data.ToArray, theme) With {
            .xlabel = "scan_time (seconds)",
            .ylabel = "M/z"
        }

        Return app.Plot(size:=size)
    End Function
End Class
