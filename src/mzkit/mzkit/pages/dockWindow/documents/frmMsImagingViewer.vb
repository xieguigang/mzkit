#Region "Microsoft.VisualBasic::bbb734cded1b830a28ec32418fc46733, src\mzkit\mzkit\pages\dockWindow\documents\frmMsImagingViewer.vb"

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
'     Function: (+2 Overloads) createRenderTask
' 
'     Sub: checks_Click, ClearPinToolStripMenuItem_Click, CopyFullPath, ExportMatrixToolStripMenuItem_Click, exportMzPack
'          frmMsImagingViewer_Closing, frmMsImagingViewer_Load, loadimzML, loadmzML, loadRaw
'          LoadRender, OpenContainingFolder, PinToolStripMenuItem_Click, PixelSelector1_SelectPixelRegion, Plot
'          renderByMzList, renderByPixelsData, renderRGB, RenderSummary, SaveDocument
'          SaveImageToolStripMenuItem_Click, showPixel, tweaks_PropertyValueChanged
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports ControlLibrary
Imports ControlLibrary.Kesoft.Windows.Forms.Win7StyleTreeView
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports mzkit.My
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmMsImagingViewer
    Implements IFileReference

    Public Property FilePath As String Implements IFileReference.FilePath

    Friend render As Drawer
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

        AddHandler RibbonEvents.ribbonItems.ButtonMSITotalIon.ExecuteEvent, Sub() Call RenderSummary(IntensitySummary.Total)
        AddHandler RibbonEvents.ribbonItems.ButtonMSIBasePeakIon.ExecuteEvent, Sub() Call RenderSummary(IntensitySummary.BasePeak)
        AddHandler RibbonEvents.ribbonItems.ButtonMSIAverageIon.ExecuteEvent, Sub() Call RenderSummary(IntensitySummary.Average)

        AddHandler RibbonEvents.ribbonItems.ButtonExportSample.ExecuteEvent, Sub() Call exportMSISampleTable()
        AddHandler RibbonEvents.ribbonItems.ButtonExportMSIMzpack.ExecuteEvent, Sub() Call exportMzPack()

        Call ApplyVsTheme(ContextMenuStrip1)
        Call PixelSelector1.ShowMessage("Mzkit MSI Viewer")
    End Sub

    Sub exportMzPack()
        If render Is Nothing Then
            Call MyApplication.host.showStatusMessage("No MSI raw data was loaded!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Using file As New SaveFileDialog With {.Filter = "mzPack(*.mzPack)|*.mzPack"}
                If file.ShowDialog = DialogResult.OK Then
                    Dim fileName As String = file.FileName

                    Call frmTaskProgress.RunAction(
                        Sub(update)
                            Using buffer As Stream = fileName.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                                Call New mzPack With {
                                    .MS = DirectCast(render.pixelReader, ReadRawPack) _
                                        .GetScans _
                                        .ToArray
                                }.Write(buffer, progress:=update)
                            End Using
                        End Sub, title:="Export mzPack data...", info:="Save mzPack!")
                    Call MessageBox.Show($"Export mzPack data at location: {vbCrLf}{fileName}!", "BioNovoGene MSI Viewer", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using
        End If
    End Sub

    Public Sub loadRaw(file As String)
        Dim getSize As New InputMSIDimension
        Dim mask As New MaskForm(MyApplication.host.Location, MyApplication.host.Size)

        If mask.ShowDialogForm(getSize) = DialogResult.OK Then
            Dim progress As New frmProgressSpinner

            Call WindowModules.viewer.Show(DockPanel)
            Call WindowModules.msImageParameters.Show(DockPanel)
            Call New Thread(
                Sub()
                    Using raw As New MSFileReader(file)
                        Dim mzpack As mzPack = raw.LoadFromXMSIRaw(getSize.Dims.SizeParser)
                        Dim render As New Drawer(mzpack)

                        Call WindowModules.viewer.Invoke(Sub() Call LoadRender(render, file))
                        Call WindowModules.viewer.Invoke(Sub() WindowModules.viewer.DockState = DockState.Document)
                    End Using

                    Call progress.Invoke(Sub() progress.Close())

                    MyApplication.host.Invoke(
                        Sub()
                            MyApplication.host.Text = $"BioNovoGene Mzkit [{WindowModules.viewer.Text} {file.FileName}]"
                        End Sub)
                End Sub).Start()

            WindowModules.msImageParameters.DockState = DockState.DockLeft

            Call progress.ShowDialog()
        Else
            Call MyApplication.host.showStatusMessage("User cancel load MSI raw data file...")
        End If
    End Sub

    Public Sub loadmzML(file As String)
        Dim getSize As New InputMSIDimension

        If getSize.ShowDialog = DialogResult.OK Then

        End If
    End Sub

    Public Sub loadimzML(file As String)
        Call MyApplication.host.showMsImaging(imzML:=file)
    End Sub

    Public Sub LoadRender(render As Drawer, filePath As String)
        If Not Me.render Is Nothing Then
            Try
                Call Me.render.Dispose()
            Catch ex As Exception

            End Try
        End If

        Me.checks = WindowModules.msImageParameters.RenderingToolStripMenuItem
        Me.render = render
        Me.params = New MsImageProperty(render)
        Me.tweaks = WindowModules.msImageParameters.PropertyGrid1
        Me.FilePath = filePath

        WindowModules.msImageParameters.viewer = Me
        WindowModules.msImageParameters.PropertyGrid1.SelectedObject = params
        WindowModules.msImageParameters.ClearIons()
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

    Private Sub showPixel(x As Integer, y As Integer) Handles PixelSelector1.SelectPixel
        If render Is Nothing Then
            Call MyApplication.host.showStatusMessage("Please load image file at first!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        Else
            If WindowModules.MSIPixelProperty.DockState = DockState.Hidden Then
                WindowModules.MSIPixelProperty.DockState = DockState.DockRight
            End If
        End If

        Dim pixel As PixelScan = render.pixelReader.GetPixel(x, y)

        If pixel Is Nothing Then
            Call MyApplication.host.showStatusMessage($"Pixels [{x}, {y}] not contains any data.", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Call WindowModules.MSIPixelProperty.SetPixel(New InMemoryPixel(x, y, {}))

            Return
        Else
            WindowModules.MSIPixelProperty.SetPixel(pixel)
        End If

        Dim ms As New LibraryMatrix With {
            .ms2 = pixel.GetMs,
            .name = $"Pixel[{x}, {y}]"
        }

        If pinedPixel Is Nothing Then
            Call MyApplication.host.mzkitTool.showMatrix(ms.ms2, $"Pixel[{x}, {y}]")
            Call MyApplication.host.mzkitTool.PlotSpectrum(ms, focusOn:=False)
        Else
            Dim handler As New CosAlignment(Tolerance.PPM(20), New RelativeIntensityCutoff(0.05))
            Dim align As AlignmentOutput = handler.CreateAlignment(ms.ms2, pinedPixel.ms2)

            align.query = New Meta With {.id = ms.name}
            align.reference = New Meta With {.id = pinedPixel.name}

            Call MyApplication.host.mzkitTool.showAlignment(align)
        End If
    End Sub

    Private Sub PixelSelector1_SelectPixelRegion(region As Rectangle) Handles PixelSelector1.SelectPixelRegion
        If render Is Nothing Then
            Call MyApplication.host.showStatusMessage("Please load image file at first!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Dim progress As New frmProgressSpinner

        Call New Thread(
            Sub()
                Call ShowRegion(region)
                Call progress.Invoke(Sub() progress.Close())
            End Sub).Start()

        Call progress.ShowDialog()
    End Sub

    Private Sub ShowRegion(region As Rectangle)
        Dim x1 As Integer = region.Left
        Dim y1 As Integer = region.Top
        Dim x2 As Integer = region.Right
        Dim y2 As Integer = region.Bottom
        Dim rangePixels As PixelScan() = render.pixelReader _
            .GetPixel(x1, y1, x2, y2) _
            .ToArray

        If Not rangePixels.IsNullOrEmpty Then
            Dim ms As New LibraryMatrix With {
                .ms2 = rangePixels _
                    .Select(Function(p) p.GetMs) _
                    .IteratesALL _
                    .ToArray _
                    .Centroid(Tolerance.DeltaMass(0.05), New RelativeIntensityCutoff(0.05)) _
                    .ToArray,
                .name = $"Pixel [{x1},{y1} ~ {x2},{y2}]"
            }

            Call MyApplication.host.Invoke(
                Sub()
                    Call MyApplication.host.mzkitTool.showMatrix(ms.ms2, ms.name)
                    Call MyApplication.host.mzkitTool.PlotSpectrum(ms, focusOn:=False)
                End Sub)
        Else
            Call MyApplication.host.showStatusMessage($"target region [{x1}, {y1}, {x2}, {y2}] not contains any data...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        End If
    End Sub

    Friend Sub RenderSummary(summary As IntensitySummary)
        If render Is Nothing Then
            Call MyApplication.host.showStatusMessage("please load MSI raw data at first!")
            Return
        Else
            Call frmTaskProgress.RunAction(
                Sub()
                    Call Invoke(Sub() rendering = Sub()
                                                      Call MyApplication.RegisterPlot(
                                                        Sub(args)
                                                            Dim image As Bitmap = render.ShowSummaryRendering(summary,, params.colors.Description, $"{params.pixel_width},{params.pixel_height}")

                                                            image = params.Smooth(image)

                                                            PixelSelector1.MSImage(New Size(params.pixel_width, params.pixel_height)) = image
                                                            PixelSelector1.BackColor = params.background
                                                        End Sub)
                                                  End Sub)
                    Call Invoke(rendering)
                End Sub, "Render MSI", $"Rendering MSI in {summary.Description} mode...")
        End If

        Call MyApplication.host.showStatusMessage("Rendering Complete!", My.Resources.preferences_system_notifications)
        Call PixelSelector1.ShowMessage($"Render MSI in {summary.Description} mode.")
    End Sub

    Friend Sub renderRGB(r As Double, g As Double, b As Double)
        Dim selectedMz As Double() = {r, g, b}.Where(Function(mz) mz > 0).ToArray
        Dim progress As New frmProgressSpinner
        Dim size As String = $"{params.pixel_width},{params.pixel_height}"

        If selectedMz.Count = 1 Then
            MyApplication.host.showStatusMessage($"Run MS-Image rendering for selected ion m/z {selectedMz(Scan0)}...")
        ElseIf selectedMz.Count > 1 Then
            MyApplication.host.showStatusMessage($"Run MS-Image rendering for {selectedMz.Count} selected ions...")
        Else
            MyApplication.host.showStatusMessage("No RGB channels was selected!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Call New Thread(
            Sub()
                Dim err As Tolerance = params.GetTolerance
                Dim pixels As PixelData() = render.LoadPixels(selectedMz.ToArray, err).ToArray

                If pixels.IsNullOrEmpty Then
                    Call MyApplication.host.showStatusMessage($"No ion hits!", My.Resources.StatusAnnotations_Warning_32xLG_color)
                Else
                    Dim maxInto As Double = Aggregate pm As PixelData
                                       In pixels
                                       Into Max(pm.intensity)
                    Dim Rpixels = pixels.Where(Function(p) err(p.mz, r)).ToArray
                    Dim Gpixels = pixels.Where(Function(p) err(p.mz, g)).ToArray
                    Dim Bpixels = pixels.Where(Function(p) err(p.mz, b)).ToArray

                    Call Invoke(Sub() params.SetIntensityMax(maxInto))
                    Call Invoke(Sub() rendering = createRenderTask(Rpixels, Gpixels, Bpixels, size, render.dimension))
                    Call Invoke(rendering)
                End If

                Call progress.Invoke(Sub() progress.Close())
            End Sub).Start()

        Call progress.ShowDialog()
        Call MyApplication.host.showStatusMessage("Rendering Complete!", My.Resources.preferences_system_notifications)
        Call PixelSelector1.ShowMessage($"Render in RGB Channel Composition Mode: {selectedMz.JoinBy(", ")}")
    End Sub

    Private Function createRenderTask(R As PixelData(), G As PixelData(), B As PixelData(), size$, dimensionSize As Size) As Action
        loadedPixels = R.JoinIterates(G).JoinIterates(B).ToArray

        Return Sub()
                   Call MyApplication.RegisterPlot(
                       Sub(args)
                           Dim image As Bitmap = Drawer.ChannelCompositions(
                               R:=R, G:=G, B:=B,
                               dimension:=dimensionSize,
                               dimSize:=size.SizeParser,
                               scale:=params.scale
                           )

                           image = params.Smooth(image)

                           PixelSelector1.MSImage(size.SizeParser) = image
                           PixelSelector1.BackColor = params.background
                       End Sub)
               End Sub
    End Function

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

                If pixels.IsNullOrEmpty Then
                    Call MyApplication.host.showStatusMessage("no pixel data...", My.Resources.StatusAnnotations_Warning_32xLG_color)
                Else
                    Dim maxInto As Double = Aggregate pm As PixelData
                                            In pixels
                                            Into Max(pm.intensity)

                    Call Invoke(Sub() params.SetIntensityMax(maxInto))
                    Call Invoke(Sub() rendering = createRenderTask(pixels, size, render.dimension))
                    Call Invoke(rendering)
                End If

                Call progress.Invoke(Sub() progress.Close())
            End Sub).Start()

        Call progress.ShowDialog()
        Call MyApplication.host.showStatusMessage("Rendering Complete!", My.Resources.preferences_system_notifications)
        Call PixelSelector1.ShowMessage($"Render in Layer Pixels Composition Mode: {selectedMz.JoinBy(", ")}")
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

        rendering = createRenderTask(pixels, $"{params.pixel_width},{params.pixel_height}", MsiDim)
        rendering()

        Call MyApplication.host.showStatusMessage("Rendering Complete!", My.Resources.preferences_system_notifications)
    End Sub

    Private Function createRenderTask(pixels As PixelData(), size$, dimensionSize As Size) As Action
        loadedPixels = pixels

        Return Sub()
                   Call MyApplication.RegisterPlot(Sub(args) Call Plot(args, pixels, size, dimensionSize))
               End Sub
    End Function

    Private Sub Plot(args As PlotProperty, pixels As PixelData(), size$, dimensionSize As Size)
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
            logE:=params.logE,
            mapLevels:=params.mapLevels,
            colorSet:=params.colors.Description,
            scale:=params.scale
        )

        image = params.Smooth(image)

        PixelSelector1.MSImage(size.SizeParser) = image
        PixelSelector1.BackColor = params.background
    End Sub

    Protected Overrides Sub OpenContainingFolder()
        Call Process.Start(FilePath.ParentPath)
    End Sub

    Protected Overrides Sub CopyFullPath()
        Call Clipboard.SetText(FilePath)
    End Sub

    Protected Overrides Sub SaveDocument()
        If PixelSelector1.MSImage Is Nothing Then
            Call MyApplication.host.showStatusMessage("No MSI plot image for output!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Using file As New SaveFileDialog With {.Filter = "PNG image(*.png)|*.png", .Title = "Save MS-Imaging Plot"}
            If file.ShowDialog = DialogResult.OK Then
                Call PixelSelector1.MSImage.SaveAs(file.FileName)
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

        If render Is Nothing Then
            WindowModules.msImageParameters.DockState = DockState.Hidden
            WindowModules.msImageParameters.checkedMz.Clear()
            WindowModules.msImageParameters.Win7StyleTreeView1.Nodes.Clear()

            Me.DockState = DockState.Hidden

            Return
        End If

        If MessageBox.Show("Going to close current MS-imaging viewer?", FilePath.FileName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
        Else
            WindowModules.msImageParameters.DockState = DockState.Hidden
            WindowModules.msImageParameters.checkedMz.Clear()
            WindowModules.msImageParameters.Win7StyleTreeView1.Nodes.Clear()

            Me.DockState = DockState.Hidden

            render.Dispose()
            render.Free
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
                    Call loadedPixels.CreateCDF(filesave, render.dimension, params.GetTolerance)
                End Using
            End If
        End Using
    End Sub

    Dim pinedPixel As LibraryMatrix

    Private Sub PinToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PinToolStripMenuItem.Click
        Dim pos As Point = PixelSelector1.Pixel

        If render Is Nothing Then
            Return
        Else
            pinedPixel = New LibraryMatrix With {
                .ms2 = render.ReadXY(pos.X, pos.Y).ToArray,
                .name = $"Select Pixel: [{pos.X},{pos.Y}]"
            }

            If pinedPixel.ms2.Length = 0 Then
                pinedPixel = Nothing
                MyApplication.host.showStatusMessage("There is no MS data in current pixel?", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Else
                Call WindowModules.msImageParameters.LoadPinnedIons(pinedPixel.ms2)
            End If
        End If
    End Sub

    Private Sub ClearPinToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearToolStripMenuItem.Click
        pinedPixel = Nothing
    End Sub

    Dim sampleRegions As New List(Of Rectangle)

    Private Sub ClearSamplesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearSamplesToolStripMenuItem.Click
        sampleRegions.Clear()
    End Sub

    Private Sub AddSampleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddSampleToolStripMenuItem.Click
        If PixelSelector1.HasRegionSelection Then
            sampleRegions.Add(PixelSelector1.RegionSelectin)
        End If
    End Sub

    Private Sub exportMSISampleTable()
        If sampleRegions.IsNullOrEmpty Then
            Call MyApplication.host.showStatusMessage("No sample dot!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Using file As New SaveFileDialog With {.Filter = "Excel Table(*.csv)|*.csv"}
                If file.ShowDialog = DialogResult.OK Then
                    Call RscriptProgressTask.CreateMSIPeakTable(sampleRegions.ToArray, mzpack:=FilePath, saveAs:=file.FileName)
                End If
            End Using
        End If
    End Sub

    Private Sub ExportPlotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportPlotToolStripMenuItem.Click

    End Sub
End Class
