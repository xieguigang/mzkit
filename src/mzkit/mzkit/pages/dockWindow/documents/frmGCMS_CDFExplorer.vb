#Region "Microsoft.VisualBasic::9c5002c067d9b9c698dba1775621ae8a, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmGCMS_CDFExplorer.vb"

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

    '   Total Lines: 94
    '    Code Lines: 76
    ' Comment Lines: 0
    '   Blank Lines: 18
    '     File Size: 3.41 KB


    ' Class frmGCMS_CDFExplorer
    ' 
    '     Function: loadCDF
    ' 
    '     Sub: CopyFullPath, frmGCMS_CDFExplorer_Closing, frmGCMS_CDFExplorer_Load, loadCDF, OpenContainingFolder
    '          RtRangeSelector1_RangeSelect, SetRange
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Imaging
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmGCMS_CDFExplorer

    Friend gcms As Raw

    Public Shared Function loadCDF(file As String, isBackground As Boolean) As Raw
        Dim gcms As Raw

        If file.ExtensionSuffix("cdf") Then
            If file.FileLength > 1024 * 512 AndAlso Not isBackground Then
                gcms = frmTaskProgress.LoadData(Function() netCDFReader.Open(file).ReadData(showSummary:=False), info:=file.GetFullPath)
            Else
                gcms = netCDFReader.Open(file).ReadData(showSummary:=False)
            End If
        Else
            gcms = mzMLReader.LoadFile(file)
        End If

        gcms.fileName = file.GetFullPath
        Return gcms
    End Function

    Protected Overrides Sub CopyFullPath()
        Clipboard.SetText(gcms.fileName)
    End Sub

    Protected Overrides Sub OpenContainingFolder()
        Process.Start(gcms.fileName.ParentPath)
    End Sub

    Public Sub loadCDF(gcms As Raw)
        Me.TabText = gcms.fileName.FileName
        Me.gcms = gcms

        RtRangeSelector1.SetTIC(gcms.GetTIC.value)
    End Sub

    Friend Sub RtRangeSelector1_RangeSelect(min As Double, max As Double) Handles RtRangeSelector1.RangeSelect
        Dim scan As ms1_scan() = gcms.GetMsScan(min, max)

        If scan.Length = 0 OrElse scan.All(Function(x) x.intensity = 0.0) Then
            Return
        End If

        Dim spectrum As ms2() = scan _
            .Select(Function(x)
                        Return New ms2 With {
                            .mz = x.mz,
                            .intensity = x.intensity
                        }
                    End Function) _
            .ToArray
        Dim scanData As New LibraryMatrix With {
            .ms2 = spectrum _
                .Centroid(Tolerance.ParseScript("da:0.3"), LowAbundanceTrimming.Default) _
                .ToArray,
            .name = "SIM ions"
        }
        Dim q = scanData.OrderByDescending(Function(x) x.intensity).First
        Dim title1$ = $"Scan Time [{min.ToString("F3")},{max.ToString("F3")}] sec"
        Dim title2$ = $"Quantitative [{q.mz.ToString("F4")}:{q.intensity.ToString("G3")}]"

        PictureBox1.BackgroundImage = scanData.MirrorPlot(titles:={title1, title2}).AsGDIImage
    End Sub

    Public Sub SetRange(left As Double, right As Double)
        Call RtRangeSelector1.SetRange(left, right)
    End Sub

    Private Sub frmGCMS_CDFExplorer_Load(sender As Object, e As EventArgs) Handles Me.Load
        PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
        RtRangeSelector1.BackColor = Color.LightBlue
        RtRangeSelector1.FillColor = Color.DarkBlue
        RtRangeSelector1.SelectedColor = Color.Black

        SaveDocumentToolStripMenuItem.Enabled = False

        TabText = "Targetted GCMS Viewer"
    End Sub

    Private Sub frmGCMS_CDFExplorer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.DockState = DockState.Hidden
    End Sub
End Class
