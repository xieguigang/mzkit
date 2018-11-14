Imports System.Windows.Forms
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.MassSpectrum.Math.App.GCMS
Imports SMRUCC.MassSpectrum.Visualization

Public Class Form1

    Dim data As GCMSJson
    Dim WithEvents timer As New Timer With {.Enabled = True, .Interval = 1000}
    Dim render As New Microsoft.VisualBasic.Parallel.Tasks.UpdateThread(500, AddressOf renderFile)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        data = QuantifyAnalysis.ReadData("D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\16s_contents\SCFA\SCFA测试标曲.AIA\250ppm.CDF")
        render.Start()
    End Sub

    Dim file$ = "./scans.png"

    Sub renderFile()
        Call data.PlotScans(size:="3000,2000",
    viewAngle:=$"{xTrack.Value},{yTrack.Value},{zTrack.Value}",
    viewDistance:=Val(viewText.Text),
    fov:=Val(fovText.Text)
).AsGDIImage _
 .SaveAs(file)

        Call $"
viewAngle:={xTrack.Value},{yTrack.Value},{zTrack.Value}
viewDistance:={viewText.Text}
fov:={fovText.Text}".Trim.SaveTo("./args.txt")
    End Sub

    Private Sub timer_Tick(sender As Object, e As EventArgs) Handles timer.Tick
        Try
            PictureBox1.BackgroundImage = file.LoadImage

            Call Console.Write("|")
        Catch ex As Exception
            Call Console.Write("!")
        End Try
    End Sub
End Class