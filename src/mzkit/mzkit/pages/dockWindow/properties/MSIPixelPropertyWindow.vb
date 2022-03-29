#Region "Microsoft.VisualBasic::013e5f6d13b03f6bd3a820d1f06beca5, mzkit\src\mzkit\mzkit\pages\dockWindow\properties\MSIPixelPropertyWindow.vb"

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

    '   Total Lines: 57
    '    Code Lines: 51
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 2.27 KB


    ' Class MSIPixelPropertyWindow
    ' 
    '     Sub: MSIPixelPropertyWindow_Load, SetPixel
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Shapes
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Quantile
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Public Class MSIPixelPropertyWindow

    Public Sub SetPixel(pixel As PixelScan, ByRef props As PixelProperty)
        Select Case Me.DockState
            Case DockState.DockBottomAutoHide, DockState.DockLeftAutoHide, DockState.DockRightAutoHide, DockState.DockTopAutoHide, DockState.Hidden, DockState.Unknown
                props = New PixelProperty(pixel)
                Return
            Case Else
        End Select

        PropertyGrid1.SelectedObject = New PixelProperty(pixel)
        props = PropertyGrid1.SelectedObject

        Dim q As QuantileEstimationGK = pixel.GetMs.Select(Function(i) i.intensity).GKQuantile
        Dim serial As New SerialData With {
            .color = Color.SteelBlue,
            .lineType = DashStyle.Dash,
            .pointSize = 10,
            .shape = LegendStyles.Triangle,
            .title = "Intensity",
            .width = 5,
            .pts = seq(0, 1, 0.1) _
                .Select(Function(lv)
                            Return New PointData(lv, q.Query(lv))
                        End Function) _
                .ToArray
        }
        Dim Q2line As New Line(New PointF(0, q.Query(0.5)), New PointF(1, q.Query(0.5)), New Pen(Color.Red, 10))

        If DirectCast(PropertyGrid1.SelectedObject, PixelProperty).NumOfIons = 0 Then
            PictureBox1.BackgroundImage = Nothing
        Else
            PictureBox1.BackgroundImage = {serial}.Plot(
                size:="900,600",
                padding:="padding:50px 50px 100px 200px;",
                fill:=True,
                ablines:={Q2line},
                YtickFormat:="G2",
                XtickFormat:="F1"
            ).AsGDIImage
        End If
    End Sub

    Private Sub MSIPixelPropertyWindow_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "MSI Pixel Properties"
        PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
    End Sub
End Class
