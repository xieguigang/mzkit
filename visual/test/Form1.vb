Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.MassSpectrum.Math.App.GCMS
Imports SMRUCC.MassSpectrum.Visualization

Public Class Form1

    Dim data As GCMSJson

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        data = QuantifyAnalysis.ReadData("D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\16s_contents\SCFA\SCFA测试标曲.AIA\250ppm.CDF")
    End Sub

    Dim file$ = "./scans.png"

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Call data.PlotScans(
            viewAngle:=$"{xText.Text},{yText.Text},{zText.Text}",
            viewDistance:=viewText.Text,
            fov:=fovText.Text
        ).AsGDIImage _
         .SaveAs(file)

        Call $"
viewAngle:={xText.Text},{yText.Text},{zText.Text}
viewDistance:={viewText.Text}
fov:={fovText.Text}".Trim.SaveTo("./args.txt")

        PictureBox1.BackgroundImage = file.LoadImage
    End Sub
End Class