#Region "Microsoft.VisualBasic::ad0fef42a41c35e3ec6d77906a9659b4, src\mzkit\mzkit\pages\dockWindow\documents\frmMsImagingViewer.vb"

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

' Class frmMsImagingViewer
' 
'     Properties: FilePath, MimeType
' 
'     Sub: checks_Click, CopyFullPath, frmMsImagingViewer_Closing, frmMsImagingViewer_Load, LoadRender
'          OpenContainingFolder, SaveDocument, tweaks_PropertyValueChanged
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports ControlLibrary.Kesoft.Windows.Forms.Win7StyleTreeView
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports mzkit.My
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmMsImagingViewer
    Implements IFileReference

    Public Property FilePath As String Implements IFileReference.FilePath

    Dim render As Drawer
    Dim params As MsImageProperty
    Dim WithEvents checks As ToolStripMenuItem
    Dim WithEvents tweaks As PropertyGrid
    Dim rendering As Action

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return {
                New ContentType With {.Details = "Image mzML", .FileExt = ".imzML", .MIMEType = "text/xml", .Name = "Image mzML"}
            }
        End Get
    End Property

    Private Sub frmMsImagingViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = Text
        WindowModules.msImageParameters.DockState = DockState.DockLeft
        PictureBox1.BackgroundImageLayout = ImageLayout.Zoom

        Call ApplyVsTheme(ContextMenuStrip1)
    End Sub

    Public Sub LoadRender(render As Drawer, filePath As String)
        Dim checks As Win7StyleTreeView = WindowModules.msImageParameters.Win7StyleTreeView1

        Me.checks = WindowModules.msImageParameters.RenderingToolStripMenuItem
        Me.render = render
        Me.params = New MsImageProperty(render)
        Me.tweaks = WindowModules.msImageParameters.PropertyGrid1
        Me.FilePath = filePath

        WindowModules.msImageParameters.PropertyGrid1.SelectedObject = params
        WindowModules.msImageParameters.Win7StyleTreeView1.Nodes.Clear()
    End Sub

    ''' <summary>
    ''' 渲染多个图层的按钮
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub checks_Click(sender As Object, e As EventArgs) Handles checks.Click
        Dim mz As Double() = WindowModules.msImageParameters _
            .GetSelectedIons _
            .Distinct _
            .ToArray

        If mz.Length = 0 Then
            Call MyApplication.host.showStatusMessage("No ions selected for rendering!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Call renderByMzList(mz)
        End If
    End Sub

    Friend Sub renderByMzList(mz As Double())
        Dim selectedMz As New List(Of Double)
        Dim progress As New frmProgressSpinner
        Dim size As String = $"{params.pixel_width},{params.pixel_height}"

        For i As Integer = 0 To mz.Length - 1
            selectedMz.Add(Val(CStr(mz(i))))
        Next

        If selectedMz.Count = 1 Then
            MyApplication.host.showStatusMessage($"Run MS-Image rendering for selected ion m/z {selectedMz(Scan0)}...")
        Else
            MyApplication.host.showStatusMessage($"Run MS-Image rendering for {selectedMz.Count} selected ions...")
        End If

        Call New Thread(
            Sub()
                Dim err As Tolerance = params.GetTolerance
                Dim pixels As PixelData() = render.LoadPixels(selectedMz.ToArray, err).ToArray
                Dim maxInto As Double = Aggregate pm As PixelData
                                        In pixels
                                        Into Max(pm.intensity)

                Call Invoke(Sub() params.SetIntensityMax(maxInto))
                Call Invoke(Sub() rendering = createRenderTask(pixels, size, render.dimension))
                Call Invoke(rendering)
                Call progress.Invoke(Sub() progress.Close())
            End Sub).Start()

        Call progress.ShowDialog()
        Call MyApplication.host.showStatusMessage("Rendering Complete!", My.Resources.preferences_system_notifications)
    End Sub

    Dim loadedPixels As PixelData()

    Public Sub renderByPixelsData(pixels As PixelData(), MsiDim As Size)
        If params Is Nothing Then
            Me.params = New MsImageProperty
            Me.checks = WindowModules.msImageParameters.RenderingToolStripMenuItem
            Me.tweaks = WindowModules.msImageParameters.PropertyGrid1
            Me.FilePath = FilePath

            WindowModules.msImageParameters.PropertyGrid1.SelectedObject = params
            WindowModules.msImageParameters.Win7StyleTreeView1.Nodes.Clear()
        End If

        Call params.SetIntensityMax(Aggregate pm As PixelData In pixels Into Max(pm.intensity))
        Call params.Reset(MsiDim, "N/A", "N/A")

        rendering = createRenderTask(pixels, "", MsiDim)
        rendering()

        Call MyApplication.host.showStatusMessage("Rendering Complete!", My.Resources.preferences_system_notifications)
    End Sub

    Private Function createRenderTask(pixels As PixelData(), size$, dimensionSize As Size) As Action
        loadedPixels = pixels

        Return Sub()
                   Call MyApplication.RegisterPlot(
                       Sub(args)
                           Dim pixelFilter As PixelData() = (
                              From pm As PixelData
                              In pixels
                              Where pm.intensity >= params.lowerbound
                              Select If(pm.intensity > params.upperbound, New PixelData With {
                                  .intensity = params.upperbound,
                                  .level = pm.level,
                                  .mz = pm.mz,
                                  .x = pm.x,
                                  .y = pm.y
                              }, pm)
                           ).ToArray

                           pixelFilter = Drawer.ScalePixels(pixelFilter, params.GetTolerance)
                           pixelFilter = Drawer.GetPixelsMatrix(pixelFilter)

                           Dim image As Bitmap = Drawer.RenderPixels(
                               pixels:=pixelFilter,
                               dimension:=dimensionSize,
                               dimSize:=size.SizeParser,
                               threshold:=params.threshold,
                               mapLevels:=params.mapLevels,
                               colorSet:=params.colors.Description,
                               scale:=params.scale
                           )

                           image = params.Smooth(image)

                           PictureBox1.BackgroundImage = image
                           PictureBox1.BackColor = params.background
                       End Sub)
               End Sub
    End Function

    Protected Overrides Sub OpenContainingFolder()
        Call Process.Start(FilePath.ParentPath)
    End Sub

    Protected Overrides Sub CopyFullPath()
        Call Clipboard.SetText(FilePath)
    End Sub

    Protected Overrides Sub SaveDocument()
        Using file As New SaveFileDialog With {.Filter = "PNG image(*.png)|*.png", .Title = "Save MS-Imaging Plot"}
            If file.ShowDialog = DialogResult.OK Then
                Call PictureBox1.BackgroundImage.SaveAs(file.FileName)
            End If
        End Using
    End Sub

    Private Sub tweaks_PropertyValueChanged(s As Object, e As PropertyValueChangedEventArgs) Handles tweaks.PropertyValueChanged
        If Not rendering Is Nothing Then
            Call rendering()
        Else
            Call MyApplication.host.showStatusMessage("No image for render...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        End If
    End Sub

    Private Sub frmMsImagingViewer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True

        If MessageBox.Show("Going to close current MS-imaging viewer?", FilePath.FileName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
        Else
            WindowModules.msImageParameters.DockState = DockState.Hidden
            Me.DockState = DockState.Hidden
        End If
    End Sub

    Private Sub SaveImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveImageToolStripMenuItem.Click
        Call SaveDocument()
    End Sub

    Private Sub ExportMatrixToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportMatrixToolStripMenuItem.Click
        If loadedPixels.IsNullOrEmpty Then
            Call MyApplication.host.showStatusMessage("No loaded pixels data...", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Using file As New SaveFileDialog With {.Filter = "NetCDF(*.cdf)|*.cdf", .Title = "Save MS-Imaging Matrix"}
            If file.ShowDialog = DialogResult.OK Then
                Using filesave As FileStream = file.FileName.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                    Call loadedPixels.CreateCDF(filesave, render.dimension)
                End Using
            End If
        End Using
    End Sub
End Class
