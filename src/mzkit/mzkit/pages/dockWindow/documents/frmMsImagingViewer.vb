#Region "Microsoft.VisualBasic::75e280eada08208a035f6f6cda02f0f4, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmMsImagingViewer.vb"

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

'   Total Lines: 830
'    Code Lines: 652
' Comment Lines: 23
'   Blank Lines: 155
'     File Size: 36.57 KB


' Class frmMsImagingViewer
' 
'     Properties: FilePath, MimeType
' 
'     Function: (+2 Overloads) createRenderTask, registerSummaryRender
' 
'     Sub: AddSampleToolStripMenuItem_Click, checks_Click, cleanBackground, ClearPinToolStripMenuItem_Click, ClearSamplesToolStripMenuItem_Click
'          CopyFullPath, CopyImageToolStripMenuItem_Click, (+2 Overloads) DoIonStats, ExportMatrixToolStripMenuItem_Click, exportMSISampleTable
'          exportMzPack, ExportPlotToolStripMenuItem_Click, frmMsImagingViewer_Closing, frmMsImagingViewer_Load, ImageProcessingToolStripMenuItem_Click
'          loadimzML, loadmzML, loadRaw, (+2 Overloads) LoadRender, MSIFeatureDetections
'          OpenContainingFolder, PinToolStripMenuItem_Click, PixelSelector1_SelectPixelRegion, PixelSelector1_SelectPolygon, Plot
'          renderByMzList, renderByPixelsData, renderRGB, RenderSummary, SaveDocument
'          SaveImageToolStripMenuItem_Click, setupPolygonEditorButtons, showPixel, ShowRegion, TogglePolygonMode
'          tweaks_PropertyValueChanged
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.mzkit_win32.My
Imports ControlLibrary
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Task
Imports WeifenLuo.WinFormsUI.Docking
Imports stdNum = System.Math

Public Class frmMsImagingViewer
    Implements IFileReference

    Public Property FilePath As String Implements IFileReference.FilePath

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
        AddHandler RibbonEvents.ribbonItems.ButtonTogglePolygon.ExecuteEvent, Sub() Call TogglePolygonMode()
        AddHandler RibbonEvents.ribbonItems.ButtonMSICleanBackground.ExecuteEvent, Sub() Call cleanBackground()
        AddHandler RibbonEvents.ribbonItems.ButtonMSIRawIonStat.ExecuteEvent, Sub() Call DoIonStats()

        Call ApplyVsTheme(ContextMenuStrip1)
        Call setupPolygonEditorButtons()
        Call PixelSelector1.ShowMessage("BioNovoGene MZKit MSImaging Viewer")
    End Sub

    Sub MSIFeatureDetections()

    End Sub

    Sub DoIonStats()
        Dim progress As New frmProgressSpinner

        Call New Thread(
            Sub()
                Call Thread.Sleep(500)

                Dim ions As IonStat() = ServiceHub.DoIonStats

                If ions.IsNullOrEmpty Then
                    Call MyApplication.host.warning("No ions result...")
                Else
                    Call Me.Invoke(Sub() Call DoIonStats(ions))
                End If

                Call progress.CloseWindow()
            End Sub).Start()

        Call progress.ShowDialog()
    End Sub

    Sub DoIonStats(ions As IonStat())
        Dim table As frmTableViewer = VisualStudio.ShowDocument(Of frmTableViewer)

        table.ViewRow = Sub(row)
                            Call renderByMzList({Val(row("mz"))})
                            Call Me.Activate()
                        End Sub
        table.LoadTable(
            Sub(grid)
                Call grid.Columns.Add("mz", GetType(Double))
                Call grid.Columns.Add("pixels", GetType(Integer))
                Call grid.Columns.Add("density", GetType(Double))
                Call grid.Columns.Add("maxIntensity", GetType(Double))
                Call grid.Columns.Add("basePixel.X", GetType(Integer))
                Call grid.Columns.Add("basePixel.Y", GetType(Integer))
                Call grid.Columns.Add("Q1_intensity", GetType(Double))
                Call grid.Columns.Add("Q2_intensity", GetType(Double))
                Call grid.Columns.Add("Q3_intensity", GetType(Double))
                Call grid.Columns.Add("RSD", GetType(Double))

                For Each ion As IonStat In ions.OrderByDescending(Function(i) i.pixels)
                    Call grid.Rows.Add(
                        ion.mz.ToString("F4"),
                        ion.pixels,
                        ion.density.ToString("F2"),
                        stdNum.Round(ion.maxIntensity),
                        ion.basePixelX,
                        ion.basePixelY,
                        stdNum.Round(ion.Q1Intensity),
                        stdNum.Round(ion.Q2Intensity),
                        stdNum.Round(ion.Q3Intensity),
                        stdNum.Round(ion.RSD)
                    )

                    Call Application.DoEvents()
                Next
            End Sub)
    End Sub

    Sub setupPolygonEditorButtons()
        AddHandler ribbonItems.ButtonMovePolygon.ExecuteEvent,
            Sub()
                ribbonItems.ButtonPolygonEditorMoveVertex.BooleanValue = False
                ribbonItems.ButtonAddNewPolygon.BooleanValue = False
                ribbonItems.ButtonPolygonDeleteVertex.BooleanValue = False
                ribbonItems.ButtonRemovePolygon.BooleanValue = False

                PixelSelector1.OnMovePolygonMenuItemClick()
            End Sub
        AddHandler ribbonItems.ButtonPolygonEditorMoveVertex.ExecuteEvent,
            Sub()
                ribbonItems.ButtonMovePolygon.BooleanValue = False
                ribbonItems.ButtonAddNewPolygon.BooleanValue = False
                ribbonItems.ButtonPolygonDeleteVertex.BooleanValue = False
                ribbonItems.ButtonRemovePolygon.BooleanValue = False

                PixelSelector1.OnMoveComponentMenuItemClick()
            End Sub
        AddHandler ribbonItems.ButtonAddNewPolygon.ExecuteEvent,
            Sub()
                ribbonItems.ButtonMovePolygon.BooleanValue = False
                ribbonItems.ButtonPolygonEditorMoveVertex.BooleanValue = False
                ribbonItems.ButtonPolygonDeleteVertex.BooleanValue = False
                ribbonItems.ButtonRemovePolygon.BooleanValue = False

                PixelSelector1.OnAddVertexMenuItemClick()
            End Sub

        AddHandler ribbonItems.ButtonClosePolygonEditor.ExecuteEvent,
            Sub()
                ribbonItems.ButtonMovePolygon.BooleanValue = False
                ribbonItems.ButtonPolygonEditorMoveVertex.BooleanValue = False
                ribbonItems.ButtonAddNewPolygon.BooleanValue = False
                ribbonItems.ButtonPolygonDeleteVertex.BooleanValue = False
                ribbonItems.ButtonRemovePolygon.BooleanValue = False
                ribbonItems.ButtonTogglePolygon.BooleanValue = False

                Call MyApplication.host.Ribbon1.SetModes(0)
                Call MyApplication.host.showStatusMessage("Exit polygon editor!")

                PixelSelector1.SelectPolygonMode = False
                PixelSelector1.Cursor = Cursors.Cross
            End Sub

        AddHandler ribbonItems.ButtonPolygonDeleteVertex.ExecuteEvent,
            Sub()
                ribbonItems.ButtonMovePolygon.BooleanValue = False
                ribbonItems.ButtonPolygonEditorMoveVertex.BooleanValue = False
                ribbonItems.ButtonAddNewPolygon.BooleanValue = False
                ribbonItems.ButtonRemovePolygon.BooleanValue = False

                PixelSelector1.OnRemoveVertexMenuItemClick()
            End Sub

        AddHandler ribbonItems.ButtonRemovePolygon.ExecuteEvent,
            Sub()
                ribbonItems.ButtonMovePolygon.BooleanValue = False
                ribbonItems.ButtonPolygonEditorMoveVertex.BooleanValue = False
                ribbonItems.ButtonAddNewPolygon.BooleanValue = False
                ribbonItems.ButtonPolygonDeleteVertex.BooleanValue = False

                PixelSelector1.OnRemovePolygonMenuItemClick()
            End Sub

        AddHandler ribbonItems.ButtonShowPolygonVertexInfo.ExecuteEvent,
            Sub()
                PixelSelector1.ShowPointInform = ribbonItems.ButtonShowPolygonVertexInfo.BooleanValue

                If PixelSelector1.ShowPointInform Then
                    Call MyApplication.host.showStatusMessage("Turn on display point information of polygon vertex.")
                Else
                    Call MyApplication.host.showStatusMessage("Hide point information of polygon vertex!")
                End If
            End Sub

        Call MyApplication.host.Ribbon1.SetModes(0)
    End Sub

    Sub TogglePolygonMode()
        PixelSelector1.SelectPolygonMode = RibbonEvents.ribbonItems.ButtonTogglePolygon.BooleanValue

        If PixelSelector1.SelectPolygonMode Then
            Call MyApplication.host.Ribbon1.SetModes(1)
            Call MyApplication.host.showStatusMessage("Toggle edit polygon for your MS-imaging data!")

            PixelSelector1.Cursor = Cursors.Default
        Else
            Call MyApplication.host.Ribbon1.SetModes(0)
            Call MyApplication.host.showStatusMessage("Exit polygon editor!")

            PixelSelector1.Cursor = Cursors.Cross
        End If
    End Sub

    Private Sub PixelSelector1_SelectPolygon(polygon() As PointF) Handles PixelSelector1.SelectPolygon
        PixelSelector1.SelectPolygonMode = False
    End Sub

    Sub exportMzPack()
        If Not ServiceHub.MSIEngineRunning Then
            Call MyApplication.host.showStatusMessage("No MSI raw data was loaded!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Using file As New SaveFileDialog With {.Filter = "mzPack(*.mzPack)|*.mzPack"}
            If file.ShowDialog = DialogResult.OK Then
                Dim fileName As String = file.FileName

                Call frmTaskProgress.RunAction(
                    Sub(update)
                        ServiceHub.MessageCallback = update
                        ServiceHub.ExportMzpack(savefile:=fileName)
                    End Sub, title:="Export mzPack data...", info:="Save mzPack!")
                Call MessageBox.Show($"Export mzPack data at location: {vbCrLf}{fileName}!", "BioNovoGene MSI Viewer", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ServiceHub.MessageCallback = Nothing
            End If
        End Using
    End Sub

    ''' <summary>
    ''' load thermo raw
    ''' </summary>
    ''' <param name="file"></param>
    Public Sub loadRaw(file As String)
        Dim getSize As New InputMSIDimension
        Dim mask As New MaskForm(MyApplication.host.Location, MyApplication.host.Size)

        If mask.ShowDialogForm(getSize) = DialogResult.OK Then
            Dim progress As New frmProgressSpinner

            Call WindowModules.viewer.Show(DockPanel)
            Call WindowModules.msImageParameters.Show(DockPanel)
            Call ServiceHub.StartMSIService()

            Call New Thread(
                Sub()
                    Dim info As MsImageProperty = ServiceHub.LoadMSI(file, getSize.Dims.SizeParser, Sub(msg) MyApplication.host.showStatusMessage(msg))

                    Call WindowModules.viewer.Invoke(Sub() Call LoadRender(info, file))
                    Call WindowModules.viewer.Invoke(Sub() WindowModules.viewer.DockState = DockState.Document)

                    Call progress.CloseWindow()

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

    ''' <summary>
    ''' load mzpack into MSI engine services
    ''' </summary>
    ''' <param name="filePath"></param>
    Public Sub LoadRender(mzpack As String, filePath As String)
        Call frmTaskProgress.LoadData(
            Function(msg As Action(Of String))
                Call ServiceHub.StartMSIService()
                Call Me.Invoke(Sub() LoadRender(ServiceHub.LoadMSI(mzpack, msg), filePath))

                Return 0
            End Function)
    End Sub

    Sub cleanBackground()
        If Me.FilePath.StringEmpty Then
            Call MyApplication.host.warning("Load MS-imaging raw data at first!")
        Else
            Dim filePath = Me.FilePath

            Call frmTaskProgress.LoadData(
                Function(msg As Action(Of String))
                    Dim info = ServiceHub.CutBackground

                    Call Me.Invoke(Sub() LoadRender(info, filePath))
                    Call Me.Invoke(Sub() RenderSummary(IntensitySummary.BasePeak))

                    Return 0
                End Function)
        End If
    End Sub

    ''' <summary>
    ''' load mzpack into MSI engine services
    ''' </summary>
    ''' <param name="filePath"></param>
    Public Sub LoadRender(info As MsImageProperty, filePath As String)
        If info Is Nothing Then
            Return
        End If

        Me.checks = WindowModules.msImageParameters.RenderingToolStripMenuItem
        Me.params = info
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
        If Not ServiceHub.MSIEngineRunning Then
            Call MyApplication.host.showStatusMessage("Please load image file at first!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        Else
            If WindowModules.MSIPixelProperty.DockState = DockState.Hidden Then
                WindowModules.MSIPixelProperty.DockState = DockState.DockRight
            End If
        End If

        Dim pixel As PixelScan = ServiceHub.GetPixel(x, y)
        Dim info As PixelProperty = Nothing

        If pixel Is Nothing Then
            Call MyApplication.host.showStatusMessage($"Pixels [{x}, {y}] not contains any data.", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Call WindowModules.MSIPixelProperty.SetPixel(New InMemoryPixel(x, y, {}), info)
            Call PixelSelector1.ShowMessage($"Pixels [{x}, {y}] not contains any data.")

            Return
        Else
            Call WindowModules.MSIPixelProperty.SetPixel(pixel, info)
            Call PixelSelector1.ShowMessage($"Select {pixel.scanId}, totalIons: {info.TotalIon.ToString("G3")}, basePeak m/z: {info.TopIonMz.ToString("F4")}")
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

            Call MyApplication.host.mzkitTool.showAlignment(align, showScore:=True)
        End If
    End Sub

    Private Sub PixelSelector1_SelectPixelRegion(region As Rectangle) Handles PixelSelector1.SelectPixelRegion
        If Not ServiceHub.MSIEngineRunning Then
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
        Dim rangePixels As InMemoryVectorPixel() = ServiceHub _
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
        If Not ServiceHub.MSIEngineRunning Then
            Call MyApplication.host.showStatusMessage("please load MSI raw data at first!")
            Return
        Else
            Call frmTaskProgress.RunAction(
                Sub()
                    Call Invoke(Sub() rendering = registerSummaryRender(summary))
                    Call Invoke(rendering)
                End Sub, "Render MSI", $"Rendering MSI in {summary.Description} mode...")
        End If

        Call MyApplication.host.showStatusMessage("Rendering Complete!", My.Resources.preferences_system_notifications)
        Call PixelSelector1.ShowMessage($"Render MSI in {summary.Description} mode.")
    End Sub

    Private Function registerSummaryRender(summary As IntensitySummary) As Action
        Dim summaryLayer As PixelScanIntensity() = ServiceHub.LoadSummaryLayer(summary).KnnFill(6, 6).ToArray
        Dim dimSize As New Size(params.scan_x, params.scan_y)
        Dim range As DoubleRange = summaryLayer.Select(Function(i) i.totalIon).Range

        Return Sub()
                   Call MyApplication.RegisterPlot(
                       Sub(args)
                           Dim dotSize As New Size(2, 2)
                           Dim mapLevels As Integer = params.mapLevels
                           Dim image As Image = Drawer.RenderSummaryLayer(
                               layer:=summaryLayer,
                               dimension:=dimSize,
                               colorSet:=params.colors.Description,
                               pixelSize:=$"{dotSize.Width},{dotSize.Height}",
                               mapLevels:=mapLevels
                           ).AsGDIImage
                           Dim legend As Image = Nothing ' If(ShowLegendToolStripMenuItem.Checked, params.RenderingColorMapLegend(summaryLayer), Nothing)

                           PixelSelector1.SetMsImagingOutput(image, dotSize, params.colors, {range.Min, range.Max}, mapLevels)
                           PixelSelector1.BackColor = params.background
                           PixelSelector1.SetColorMapVisible(visible:=params.showColorMap)
                       End Sub)
               End Sub
    End Function

    Friend Sub renderRGB(r As Double, g As Double, b As Double)
        Dim selectedMz As Double() = {r, g, b}.Where(Function(mz) mz > 0).ToArray
        Dim progress As New frmProgressSpinner

        If params Is Nothing Then
            Call MyApplication.host.warning("No MS-imaging data is loaded yet!")
            Return
        End If

        Dim size As String = $"{params.pixel_width},{params.pixel_height}"

        If selectedMz.Count = 1 Then
            MyApplication.host.showStatusMessage($"Run MS-Image rendering for selected ion m/z {selectedMz(Scan0)}...")
        ElseIf selectedMz.Count > 1 Then
            MyApplication.host.showStatusMessage($"Run MS-Image rendering for {selectedMz.Count} selected ions...")
        Else
            MyApplication.host.showStatusMessage("No RGB channels was selected!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        mzdiff = params.GetTolerance
        targetMz = selectedMz

        Call New Thread(
            Sub()
                Dim pixels As PixelData() = ServiceHub.LoadPixels(selectedMz, mzdiff)

                If pixels.IsNullOrEmpty Then
                    Call MyApplication.host.showStatusMessage($"No ion hits!", My.Resources.StatusAnnotations_Warning_32xLG_color)
                Else
                    Dim maxInto As Double = Aggregate pm As PixelData
                                            In pixels
                                            Into Max(pm.intensity)
                    Dim Rpixels = pixels.Where(Function(p) mzdiff(p.mz, r)).ToArray
                    Dim Gpixels = pixels.Where(Function(p) mzdiff(p.mz, g)).ToArray
                    Dim Bpixels = pixels.Where(Function(p) mzdiff(p.mz, b)).ToArray

                    Call Invoke(Sub() params.SetIntensityMax(maxInto))
                    Call Invoke(Sub() rendering = createRenderTask(Rpixels, Gpixels, Bpixels, size))
                    Call Invoke(rendering)
                End If

                Call progress.Invoke(Sub() progress.Close())
            End Sub).Start()

        Call progress.ShowDialog()
        Call MyApplication.host.showStatusMessage("Rendering Complete!", My.Resources.preferences_system_notifications)
        Call PixelSelector1.ShowMessage($"Render in RGB Channel Composition Mode: {selectedMz.Select(Function(d) stdNum.Round(d, 4)).JoinBy(", ")}")
    End Sub

    Private Function createRenderTask(R As PixelData(), G As PixelData(), B As PixelData(), pixelSize$) As Action
        Dim dimensionSize As New Size(params.scan_x, params.scan_y)
        Dim threshold As IThreshold = RibbonEvents.GetQuantizationThreshold

        loadedPixels = R.JoinIterates(G).JoinIterates(B).ToArray

        Return Sub()
                   Call MyApplication.RegisterPlot(
                       Sub(args)
                           Dim drawer As New PixelRender(heatmapRender:=False)
                           Dim qr As Double = threshold(R.Select(Function(p) p.intensity).ToArray, params.maxCut)
                           Dim qg As Double = threshold(G.Select(Function(p) p.intensity).ToArray, params.maxCut)
                           Dim qb As Double = threshold(B.Select(Function(p) p.intensity).ToArray, params.maxCut)

                           Dim dotSize As Size = New Size(2, 2)   ' pixelSize.SizeParser
                           Dim image As Image = drawer.ChannelCompositions(
                               R:=R, G:=G, B:=B,
                               dimension:=dimensionSize,
                               dimSize:=dotSize,
                               scale:=params.scale,
                               cut:=(New DoubleRange(0, qr), New DoubleRange(0, qg), New DoubleRange(0, qb)),
                               background:=params.background.ToHtmlColor
                           ).AsGDIImage
                           Dim legend As Image = Nothing

                           PixelSelector1.SetMsImagingOutput(image, dotSize, Nothing, Nothing, Nothing)
                           PixelSelector1.BackColor = params.background
                           PixelSelector1.SetColorMapVisible(visible:=params.showColorMap)
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

        mzdiff = params.GetTolerance
        targetMz = selectedMz.ToArray

        Call New Thread(
            Sub()
                Dim pixels As PixelData() = ServiceHub.LoadPixels(selectedMz, mzdiff)

                If pixels.IsNullOrEmpty Then
                    Call MyApplication.host.showStatusMessage("no pixel data...", My.Resources.StatusAnnotations_Warning_32xLG_color)
                Else
                    Dim maxInto As Double = Aggregate pm As PixelData
                                            In pixels
                                            Into Max(pm.intensity)

                    Call Invoke(Sub() params.SetIntensityMax(maxInto))
                    Call Invoke(Sub() rendering = createRenderTask(pixels, size))
                    Call Invoke(rendering)
                End If

                Call progress.Invoke(Sub() progress.Close())
            End Sub).Start()

        Call progress.ShowDialog()
        Call MyApplication.host.showStatusMessage("Rendering Complete!", My.Resources.preferences_system_notifications)
        Call PixelSelector1.ShowMessage($"Render in Layer Pixels Composition Mode: {selectedMz.Select(Function(d) stdNum.Round(d, 4)).JoinBy(", ")}")
    End Sub

    Dim loadedPixels As PixelData()
    Dim targetMz As Double()
    Dim mzdiff As Tolerance

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

        rendering = createRenderTask(pixels, $"{params.pixel_width},{params.pixel_height}")
        rendering()

        Call MyApplication.host.showStatusMessage("Rendering Complete!", My.Resources.preferences_system_notifications)
    End Sub

    Private Function createRenderTask(pixels As PixelData(), size$) As Action
        loadedPixels = pixels

        Return Sub()
                   Call MyApplication.RegisterPlot(Sub(args) Call Plot(args, pixels, size))
               End Sub
    End Function

    Private Sub Plot(args As PlotProperty, pixels As PixelData(), size$)
        Dim dimensionSize As New Size(params.scan_x, params.scan_y)
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

        'If params.densityCut > 0 Then
        '    pixelFilter = pixelFilter _
        '        .DensityCut(qcut:=params.densityCut) _
        '        .ToArray
        'End If

        pixelFilter = MsImaging.Drawer.ScalePixels(pixelFilter, params.GetTolerance, cut:={0, 1})
        pixelFilter = MsImaging.Drawer.GetPixelsMatrix(pixelFilter)
        size = "2,2"

        Dim range As DoubleRange = pixelFilter.Select(Function(i) i.intensity).Range
        Dim drawer As New PixelRender(heatmapRender:=False)
        Dim image As Image = drawer.RenderPixels(
            pixels:=pixelFilter,
            dimension:=dimensionSize,
            dimSize:=size.SizeParser,
            mapLevels:=params.mapLevels,
            colorSet:=params.colors.Description,
            scale:=params.scale
        ).AsGDIImage
        Dim legend As Image = Nothing ' If(ShowLegendToolStripMenuItem.Checked, params.RenderingColorMapLegend(pixelFilter), Nothing)

        PixelSelector1.SetMsImagingOutput(image, size.SizeParser, params.colors, {range.Min, range.Max}, params.mapLevels)
        PixelSelector1.BackColor = params.background
        PixelSelector1.SetColorMapVisible(visible:=params.showColorMap)
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

        If Not ServiceHub.MSIEngineRunning Then
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

            ServiceHub.CloseMSIEngine()
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

        Dim dimension As New Size(params.scan_x, params.scan_y)

        Using file As New SaveFileDialog With {.Filter = "NetCDF(*.cdf)|*.cdf", .Title = "Save MS-Imaging Matrix"}
            If file.ShowDialog = DialogResult.OK Then
                Using filesave As FileStream = file.FileName.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                    Call loadedPixels.CreateCDF(filesave, dimension, params.GetTolerance)
                End Using
            End If
        End Using
    End Sub

    Dim pinedPixel As LibraryMatrix

    Private Sub PinToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PinToolStripMenuItem.Click
        Dim pos As Point = PixelSelector1.Pixel
        Dim pixel As PixelScan

        If Not ServiceHub.MSIEngineRunning Then
            Return
        Else
            pixel = ServiceHub.GetPixel(pos.X, pos.Y)
            pinedPixel = New LibraryMatrix With {
                .ms2 = pixel?.GetMs,
                .name = $"Select Pixel: [{pos.X},{pos.Y}]"
            }

            If pixel Is Nothing OrElse pinedPixel.ms2.IsNullOrEmpty Then
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

    Private Sub ClearSamplesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearToolStripMenuItem1.Click
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
        If Not ServiceHub.MSIEngineRunning Then
            Call MyApplication.host.warning("You must load raw data file at first!")
            Return
        End If

        Using file As New SaveFileDialog With {.Filter = "Plot Image(*.png)|*.png"}
            If file.ShowDialog = DialogResult.OK Then
                Call RscriptProgressTask.ExportSingleIonPlot(targetMz(0), mzdiff.GetScript, saveAs:=file.FileName)
            End If
        End Using
    End Sub

    Private Sub ImageProcessingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImageProcessingToolStripMenuItem.Click
        Dim getConfig As New InputImageProcessor
        Dim mask As New MaskForm(MyApplication.host.Location, MyApplication.host.Size)

        If mask.ShowDialogForm(getConfig) = DialogResult.OK Then
            Dim levels As Integer = CInt(getConfig.TrackBar1.Value)
            Dim contract As Double = getConfig.TrackBar2.Value

            If levels > 0 OrElse contract <> 0.0 Then
                Dim progress As New frmTaskProgress

                ' just exit image progress
                progress.TaskCancel = Sub() PixelSelector1.cancelBlur = True
                progress.ShowProgressTitle("Image Processing", True)
                progress.ShowProgressDetails("Do gauss blur...", True)
                progress.SetProgressMode()

                Call New Thread(Sub()
                                    Call Thread.Sleep(1000)
                                    Call progress.SetProgress(0, "Do gauss blur...")
                                    Call Me.Invoke(Sub() PixelSelector1.doGauss(levels * 13, contract, Sub(p) progress.SetProgress(p, $"Do gauss blur... {p.ToString("F2")}%")))
                                    Call progress.Invoke(Sub() progress.Close())
                                End Sub).Start()

                Call progress.ShowDialog()
            End If
        End If
    End Sub

    Private Sub CopyImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyImageToolStripMenuItem.Click
        Clipboard.Clear()
        Clipboard.SetImage(PixelSelector1.MSImage)

        Call MyApplication.host.showStatusMessage("MS-imaging plot has been copied to the clipboard!")
    End Sub
End Class
