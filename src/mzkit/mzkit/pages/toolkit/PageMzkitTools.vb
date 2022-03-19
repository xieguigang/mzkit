#Region "Microsoft.VisualBasic::a5e9b66e1d576a5f5d204479c731f2b8, mzkit\src\mzkit\mzkit\pages\toolkit\PageMzkitTools.vb"

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

    '   Total Lines: 876
    '    Code Lines: 665
    ' Comment Lines: 56
    '   Blank Lines: 155
    '     File Size: 39.36 KB


    ' Class PageMzkitTools
    ' 
    '     Function: getSelectedIonSpectrums, getXICMatrix, missingCacheFile, rawTIC, relativeInto
    ' 
    '     Sub: ClearToolStripMenuItem_Click, CustomTabControl1_TabClosing, DataGridView1_CellContentClick, ExportExactMassSearchTable, (+2 Overloads) MolecularNetworkingTool
    '          PageMzkitTools_Load, PageMzkitTools_Resize, PictureBox1_DoubleClick, PictureBox1_MouseClick, PlotMatrx
    '          PlotSpectrum, Ribbon_Load, SaveImageToolStripMenuItem_Click, SaveMatrixToolStripMenuItem_Click, (+2 Overloads) showAlignment
    '          (+3 Overloads) showMatrix, (+2 Overloads) ShowMatrix, ShowMRMTIC, ShowPage, ShowPlotTweaks
    '          showScatter, showSpectrum, ShowTabPage, showUVscans, ShowXIC
    '          (+3 Overloads) TIC
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.MoleculeNetworking
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.Math.UV
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.mzkit_win32.My
Imports BioNovoGene.mzkit_win32.RibbonLib.Controls
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Contour
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports RibbonLib
Imports RibbonLib.Interop
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports Task
Imports WeifenLuo.WinFormsUI.Docking
Imports stdNum = System.Math

Public Class PageMzkitTools

    Dim RibbonItems As RibbonItems
    Dim matrix As Array

    Friend matrixName As String

    Friend _ribbonExportDataContextMenuStrip As ExportData

    Public Sub Ribbon_Load(ribbon As Ribbon)
        _ribbonExportDataContextMenuStrip = New ExportData(ribbon, RibbonItems.cmdContextMap)
    End Sub

    Private Function missingCacheFile(raw As Raw) As DialogResult
        Dim options As DialogResult = MessageBox.Show($"The specific raw data cache is missing, run imports again?{vbCrLf}{raw.source.GetFullPath}", $"[{raw.source.FileName}] Cache Not Found!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)
        Dim fileExplorer = WindowModules.fileExplorer

        If options = DialogResult.OK Then
            Dim newRaw As Raw = frmFileExplorer.getRawCache(raw.source)

            raw.cache = newRaw.cache

            MyApplication.host.showStatusMessage("Ready!")
            MyApplication.host.ToolStripStatusLabel2.Text = fileExplorer.treeView1.Nodes(Scan0).GetTotalCacheSize
        End If

        Return options
    End Function

    Public Sub ShowPage()
        MyApplication.host.ShowPage(Me)
    End Sub

    Public Sub showScatter(raw As Raw, XIC As Boolean, directSnapshot As Boolean, contour As Boolean)
        If Not raw.cacheFileExists Then
            MessageBox.Show("Sorry, can not view file data, the cache file is missing...", "Cache Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        ElseIf directSnapshot Then
            PictureBox1.BackgroundImage = raw.GetSnapshot
        Else
            Dim colorSet As String
            Dim data As ContourLayer() = Nothing
            Dim width As Integer = 2048
            Dim height As Integer = 1600
            Dim padding As String = "padding:100px 400px 100px 100px;"

            If XIC Then
                colorSet = "YlGnBu:c8"
                width = 2400
            ElseIf contour Then
                colorSet = "Jet"

                Dim spinner As New frmProgressSpinner
                Dim task As New Thread(
                    Sub()
                        data = raw.GetContourData
                        spinner.Invoke(Sub() Call spinner.Close())
                    End Sub)

                Call task.Start()
                Call spinner.ShowDialog()

                width = 3600
                height = 2700
                padding = "padding:100px 750px 100px 100px;"
            Else
                colorSet = "darkblue,blue,skyblue,green,orange,red,darkred"
            End If

            Call MyApplication.RegisterPlot(
                Sub(args)
                    Dim spinner As New frmProgressSpinner
                    Dim task As New Thread(
                        Sub()
                            Dim image As Image

                            If contour Then
                                image = data.Plot(
                                    size:=$"{args.width},{args.height}",
                                    padding:=args.GetPadding.ToString,
                                    colorSet:=args.GetColorSetName,
                                    ppi:=200,
                                    legendTitle:=args.legend_title
                                ).AsGDIImage
                            ElseIf XIC Then
                                image = raw.Draw3DPeaks(colorSet:=args.GetColorSetName, size:=$"{args.width},{args.height}", args.GetPadding.ToString)
                            Else
                                image = raw.DrawScatter(colorSet:=args.GetColorSetName)
                            End If

                            Me.Invoke(Sub() PictureBox1.BackgroundImage = image)
                            spinner.Invoke(Sub() Call spinner.Close())
                        End Sub)

                    Call task.Start()
                    Call spinner.ShowDialog()
                End Sub, colorSet:=colorSet, width:=width, height:=height, padding:=padding, legendTitle:="Levels")
        End If

        Me.matrixName = $"{raw.source.FileName}_{If(XIC, "XICPeaks", "rawscatter_2D")}"

        MyApplication.host.ShowPage(Me)
        MyApplication.host.Invoke(Sub() RibbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.NotAvailable)
    End Sub

    Private Sub PageMzkitTools_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim host = MyApplication.host
        RibbonItems = host.ribbonItems

        ' Call InitializeFileTree()
        Call Globals.sharedProgressUpdater("Attatch Command Events...")

        'AddHandler TreeView1.AfterSelect, AddressOf TreeView1_AfterSelect
        'AddHandler host.fileExplorer.Button1.Click, Sub(obj, evt) Call SearchByMz(host.fileExplorer.TextBox2.Text)

        'AddHandler host.fileExplorer.ShowTICToolStripMenuItem.Click, AddressOf ShowTICToolStripMenuItem_Click
        'AddHandler host.fileExplorer.ShowXICToolStripMenuItem.Click, AddressOf ShowXICToolStripMenuItem_Click

        'AddHandler host.fileExplorer.ClearToolStripMenuItem.Click, AddressOf ClearToolStripMenuItem_Click
        'AddHandler host.fileExplorer.ExportToolStripMenuItem.Click, AddressOf ExportToolStripMenuItem_Click

        'AddHandler host.fileExplorer.MS1ToolStripMenuItem.Click, AddressOf MS1ToolStripMenuItem_Click
        'AddHandler host.fileExplorer.MS2ToolStripMenuItem.Click, AddressOf MS2ToolStripMenuItem_Click

        'AddHandler host.fileExplorer.MolecularNetworkingToolStripMenuItem.Click, AddressOf MolecularNetworkingToolStripMenuItem_Click

        'AddHandler host.fileExplorer.SearchInFileToolStripMenuItem.Click, AddressOf SearchInFileToolStripMenuItem_Click
        'AddHandler host.fileExplorer.CustomToolStripMenuItem.Click, AddressOf CustomToolStripMenuItem_Click
        'AddHandler host.fileExplorer.DefaultToolStripMenuItem.Click, AddressOf DefaultToolStripMenuItem_Click
        'AddHandler host.fileExplorer.SmallMoleculeToolStripMenuItem.Click, AddressOf SmallMoleculeToolStripMenuItem_Click
        'AddHandler host.fileExplorer.NatureProductToolStripMenuItem.Click, AddressOf NatureProductToolStripMenuItem_Click
        'AddHandler host.fileExplorer.GeneralFlavoneToolStripMenuItem.Click, AddressOf GeneralFlavoneToolStripMenuItem_Click
    End Sub

    Dim currentMatrix As [Variant](Of ms2(), ChromatogramTick())

    Friend Sub showSpectrum(scanId As String, raw As Raw)
        If raw.cacheFileExists Then
            Dim prop As SpectrumProperty = Nothing
            Dim scanData As LibraryMatrix = raw.GetSpectrum(scanId, Globals.Settings.viewer.GetMethod, Sub(src, cache) frmFileExplorer.getRawCache(src,, cache), prop)

            If prop.msLevel = 1 AndAlso RibbonItems.CheckBoxShowKEGGAnnotation.BooleanValue Then
                Call ConnectToBioDeep.OpenAdvancedFunction(
                    Sub()
                        Dim mzdiff1 As Tolerance = Tolerance.DeltaMass(0.001)
                        Dim mode As String = scanData.name.Match("[+-]")
                        Dim kegg As MSJointConnection = frmTaskProgress.LoadData(Function() Globals.LoadKEGG(AddressOf MyApplication.LogText, If(mode = "+", 1, -1), mzdiff1), info:="Load KEGG repository data...")
                        Dim anno As MzQuery() = kegg.SetAnnotation(scanData.mz)
                        Dim mzdiff As Tolerance = Tolerance.DeltaMass(0.05)
                        Dim compound As Compound

                        For Each mzi As ms2 In scanData.ms2
                            Dim hit As MzQuery = anno.Where(Function(d) mzdiff(d.mz, mzi.mz)).FirstOrDefault

                            If Not hit.unique_id.StringEmpty Then
                                compound = kegg.GetCompound(hit.unique_id)
                                mzi.Annotation = $"{mzi.mz.ToString("F4")} {compound.commonNames.FirstOrDefault([default]:=hit.unique_id)}{hit.precursorType}"
                            End If
                        Next
                    End Sub)
            End If

            showMatrix(scanData.ms2, scanId)

            Dim title1$
            Dim title2$

            If prop.msLevel = 1 Then
                title1 = $"MS1 Scan@{prop.retentionTime}sec"
                title2 = scanData.name
            Else
                title1 = $"M/Z {prop.precursorMz}, RT {prop.rtmin}min"
                title2 = scanData.name
            End If

            Call VisualStudio.ShowProperties(prop)
            Call PlotSpectrum(scanData)
            ' Call MyApplication.host.ShowPropertyWindow()
        Else
            Call missingCacheFile(raw)
        End If
    End Sub

    Public Sub PlotSpectrum(scanData As LibraryMatrix, Optional focusOn As Boolean = True)
        Call MyApplication.RegisterPlot(
              Sub(args)
                  scanData.name = args.title
                  PictureBox1.BackgroundImage = PeakAssign.DrawSpectrumPeaks(
                          scanData,
                          padding:=args.GetPadding.ToString,
                          bg:=args.background.ToHtmlColor,
                          size:=$"{args.width},{args.height}",
                          labelIntensity:=If(args.show_tag, 0.25, 100),
                          gridFill:=args.gridFill.ToHtmlColor,
                          barStroke:=$"stroke: steelblue; stroke-width: {args.line_width}px; stroke-dash: solid;"
                      ) _
                      .AsGDIImage
              End Sub,
          width:=2100,
          height:=1200,
          padding:="padding: 100px 30px 50px 100px;",
          bg:="white",
          title:=scanData.name,
          xlab:="M/z ratio",
          ylab:="Relative Intensity (%)"
      )

        If focusOn Then
            Call ShowTabPage(TabPage5)
        End If
    End Sub

    Public Sub PlotMatrx(title1$, title2$, scanData As LibraryMatrix, Optional focusOn As Boolean = True)
        Call MyApplication.RegisterPlot(
            Sub(args)
                PictureBox1.BackgroundImage = scanData _
                    .MirrorPlot(
                        titles:={title1, title2},
                        margin:=args.GetPadding.ToString,
                        drawLegend:=args.show_legend,
                        bg:=args.background.ToHtmlColor,
                        plotTitle:=args.title,
                        size:=$"{args.width},{args.height}"
                    ) _
                    .AsGDIImage
            End Sub,
            width:=1200,
            height:=800,
            padding:="padding: 100px 30px 50px 100px;",
            bg:="white",
            title:="BioDeep™ MS/MS alignment Viewer"
        )

        If focusOn Then
            Call ShowTabPage(TabPage5)
        End If
    End Sub

    Friend Sub ShowMatrix(PDA As PDAPoint(), name As String)
        Me.matrix = PDA
        matrixName = name

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "scan_time"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "total_ion"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "relative"})

        Dim max As Double = PDA.Select(Function(a) a.total_ion).Max

        For Each tick As PDAPoint In PDA
            DataGridView1.Rows.Add({tick.scan_time, tick.total_ion, tick.total_ion / max * 100})
        Next
    End Sub

    Friend Sub ShowMatrix(UVscan As UVScanPoint(), name As String)
        Me.matrix = UVscan
        matrixName = name

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "wavelength(nm)"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "intensity"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "relative"})

        Dim max As Double = UVscan.Select(Function(a) a.intensity).Max

        For Each tick As UVScanPoint In UVscan
            DataGridView1.Rows.Add({tick.wavelength, tick.intensity, tick.intensity / max * 100})
        Next
    End Sub

    Friend Sub showUVscans(scans As IEnumerable(Of GeneralSignal), title$, xlable$)
        Dim scanCollection As GeneralSignal() = scans.ToArray

        Call MyApplication.RegisterPlot(
            Sub(args)
                PictureBox1.BackgroundImage = UVsignalPlot.Plot(
                    signals:=scanCollection,
                    legendTitle:=Function(scan) If(scanCollection.Length = 1, $"UV scans", scan("title")),
                    size:=$"{args.width},{args.height}",
                    pt_size:=args.point_size,
                    line_width:=args.line_width,
                    title:=args.title,
                    xlabel:=args.xlabel,
                    ylabel:=args.ylabel,
                    showLegend:=args.show_legend,
                    showGrid:=args.show_grid,
                    gridFill:=args.gridFill.ToHtmlColor
                ).AsGDIImage
            End Sub, width:=2560, height:=1440,
                     padding:="padding:125px 50px 150px 200px;",
                     bg:="white",
                     title:=title,
                     xlab:=xlable,
                     ylab:="intensity",
                     gridFill:="white")

        ShowTabPage(TabPage5)
    End Sub

    Public Sub showAlignment(result As AlignmentOutput, Optional showScore As Boolean = False)
        If result Is Nothing Then
            Return
        Else
            With result.GetAlignmentMirror
                Call showAlignment(.query, .ref, result, showScore)
            End With
        End If
    End Sub

    Public Sub showAlignment(query As LibraryMatrix, ref As LibraryMatrix, result As AlignmentOutput, showScore As Boolean)
        Dim prop As New AlignmentProperty(result)
        Dim alignName As String = If(showScore, $"Cos: [{result.forward.ToString("F3")}, {result.reverse.ToString("F3")}]", $"{query.name}_vs_{ref.name}")

        MyApplication.host.ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active

        Call showMatrix(result.alignments, alignName)
        Call MyApplication.RegisterPlot(
            Sub(args)
                PictureBox1.BackgroundImage = MassSpectra.AlignMirrorPlot(
                    query:=query,
                    ref:=ref,
                    size:=$"{args.width},{args.height}",
                    title:=args.title,
                    drawLegend:=args.show_legend,
                    xlab:=args.xlabel,
                    ylab:=args.ylabel
                ).AsGDIImage
            End Sub,
            width:=1200,
            height:=800,
            padding:="padding: 100px 30px 50px 100px;",
            title:=alignName,
            xlab:="M/Z ratio",
            ylab:="Relative Intensity(%)"
        )

        Call VisualStudio.ShowProperties(prop)

        ShowTabPage(TabPage5)
    End Sub

    Private Function rawTIC(raw As Raw, isBPC As Boolean) As NamedCollection(Of ChromatogramTick)
        Dim TIC As New NamedCollection(Of ChromatogramTick) With {
            .name = $"{If(isBPC, "BPC", "TIC")} [{raw.source.FileName}]",
            .value = raw.GetMs1Scans _
                .Select(Function(m)
                            Return New ChromatogramTick With {.Time = m.rt, .Intensity = If(isBPC, m.BPC, m.TIC)}
                        End Function) _
                .ToArray
        }

        TIC.value = {
                New ChromatogramTick With {.Time = raw.rtmin},
                New ChromatogramTick With {.Time = raw.rtmax}
            }.JoinIterates(TIC.value) _
             .OrderBy(Function(c) c.Time) _
             .ToArray

        Return TIC
    End Function

    Public Sub TIC(rawList As IEnumerable(Of Raw), Optional isBPC As Boolean = False)
        Dim TICList As New List(Of NamedCollection(Of ChromatogramTick))

        For Each raw As Raw In rawList
            TICList.Add(rawTIC(raw, isBPC))
        Next

        Call TIC(TICList.ToArray)
    End Sub

    Public Sub TIC(TICList As NamedCollection(Of ChromatogramTick)(), Optional d3 As Boolean = False)
        If TICList.IsNullOrEmpty Then
            MyApplication.host.showStatusMessage("no chromatogram data!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        ElseIf TICList.All(Function(file) file.All(Function(t) t.Intensity = 0.0)) Then
            MyApplication.host.showStatusMessage("not able to create a TIC/BPC plot due to the reason of all of the tick intensity data is ZERO, please check your raw data file!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Call showMatrix(TICList(Scan0).value, TICList(Scan0).name)

        MyApplication.RegisterPlot(
            Sub(args)
                If d3 Then
                    PictureBox1.BackgroundImage = New ScanVisual3D(scans:=TICList, angle:=60, fillCurve:=True, fillAlpha:=120, drawParallelAxis:=True, theme:=New Theme With {
                        .colorSet = args.GetColorSetName,
                        .gridFill = args.gridFill.ToHtmlColor,
                        .padding = args.GetPadding.ToString,
                        .drawLegend = args.show_legend,
                        .drawLabels = args.show_tag,
                        .drawGrid = args.show_grid,
                        .tagCSS = New CSSFont(args.label_font).ToString
                    }) With {
                        .xlabel = args.xlabel,
                        .ylabel = args.ylabel,
                        .main = args.title
                    }.Plot($"{args.width},{args.height}", ppi:=100) _
                      .AsGDIImage
                Else
                    PictureBox1.BackgroundImage = ChromatogramPlot.TICplot(
                        ionData:=TICList.ToArray,
                        colorsSchema:=args.GetColorSetName,
                        fillCurve:=Globals.Settings.viewer.fill,
                        size:=$"{args.width},{args.height}",
                        margin:=args.GetPadding.ToString,
                        gridFill:=args.gridFill.ToHtmlColor,
                        bg:=args.background.ToHtmlColor,
                        showGrid:=args.show_grid,
                        showLegends:=args.show_legend,
                        showLabels:=args.show_tag,
                        xlabel:=args.xlabel,
                        ylabel:=args.ylabel,
                        labelFontStyle:=New CSSFont(args.label_font).ToString
                    ).AsGDIImage
                End If
            End Sub, width:=1600, height:=1200, showGrid:=True, padding:="padding:100px 100px 150px 200px;", showLegend:=Not d3, xlab:="Time (s)", ylab:="Intensity")

        MyApplication.host.ShowPage(Me)
    End Sub

    Public Sub ShowMRMTIC(name As String, ticks As ChromatogramTick())
        Call TIC({New NamedCollection(Of ChromatogramTick)(name, ticks)})
    End Sub

    Public Sub TIC(isBPC As Boolean)
        Dim rawList As Raw() = WindowModules.fileExplorer.GetSelectedRaws.ToArray

        If rawList.Length = 0 Then
            MyApplication.host.showStatusMessage("No file data selected for TIC plot...")
        Else
            Call TIC(rawList, isBPC)
        End If
    End Sub

    Public Sub SaveImageToolStripMenuItem_Click()
        If Not PictureBox1.BackgroundImage Is Nothing Then
            Dim preFileName As String = matrixName.NormalizePathString(alphabetOnly:=False)

            Using file As New SaveFileDialog With {.Filter = "image(*.png)|*.png", .FileName = preFileName & ".png"}
                If file.ShowDialog = DialogResult.OK Then
                    Call PictureBox1.BackgroundImage.SaveAs(file.FileName)
                    Call Process.Start(file.FileName)
                End If
            End Using
        Else
            MyApplication.host.showStatusMessage("No plot image for save, please select one spectrum to start!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        End If
    End Sub

    Public Sub ExportExactMassSearchTable()
        Call DataGridView1.SaveDataGrid($"Exact mass search result table export to [%s] successfully!")
    End Sub

    Public Sub SaveMatrixToolStripMenuItem_Click()
        If matrix Is Nothing Then
            MyApplication.host.showStatusMessage("No matrix data for save, please select one spectrum to start!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Using file As New SaveFileDialog() With {.Filter = "Excel Table(*.xls)|*.xls", .FileName = matrixName.NormalizePathString(False)}
                If file.ShowDialog = DialogResult.OK Then

                    Select Case matrix.GetType
                        Case GetType(ms2()) : Call DirectCast(matrix, ms2()).SaveTo(file.FileName)
                        Case GetType(ChromatogramTick()) : Call DirectCast(matrix, ChromatogramTick()).SaveTo(file.FileName)
                        Case GetType(SSM2MatrixFragment()) : Call DirectCast(matrix, SSM2MatrixFragment()).SaveTo(file.FileName)
                        Case GetType(PDAPoint()) : Call DirectCast(matrix, PDAPoint()).SaveTo(file.FileName)
                        Case GetType(UVScanPoint()) : Call DirectCast(matrix, UVScanPoint()).SaveTo(file.FileName)

                        Case Else
                            Throw New NotImplementedException
                    End Select

                End If
            End Using
        End If
    End Sub

    ''' <summary>
    ''' load data and then run clustering
    ''' </summary>
    ''' <param name="progress"></param>
    ''' <param name="similarityCutoff"></param>
    Friend Sub MolecularNetworkingTool(progress As frmTaskProgress, similarityCutoff As Double)
        Thread.Sleep(1000)

        progress.ShowProgressTitle("Load Scan data")
        progress.ShowProgressDetails("loading cache ms2 scan data...")

        Dim raw As PeakMs2() = getSelectedIonSpectrums(AddressOf progress.ShowProgressTitle).ToArray

        If raw.Length = 0 Then
            MyApplication.host.showStatusMessage("No spectrum data, please select a file or some spectrum...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Call MolecularNetworkingTool(raw, progress, similarityCutoff)
        End If

        Call progress.Invoke(Sub() progress.Close())
    End Sub

    Friend Sub MolecularNetworkingTool(raw As PeakMs2(), progress As frmTaskProgress, similarityCutoff As Double)
        Dim protocol As New Protocols(
            ms1_tolerance:=Tolerance.PPM(15),
            ms2_tolerance:=Tolerance.DeltaMass(0.3),
            treeIdentical:=Globals.Settings.network.treeNodeIdentical,
            treeSimilar:=Globals.Settings.network.treeNodeSimilar,
            intoCutoff:=Globals.Settings.viewer.GetMethod
        )
        Dim progressMsg As Action(Of String) = AddressOf progress.ShowProgressTitle

        ' filter empty spectrum
        raw = (From r As PeakMs2 In raw Where Not r.mzInto.IsNullOrEmpty).ToArray

        'Dim run As New List(Of PeakMs2)
        'Dim nodes As New Dictionary(Of String, ScanEntry)
        'Dim idList As New Dictionary(Of String, Integer)



        'Using cache As New netCDFReader(raw.cache)
        '    For Each scan In raw.scans.Where(Function(s) s.mz > 0)
        '        Dim uid As String = $"M{CInt(scan.mz)}T{CInt(scan.rt)}"

        '        If idList.ContainsKey(uid) Then
        '            idList(uid) += 1
        '            uid = uid & "_" & (idList(uid) - 1)
        '        Else
        '            idList.Add(uid, 1)
        '        End If

        '        run += New PeakMs2 With {
        '            .rt = scan.rt,
        '            .mz = scan.mz,
        '            .lib_guid = uid,
        '            .mzInto = cache.getDataVariable(scan.id).numerics.AsMs2.ToArray.Centroid(Tolerance.DeltaMass(0.3)).ToArray
        '        }

        '        progress.Invoke(Sub() progress.ShowProgressTitle (scan.id)
        '        nodes.Add(run.Last.lib_guid, scan)
        '    Next
        'End Using

        progress.ShowProgressTitle("run molecular networking....")

        ' Call tree.doCluster(run)
        Dim links = protocol.RunProtocol(raw, progressMsg).ProduceNodes.Networking.ToArray
        Dim net As IO.DataSet() = links _
            .Select(Function(a)
                        Return New IO.DataSet With {
                            .ID = a.Name,
                            .Properties = a.Value _
                                .ToDictionary(Function(t) t.Key,
                                                Function(t)
                                                    Return stdNum.Min(t.Value.forward, t.Value.reverse)
                                                End Function)
                        }
                    End Function) _
            .ToArray   ' MoleculeNetworking.CreateMatrix(run, 0.8, Tolerance.DeltaMass(0.3), Sub(msg) progress.Invoke(Sub() progress.ShowProgressDetails ( msg)).ToArray

        progress.ShowProgressDetails("run family clustering....")

        If net.Length < 3 Then
            Call MyApplication.host.showStatusMessage("the ions data is not enough for create network!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Dim kn As Integer

            If net.Length > 9 Then
                kn = 9
            Else
                kn = CInt(net.Length / 2)
            End If

            Dim clusters = net.ToKMeansModels.Kmeans(expected:=kn, debug:=False)
            Dim rawLinks = links.ToDictionary(Function(a) a.Name, Function(a) a.Value)

            progress.ShowProgressDetails("initialize result output...")

            MyApplication.host.Invoke(
                Sub()
                    Call MyApplication.host.mzkitMNtools.loadNetwork(clusters, protocol, rawLinks, similarityCutoff)
                    Call MyApplication.host.ShowPage(MyApplication.host.mzkitMNtools)
                End Sub)
        End If
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        If e.ColumnIndex = Scan0 AndAlso e.RowIndex >= 0 Then
            Dim scanId As String = DataGridView1.Rows(e.RowIndex).Cells(0).Value?.ToString

            If Not scanId.StringEmpty Then
                ' Call showSpectrum(scanId, TreeView1.CurrentRawFile.raw)
            End If
        End If
    End Sub

    Sub showMatrix(matrix As ms2(), name As String)
        Me.matrix = matrix

        matrixName = name

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "m/z"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "intensity"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "relative"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "annotation"})

        Dim max As Double

        If matrix.Length = 0 Then
            max = 0
            Call MyApplication.host.showStatusMessage($"'{name}' didn't contains any data...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            max = matrix.Select(Function(a) a.intensity).Max
        End If

        For Each tick As ms2 In matrix
            DataGridView1.Rows.Add({tick.mz, tick.intensity, CInt(tick.intensity / max * 100), tick.Annotation})
            Application.DoEvents()
        Next
    End Sub

    Sub showMatrix(matrix As SSM2MatrixFragment(), name As String)
        Me.matrix = matrix
        matrixName = name

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "m/z"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "intensity(query)"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "intensity(target)"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "tolerance"})

        For Each tick In matrix
            DataGridView1.Rows.Add({tick.mz, tick.query, tick.ref, tick.da})
        Next
    End Sub

    Public Sub showMatrix(matrix As ChromatogramTick(), name As String)
        Me.matrix = matrix
        matrixName = name

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "time"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "intensity"})

        For Each tick In matrix
            DataGridView1.Rows.Add({tick.Time, tick.Intensity})
        Next
    End Sub

    Public Sub ShowXIC(ppm As Double, plotTIC As NamedCollection(Of ChromatogramTick), getXICCollection As Func(Of Double, IEnumerable(Of NamedCollection(Of ChromatogramTick))), maxY As Double)
        Dim progress As New frmProgressSpinner
        Dim plotImage As Image = Nothing
        Dim relative As Boolean = relativeInto()

        Call New Thread(
            Sub()
                Dim XICPlot As New List(Of NamedCollection(Of ChromatogramTick))

                If Not plotTIC.IsEmpty Then
                    XICPlot.Add(plotTIC)
                End If

                XICPlot.AddRange(getXICCollection(ppm))

                If XICPlot.Count = 0 Then
                    Call MyApplication.host.showStatusMessage("No XIC ions data for generate plot!", My.Resources.StatusAnnotations_Warning_32xLG_color)
                Else
                    plotImage = XICPlot _
                        .ToArray _
                        .TICplot(
                            intensityMax:=If(relative, 0, maxY),
                            isXIC:=True,
                            colorsSchema:=Globals.GetColors,
                            fillCurve:=Globals.Settings.viewer.fill,
                            gridFill:="white"
                        ).AsGDIImage
                End If

                Call progress.Invoke(Sub() progress.Close())
            End Sub).Start()

        progress.ShowDialog()

        PictureBox1.BackgroundImage = plotImage
        ShowTabPage(TabPage5)
    End Sub

    Friend Iterator Function getSelectedIonSpectrums(progress As Action(Of String)) As IEnumerable(Of PeakMs2)
        Dim raw = WindowModules.rawFeaturesList.CurrentRawFile

        For Each ionNode As TreeNode In WindowModules.rawFeaturesList.GetSelectedNodes.Where(Function(a) TypeOf a.Tag Is ScanMS2)
            Dim scanId As String = ionNode.Text
            Dim info As ScanMS2 = ionNode.Tag
            Dim guid As String = $"{raw.source.FileName}#{scanId}"

            If Not progress Is Nothing Then
                Call progress(guid)
            End If

            Yield New PeakMs2 With {
                .mz = info.parentMz,
                .scan = info.scan_id,
                .activation = info.activationMethod,
                .collisionEnergy = info.collisionEnergy,
                .file = raw.source.FileName,
                .lib_guid = guid,
                .mzInto = info.GetMs.ToArray,
                .precursor_type = "n/a",
                .rt = info.rt,
                .intensity = info.intensity
            }
        Next
    End Function

    Private Function relativeInto() As Boolean
        Return MyApplication.host.ribbonItems.CheckBoxXICRelative.BooleanValue
    End Function

    Friend Function getXICMatrix(raw As Raw, scanId As String, ppm As Double, relativeInto As Boolean) As NamedCollection(Of ChromatogramTick)
        Dim ms2 As ScanMS2 = raw.FindMs2Scan(scanId)
        Dim name As String = raw.source.FileName

        If ms2 Is Nothing OrElse ms2.parentMz = 0.0 Then
            MyApplication.host.showStatusMessage("XIC plot is not avaliable for MS1 parent!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return Nothing
        Else
            MyApplication.host.showStatusMessage(name, Nothing)
        End If

        Dim selectedIons = raw _
            .GetMs2Scans _
            .Where(Function(a) PPMmethod.PPM(a.parentMz, ms2.parentMz) <= ppm) _
            .ToArray
        Dim XIC As ChromatogramTick() = selectedIons _
            .Select(Function(a)
                        Return New ChromatogramTick With {
                            .Time = a.rt,
                            .Intensity = a.intensity
                        }
                    End Function) _
            .ToArray
        Dim mzmin = Aggregate ion In selectedIons Into Min(ion.parentMz)
        Dim mzmax = Aggregate ion In selectedIons Into Max(ion.parentMz)

        name = $"XIC [m/z={ms2.parentMz.ToString("F4")}] [mzmin={mzmin.ToString("F4")}, mzmax={mzmax.ToString("F4")}]"

        If Not relativeInto Then
            XIC = {
                  New ChromatogramTick With {.Time = raw.rtmin},
                  New ChromatogramTick With {.Time = raw.rtmax}
              }.JoinIterates(XIC) _
               .OrderBy(Function(c) c.Time) _
               .ToArray
        End If

        Dim plotTIC As New NamedCollection(Of ChromatogramTick) With {
            .name = name,
            .value = XIC,
            .description = ms2.parentMz & " " & raw.source.FileName
        }

        Return plotTIC
    End Function

    'Private Sub AddToolStripMenuItem_Click(sender As Object, e As EventArgs)
    '    Dim XIC = getXICMatrix(TreeView1.CurrentRawFile.raw)

    '    If Not XIC.IsEmpty Then
    '        XICCollection.Add(XIC)
    '    End If
    'End Sub

    Private Sub ClearToolStripMenuItem_Click(sender As Object, e As EventArgs)
        ' XICCollection.Clear()
        ' MyApplication.host.fileExplorer.Clear()
        '  MyApplication.host.fileExplorer.ClearToolStripMenuItem.Text = "Clear"
    End Sub

    Private Sub PictureBox1_DoubleClick(sender As Object, e As EventArgs) Handles PictureBox1.DoubleClick
        If Not PictureBox1.BackgroundImage Is Nothing Then
            Dim temp As String = TempFileSystem.GetAppSysTempFile(".png", App.PID, "imagePlot_")

            Call PictureBox1.BackgroundImage.SaveAs(temp)
            Call Process.Start(temp)
        End If
    End Sub

    Private Sub PictureBox1_MouseClick(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseClick
        If e.Button = MouseButtons.Right Then
            Dim p As Point = PictureBox1.PointToScreen(e.Location)
            MyApplication.host.Ribbon1.ShowContextPopup(CUInt(RibbonItems.cmdContextMap), p.X, p.Y)
        End If
    End Sub

    Private Sub CustomTabControl1_TabClosing(sender As Object, e As TabControlCancelEventArgs) Handles CustomTabControl1.TabClosing
        e.Cancel = True

        If CustomTabControl1.Controls.Count = 1 Then
            If e.TabPage Is TabPage5 Then

            Else
                CustomTabControl1.Controls.Clear()
                ShowTabPage(TabPage5)
            End If
        Else
            CustomTabControl1.Controls.Remove(e.TabPage)
            e.TabPage.Hide()
        End If
    End Sub

    Public Sub ShowTabPage(tabpage As TabPage)
        If Not CustomTabControl1.Controls.Contains(tabpage) Then
            CustomTabControl1.Controls.Add(tabpage)
        End If

        WindowModules.panelMain.Show(MyApplication.host.dockPanel)

        CustomTabControl1.SelectedTab = tabpage
        tabpage.Visible = True
    End Sub

    Public Sub ShowPlotTweaks()
        RibbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active
        VisualStudio.Dock(WindowModules.plotParams, DockState.DockRight)
    End Sub

    Private Sub PageMzkitTools_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        With Size
            WindowModules.plotParams.params.width = .Width
            WindowModules.plotParams.params.height = .Height
        End With
    End Sub
End Class
