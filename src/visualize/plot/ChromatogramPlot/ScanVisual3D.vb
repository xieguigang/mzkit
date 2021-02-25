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

    ReadOnly scans As (mz#, scan As ms1_scan())()
    ReadOnly height As Double = 0.45
    ReadOnly angle As Double = 60
    ReadOnly fillCurve As Boolean
    ReadOnly fillAlpha As Integer

    Public Sub New(points As IEnumerable(Of ms1_scan), tolerance As Tolerance, theme As Theme)
        MyBase.New(theme)

        scans = points _
            .GroupBy(Function(p) p.mz, tolerance) _
            .Select(Function(mz)
                        Return (Val(mz.name), mz.OrderBy(Function(t) t.scan_time).ToArray)
                    End Function) _
            .OrderBy(Function(mz) mz.Item1) _
            .ToArray
    End Sub

    Private Function evalDx() As Double

    End Function

    Private Function evalDy() As Double

    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim rect As Rectangle = canvas.PlotRegion
        Dim height As Double = Me.height * rect.Height
        Dim dx As Double = evalDx()
        Dim dy As Double = evalDy()
        Dim TIC As NamedCollection(Of ChromatogramTick)

        For i As Integer = 0 To scans.Length - 1
            canvas = canvas.Offset2D(dx * i, dy * i)
            TIC = New NamedCollection(Of ChromatogramTick) With {
                .name = scans(i).mz,
                .value = scans(i).scan _
                    .Select(Function(t)
                                Return New ChromatogramTick With {
                                    .Time = t.scan_time,
                                    .Intensity = t.intensity
                                }
                            End Function) _
                    .ToArray
            }

            Call New TICplot(
                ionData:={TIC},
                timeRange:=Nothing,
                intensityMax:=0,
                isXIC:=False,
                parallel:=False,
                fillCurve:=fillCurve,
                fillAlpha:=fillAlpha,
                labelLayoutTicks:=-1,
                deln:=10,
                theme:=theme
            ).Plot(g, canvas.PlotRegion)
        Next
    End Sub
End Class
