#Region "Microsoft.VisualBasic::a3c88ad0bdeac9cdf79797810251fd0e, test\Form1.vb"

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

' Class Form1
' 
'     Sub: Form1_Load, renderFile, timer_Tick
' 
' /********************************************************************************/

#End Region

Imports System.Windows.Forms
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.MassSpectrum.Math.GCMS

Public Class Form1

    ' Dim data As GCMSJson
    Dim WithEvents timer As New Timer With {.Enabled = True, .Interval = 1000}
    Dim render As New Microsoft.VisualBasic.Parallel.Tasks.UpdateThread(500, AddressOf renderFile)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' data = QuantifyAnalysis.ReadData("D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\16s_contents\SCFA\SCFA测试标曲.AIA\250ppm.CDF")
        render.Start()
    End Sub

    Dim file$ = "./scans.png"

    Sub renderFile()
        '        Call data.PlotScans(size:="3000,2000",
        '    viewAngle:=$"{xTrack.Value},{yTrack.Value},{zTrack.Value}",
        '    viewDistance:=Val(viewText.Text),
        '    fov:=Val(fovText.Text)
        ').AsGDIImage _
        ' .SaveAs(file)

        '        Call $"
        'viewAngle:={xTrack.Value},{yTrack.Value},{zTrack.Value}
        'viewDistance:={viewText.Text}
        'fov:={fovText.Text}".Trim.SaveTo("./args.txt")
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
