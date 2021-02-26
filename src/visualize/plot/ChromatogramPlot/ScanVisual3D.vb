#Region "Microsoft.VisualBasic::dc20a3c1f3007202a253b2266ce29547, src\visualize\plot\GCMSscanVisual.vb"

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

' Module GCMSscanVisual
' 
'     Function: PlotScans
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Plot3D.Device
Imports Microsoft.VisualBasic.Data.ChartPlots.Plot3D.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Imaging.Drawing3D.Models
Imports Microsoft.VisualBasic.Imaging.Drawing3D.Models.Isometric
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports stdNum = System.Math

' intensity
'  ^ y   z
'  |   / m/z
'  |  /  
'  | /
'  |/          retention time
'  ------------------> X 

Public Class ScanVisual3D : Inherits Plot

    ReadOnly scans As NamedCollection(Of ChromatogramTick)()
    ReadOnly height As Double = 0.45
    ReadOnly angle As Double = 60
    ReadOnly fillCurve As Boolean
    ReadOnly fillAlpha As Integer

#Region "constructor"
    Public Sub New(scans As IEnumerable(Of ms1_scan), tolerance As Tolerance, angle As Double, theme As Theme)
        Call Me.New(GetScanCollection(scans, tolerance), angle, theme)
    End Sub

    Public Sub New(scans As IEnumerable(Of NamedCollection(Of ChromatogramTick)), angle As Double, theme As Theme)
        MyBase.New(theme)

        Me.angle = angle
        Me.scans = scans.ToArray
    End Sub
#End Region

    Private Shared Iterator Function GetScanCollection(points As IEnumerable(Of ms1_scan), tolerance As Tolerance) As IEnumerable(Of NamedCollection(Of ChromatogramTick))
        For Each scan In points _
             .GroupBy(Function(p) p.mz, tolerance) _
             .Select(Function(mz)
                         Return (Val(mz.name), mz.OrderBy(Function(t) t.scan_time).ToArray)
                     End Function) _
             .OrderBy(Function(mz) mz.Item1)

            Yield New NamedCollection(Of ChromatogramTick) With {
                .name = scan.Item1.ToString("F4"),
                .value = scan.Item2 _
                    .Select(Function(t)
                                Return New ChromatogramTick With {
                                    .Intensity = t.intensity,
                                    .Time = t.scan_time
                                }
                            End Function) _
                    .ToArray
            }
        Next
    End Function

    Private Function evalDx(canvas As GraphicsRegion) As Double
        ' cos(a) = dx / dc
        ' dx = cos(a) * dc
        Dim dc As Double = evalDc(canvas)
        Dim dx As Double = stdNum.Cos(d:=angle.ToRadians) * dc

        Return dx
    End Function

    Private Function evalDc(canvas As GraphicsRegion) As Double
        Dim rect As Rectangle = canvas.PlotRegion
        Dim height As Double = Me.height * rect.Height
        Dim y1 As Double = rect.Bottom - height
        Dim y2 As Double = rect.Top
        Dim dh As Double = y1 - y2

        ' tan(a) = dh / dw
        ' dw = dh / tan(a)

        Dim dw As Double = dh / stdNum.Tan(a:=angle.ToRadians)

        ' c/|
        ' /a|
        ' ---
        ' dw

        ' cos(a) = dw / c
        ' c = dw / cos(a)

        Dim c As Double = dw / stdNum.Cos(d:=angle.ToRadians)
        Dim dc As Double = c / (scans.Length + 1)

        Return dc
    End Function

    Private Function evalDy(canvas As GraphicsRegion) As Double
        ' sin(a) = dy / dc
        ' dy = sin(a) * dc
        Dim dc As Double = evalDc(canvas)
        Dim dy As Double = stdNum.Sin(a:=angle.ToRadians) * dc

        Return dy
    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim dx As Double = evalDx(canvas)
        Dim dy As Double = evalDy(canvas)
        Dim theme As Theme = Me.theme.Clone
        Dim parallelCanvas As New GraphicsRegion With {
            .Size = canvas.Size,
            .Padding = New Padding With {
                .Top = canvas.Padding.Top,
                .Left = canvas.Padding.Left * 2,
                .Right = canvas.Padding.Right,
                .Bottom = canvas.Padding.Bottom + Me.height * canvas.PlotRegion.Height
            }
        }

        theme.background = "transparent"
        theme.gridFill = "transparent"
        theme.drawGrid = False
        theme.drawLegend = False
        theme.drawAxis = False

        For i As Integer = 0 To scans.Length - 1
            parallelCanvas = parallelCanvas.Offset2D(-dx, dy)

            Call New TICplot(
                ionData:={scans(i)},
                timeRange:=Nothing,
                intensityMax:=0,
                isXIC:=False,
                parallel:=False,
                fillCurve:=fillCurve,
                fillAlpha:=fillAlpha,
                labelLayoutTicks:=-1,
                deln:=10,
                theme:=theme
            ).Plot(g, parallelCanvas.PlotRegion)
        Next
    End Sub
End Class
