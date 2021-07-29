Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Quantile
Imports Task

Public Class MSIPixelPropertyWindow



    Public Sub SetPixel(pixel As PixelScan)
        PropertyGrid1.SelectedObject = New PixelProperty(pixel)

        Dim q As QuantileEstimationGK = pixel.GetMs.Select(Function(i) i.intensity).GKQuantile
        Dim serial As New SerialData With {
            .color = Color.Blue,
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

        PictureBox1.BackgroundImage = {serial}.Plot(fill:=True).AsGDIImage
        PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
    End Sub

    Private Sub MSIPixelPropertyWindow_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "MSI Pixel Properties"
    End Sub
End Class