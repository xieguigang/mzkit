#Region "Microsoft.VisualBasic::998594642b65f16146444903f7059118, visualize\plot\ChromatogramPlot\ScanContour.vb"

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
Imports Microsoft.VisualBasic.Data.Plots
Imports Microsoft.VisualBasic.Data.Plots.Basic
Imports Microsoft.VisualBasic.Data.Plots.Canvas
Imports Microsoft.VisualBasic.Data.Plots.Legacy.Plots
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

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
    ''' group by mz/rt and resample the scattered (rt, mz, intensity) points
    ''' onto a regular grid so the new ContourPlot can consume it as a Matrix.
    ''' </summary>
    Private Function CreateMatrix() As Double(,)
        Dim colCount = CInt(std.Round((rtRange.Max - rtRange.Min) / xsteps)) + 1
        Dim rowCount = CInt(std.Round((mzRange.Max - mzRange.Min) / ysteps)) + 1
        Dim matrix(rowCount - 1, colCount - 1) As Double

        ' 先收集所有有效栅格点，并找出最小强度作为背景填充值
        Dim points As New List(Of (col As Integer, row As Integer, z As Double))()
        Dim zmin As Double = Double.MaxValue
        Dim rtLeft As Double = 0
        Dim mzLeft As Double = 0
        Dim mzi As Double

        For rt As Double = rtRange.Min To rtRange.Max Step xsteps
            Dim rti As Double = rt
            Dim rtRow As ms1_scan() = scans _
                .Where(Function(x) x.scan_time >= rtLeft AndAlso x.scan_time < rti) _
                .ToArray

            If rtRow.Length = 0 Then
                rtLeft = rt
                Continue For
            End If

            Dim col = CInt(std.Round((rt - rtRange.Min) / xsteps))

            For mz As Double = mzRange.Min To mzRange.Max Step ysteps
                mzi = mz

                Dim bin = rtRow _
                    .Where(Function(x) x.mz >= mzLeft AndAlso x.mz < mzi) _
                    .ToArray

                If bin.Length > 0 Then
                    Dim z = bin _
                        .OrderByDescending(Function(x) x.intensity) _
                        .First _
                        .intensity _
                        .DoCall(AddressOf std.Log10)
                    Dim row = CInt(std.Round((mz - mzRange.Min) / ysteps))

                    points.Add((col, row, z))

                    If z < zmin Then zmin = z
                End If

                mzLeft = mz
            Next

            mzLeft = 0
            rtLeft = rt
        Next

        Dim background = If(zmin = Double.MaxValue, 0.0, zmin)

        For r As Integer = 0 To rowCount - 1
            For c As Integer = 0 To colCount - 1
                matrix(r, c) = background
            Next
        Next

        For Each p In points
            matrix(p.row, p.col) = p.z
        Next

        Return matrix
    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim matrix As Double(,) = CreateMatrix()
        Dim colCount = matrix.GetLength(1)
        Dim rowCount = matrix.GetLength(0)

        Dim xRange(colCount - 1) As Double
        Dim yRange(rowCount - 1) As Double

        For c As Integer = 0 To colCount - 1
            xRange(c) = rtRange.Min + c * xsteps
        Next
        For r As Integer = 0 To rowCount - 1
            yRange(r) = mzRange.Min + r * ysteps
        Next

        Dim app As New ContourPlot(g, ThemeBridge.ToPlotTheme(theme)) With {
            .Matrix = matrix,
            .XRange = xRange,
            .YRange = yRange,
            .Mode = ContourPlot.ContourMode.Filled,
            .ColorMap = ScalerPalette.viridis,
            .ShowColorLegend = True
        }

        app.Plot()
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
