#Region "Microsoft.VisualBasic::c7b803d9260281bda3b38f3b327398f9, visualize\plot\StandardCurvesPlot.vb"

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

    '   Total Lines: 69
    '    Code Lines: 61 (88.41%)
    ' Comment Lines: 5 (7.25%)
    '    - Xml Docs: 60.00%
    ' 
    '   Blank Lines: 3 (4.35%)
    '     File Size: 3.41 KB


    ' Module StandardCurvesPlot
    ' 
    '     Function: StandardCurves
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Plots
Imports Microsoft.VisualBasic.Data.Plots.Canvas
Imports Microsoft.VisualBasic.Data.Plots.Engine
Imports Microsoft.VisualBasic.Data.Plots.Legacy.Plots
Imports Microsoft.VisualBasic.Data.Plots.Statistics
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports System.Linq

''' <summary>
''' plot regression model of the MRM/SRM standard curves result
''' </summary>
Public Module StandardCurvesPlot

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function StandardCurves(model As StandardCurve,
                                   Optional samples As IEnumerable(Of NamedValue(Of Double)) = Nothing,
                                   Optional name$ = "",
                                   Optional size$ = "1600,1200",
                                   Optional margin$ = "padding: 100px 100px 100px 200px",
                                   Optional factorFormat$ = "G4",
                                   Optional sampleLabelFont$ = CSSFont.Win10Normal,
                                   Optional labelerIterations% = 1000,
                                   Optional gridFill$ = NameOf(Color.LightGray),
                                   Optional showLegend As Boolean = True,
                                   Optional showYFitPoints As Boolean = True,
                                   Optional reverse As Boolean = False) As GraphicsData

        Dim wh = size.Split(","c)
        Dim width = CInt(Val(wh(0)))
        Dim height = CInt(Val(wh(1)))

        ' 观测点优先取自标准曲线自身的参考点；否则退回到传入的 samples 参数
        Dim x As Double()
        Dim y As Double()

        If model.points IsNot Nothing AndAlso model.points.Any() Then
            If model.requireISCalibration Then
                x = model.points.Select(Function(p) p.cIS / p.Cti).ToArray
                y = model.points.Select(Function(p) p.AIS / p.Ati).ToArray
            Else
                x = model.points.Select(Function(p) p.Cti).ToArray
                y = model.points.Select(Function(p) p.Ati).ToArray
            End If
        ElseIf samples IsNot Nothing Then
            Dim pts = samples.ToArray
            x = pts.Select(Function(s) Val(s.Name)).ToArray
            y = pts.Select(Function(s) s.Value).ToArray
        Else
            x = {}
            y = {}
        End If

        Dim yfit = x.Select(Function(xi) model.linear.GetY(xi)).ToArray

        Dim fit As New RegressionFit With {
            .X = x,
            .Y = y,
            .Yfit = yfit,
            .EquationText = If(model.linear.Polynomial Is Nothing, "", model.linear.Polynomial.ToString)
        }

        Dim plotInternal = Sub(ByRef g As IGraphics, layout As GraphicsRegion)
                               Dim app As New RegressionPlot(g, New PlotTheme(margin)) With {
                                   .Fit = fit,
                                   .ShowLegend = showLegend,
                                   .ShowConfidenceBand = False,
                                   .Title = name
                               }

                               If model.requireISCalibration Then
                                   app.XLabel = "(CIS/Cti u mol/L) ratio"
                                   app.YLabel = "Peak area ratio (AIS/Ati)"
                               Else
                                   app.XLabel = "Cti u mol/L"
                                   app.YLabel = "Peak area(Ati)"
                               End If

                               app.Plot()
                           End Sub

        Dim dims = size.Split(","c)
        Dim sz As New System.Drawing.Size(CInt(Val(dims(0))), CInt(Val(dims(1))))
        Return g.GraphicsPlots(sz, margin, "white", plotInternal)
    End Function
End Module
