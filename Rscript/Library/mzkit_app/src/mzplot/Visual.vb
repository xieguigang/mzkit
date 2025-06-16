﻿#Region "Microsoft.VisualBasic::e1dd98a2401630c226f9e1f2615a4702, Rscript\Library\mzkit_app\src\mzplot\Visual.vb"

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

    '   Total Lines: 959
    '    Code Lines: 744 (77.58%)
    ' Comment Lines: 127 (13.24%)
    '    - Xml Docs: 92.91%
    ' 
    '   Blank Lines: 88 (9.18%)
    '     File Size: 43.97 KB


    ' Module Visual
    ' 
    '     Function: assembleOverlaps, ParseSpectrumAlignment, plotAlignments, plotChromatogram, PlotGCxGCHeatMap
    '               plotGCxGCTic2D, plotMolecularNetworkingHistogram, plotMRM, plotMS, plotOverlaps
    '               plotPeaktable, plotRawChromatogram, PlotRawScatter, plotRtShifts, plotSignal
    '               plotSignal2, plotTIC, plotTIC2, PlotUVSignals, Snapshot3D
    '               SpectrumPlot, XicScatterDensity
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.GCxGC
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.SignalReader
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports BioNovoGene.BioDeep.MassSpectrometry.MoleculeNetworking
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.BarPlot
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports Chromatogram = BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram.Chromatogram
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

''' <summary>
''' Mass spectrum data visualization
''' </summary>
<Package("visual")>
Module Visual

    Sub Main()
        Call RInternal.generic.add("plot", GetType(GeneralSignal), AddressOf plotSignal)
        Call RInternal.generic.add("plot", GetType(GeneralSignal()), AddressOf plotSignal2)
        Call RInternal.generic.add("plot", GetType(MGF.Ions), AddressOf plotMS)
        Call RInternal.generic.add("plot", GetType(PeakMs2), AddressOf plotMS)
        Call RInternal.generic.add("plot", GetType(LibraryMatrix), AddressOf plotMS)
        Call RInternal.generic.add("plot", GetType(Chromatogram), AddressOf plotChromatogram)
        Call RInternal.generic.add("plot", GetType(mzPack), AddressOf plotRawChromatogram)
        Call RInternal.generic.add("plot", GetType(ChromatogramOverlap), AddressOf plotOverlaps)
        Call RInternal.generic.add("plot", GetType(D2Chromatogram()), AddressOf plotGCxGCTic2D)
        Call RInternal.generic.add("plot", GetType(D2Chromatogram), AddressOf plotTIC2)
        Call RInternal.generic.add("plot", GetType(ChromatogramTick()), AddressOf plotTIC)
        Call RInternal.generic.add("plot", GetType(PeakSet), AddressOf plotPeaktable)
        Call RInternal.generic.add("plot", GetType(AlignmentOutput), AddressOf plotAlignments)
        Call RInternal.generic.add("plot", GetType(ScanMS1), AddressOf plotMS)
        Call RInternal.generic.add("plot", GetType(ScanMS2), AddressOf plotMS)
        Call RInternal.generic.add("plot", GetType(RtShift()), AddressOf plotRtShifts)
        Call RInternal.generic.add("plot", GetType(IonChromatogram), AddressOf plotMRM)
    End Sub

    <RGenericOverloads("plot")>
    Private Function plotMRM(xic As IonChromatogram, args As list, env As Environment) As Object
        If Not args.hasName("name") Then
            Call args.add("name", If(xic.name, xic.description))
        End If

        Return plotTIC(xic.chromatogram, args, env)
    End Function

    <RGenericOverloads("plot")>
    Private Function plotRtShifts(rt_shifts As RtShift(), args As list, env As Environment) As Object
        Dim samples = rt_shifts _
            .GroupBy(Function(a) a.sample) _
            .Select(Function(file) New NamedCollection(Of RtShift)(file.Key, file)) _
            .ToArray
        Dim rt_range As New DoubleRange(rt_shifts.Select(Function(a) a.refer_rt))
        Dim res As Double = args.getValue({"res"}, env, [default]:=1000)
        Dim dt As Double = (rt_range.Max - rt_range.Min) / res
        Dim x_axis As Double() = seq(rt_range.Min, rt_range.Max, by:=dt).ToArray
        Dim lines As New List(Of SerialData)
        Dim size As String = InteropArgumentHelper.getSize(args.getByName("size"), env, "3800,3000")
        Dim padding As String = InteropArgumentHelper.getPadding(args.getByName("padding"), "padding: 100px 600px 200px 200px;")
        Dim colorSet = args.getValue({"colorSet", "colors"}, env, "paper")
        Dim colors As Color() = Designer.GetColors(colorSet, n:=samples.Length + 1)
        Dim fill_color As String = RColorPalette.getColor(args.getBySynonyms("fill", "grid.fill"), "lightgray")
        Dim idx As i32 = 0
        Dim plot_ri As Boolean = args.getValue({"x_axis.ri"}, env, False)
        Dim points As NamedCollection(Of RtShift)()
        Dim legend_split As Integer = args.getValue({"legend_split", "legend.split"}, env, 20)
        Dim ppi As Integer = If(env.getDriver = Drivers.SVG, 120, 300)

        For Each sample As NamedCollection(Of RtShift) In samples
            If plot_ri Then
                points = sample _
                    .GroupBy(Function(a) a.RI, offsets:=dt) _
                    .OrderBy(Function(a) Val(a.name)) _
                    .ToArray
            Else
                points = sample _
                    .GroupBy(Function(a) a.refer_rt, offsets:=dt) _
                    .OrderBy(Function(a) Val(a.name)) _
                    .ToArray
            End If

            Dim shift_points = points _
                .Select(Function(dti)
                            Return New PointData(Val(dti.name), Aggregate pt In dti Into Sum(pt.shift))
                        End Function) _
                .ToArray

            lines.Add(New SerialData With {
                .lineType = DashStyle.Solid,
                .pointSize = 3,
                .pts = shift_points,
                .shape = LegendStyles.Square,
                .title = sample.name,
                .width = 2,
                .color = colors(++idx)
            })
        Next

        Return Scatter.Plot(lines, size:=size, padding:=padding, drawLine:=True, fill:=False,
                            Xlabel:=If(plot_ri, "retention index", "retention time(s)"),
                            Ylabel:="RT shift(s)",
                            XtickFormat:="F0", YtickFormat:="G4",
                            gridFill:=fill_color,
                            tickFontStyle:=CSSFont.Win7Small,
                            legendFontCSS:=CSSFont.Win10Normal,
                            legendSplit:=legend_split,
                            driver:=env.getDriver,
                            dpi:=ppi)
    End Function

    <RGenericOverloads("plot")>
    Private Function plotAlignments(aligns As AlignmentOutput, args As list, env As Environment) As Object
        Dim pairwise = aligns.GetAlignmentMirror
        Dim title As String = args.getValue("title", env, [default]:=$"{aligns.query?.id} vs {aligns.reference?.id}")
        Dim legend_layout As String = args.getValue("legend_layout", env, "none")
        Dim bar_width As Single = args.getValue("bar_width", env, 8.0)
        Dim color1 As String = args.getValue("color1", env, AlignmentPlot.DefaultColor1)
        Dim color2 As String = args.getValue("color2", env, AlignmentPlot.DefaultColor2)
        Dim label_intensity As Double = args.getValue("label_into", env, 0.2)
        Dim label_mz As String = args.getValue("label_mz", env, "F4")
        Dim grid_x As Boolean = args.getValue("grid_x", env, False)
        Dim show_hit_highlights As Boolean = args.getValue("show_hits", env, False)

        Return MassSpectra.AlignMirrorPlot(
            query:=pairwise.query,
            ref:=pairwise.ref,
            title:=title,
            drawGrid:=True,
            tagXFormat:=label_mz,
            labelDisplayIntensity:=label_intensity,
            driver:=env.getDriver,
            legendLayout:=legend_layout,
            bw:=bar_width,
            color1:=color1,
            color2:=color2,
            drawGridX:=grid_x,
            highlights:=If(show_hit_highlights, aligns.GetHitsMzPeaks.ToArray, Nothing)
        )
    End Function

    ''' <summary>
    ''' draw peaktable as heatmap/scatter
    ''' </summary>
    ''' <param name="peakSet"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <RGenericOverloads("plot")>
    Private Function plotPeaktable(peakSet As PeakSet, args As list, env As Environment) As Object
        Dim theme As New Theme With {
            .axisLabelCSS = "font-style: normal; font-size: 12; font-family: " & FontFace.CambriaMath & ";",
            .colorSet = "Jet",
            .padding = InteropArgumentHelper.getPadding(args.getBySynonyms("padding", "paddings", "margin"), "padding: 100px 650px 200px 250px;", env),
            .pointSize = args.getValue({"point_size", "point.size"}, env, 10.0)
        }
        Dim scatter As Boolean = args.getValue({"scatter"}, env, False)
        Dim app As Plot
        Dim nlevels As Integer = args.getValue({"levels", "nlevel"}, env, 30)

        If scatter Then
            Dim dimension As String = args.getValue({"dimension", "dim_name"}, env, "default")
            Dim scatter_data As ms1_scan() = peakSet.Ms1Scatter(dimension).ToArray

            If scatter_data.All(Function(i) i.intensity = 0.0) Then
                Return RInternal.debug.stop({
                    $"missing of the data for plot the scatter on specific data dimension: {dimension}",
                    $"dimension: {dimension}"
                }, env)
            End If

            app = New RawScatterPlot(scatter_data, nlevels, "peaktable", theme) With {
                .legendTitle = dimension
            }
        Else
            app = New PeakTablePlot(peakSet, theme) With {
                .mapLevels = nlevels
            }
        End If

        Return app.Plot(driver:=env.getDriver)
    End Function

    <RGenericOverloads("plot")>
    Private Function plotGCxGCTic2D(x As Chromatogram2DScan(), args As list, env As Environment) As Object
        Dim theme As New Theme With {
            .padding = args.getValue("padding", env, "padding: 250px 500px 200px 200px;"),
            .colorSet = args.getValue("colorSet", env, "Jet")
        }
        Dim size As String = InteropArgumentHelper.getSize(args.getByName("size"), env, "3800,3000")
        Dim q As Double = args.getValue("TrIQ", env, 1.0)
        Dim qlow As Double = args.getValue("q.low", env, 0.05)
        Dim mapLevels As Integer = args.getValue("map.levels", env, 64)
        Dim mesh3D As Boolean = args.getValue("peaks3D", env, False)
        Dim app As Plot
        Dim driver As Drivers = env.getDriver

        If mesh3D Then
            app = New GCxGCTIC3DPeaks(x, 5, mapLevels, theme) With {
                .xlabel = args.getValue("xlab", env, "Dimension 1 RT(s)"),
                .ylabel = args.getValue("ylab", env, "Dimension 2 RT(s)"),
                .zlabel = args.getValue("zlab", env, "Intensity"),
                .legendTitle = "Intensity",
                .main = "GCxGC 2D Imaging"
            }
        Else
            app = New GCxGCTIC2DPlot(x, qlow, q, mapLevels, theme) With {
                .xlabel = args.getValue("xlab", env, "Dimension 1 RT(s)"),
                .ylabel = args.getValue("ylab", env, "Dimension 2 RT(s)"),
                .legendTitle = "Intensity",
                .main = "GCxGC 2D Imaging"
            }
        End If

        Return app.Plot(size, driver:=driver)
    End Function

    ''' <summary>
    ''' plot TIC overlaps
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' 
    <RGenericOverloads("plot")>
    <Extension>
    Private Function plotOverlaps(x As ChromatogramOverlap, args As list, env As Environment) As Object
        Dim isBPC As Boolean = args.getValue("bpc", env, [default]:=False)
        Dim alpha As Integer = args.getValue("opacity", env, [default]:=100)
        Dim colorSet As String = args.getValue("colors", env, [default]:="Paired:c12")
        Dim gridFill As String = args.getValue("grid.fill", env, [default]:="white")
        Dim fill As Boolean = args.getValue("fill", env, [default]:=True)
        Dim showLabels As Boolean = args.getValue("show.labels", env, [default]:=True)
        Dim showLegends As Boolean = args.getValue("show.legends", env, [default]:=True)
        Dim parallel As Boolean = args.getValue("parallel", env, [default]:=False)
        Dim axisStroke As String = args.getValue("axis.stroke", env, [default]:="stroke: black; stroke-width: 3px; stroke-dash: solid;")
        Dim lineStroke As String = args.getValue("line.stroke", env, [default]:="stroke: black; stroke-width: 2px; stroke-dash: solid;")
        Dim padding As String = InteropArgumentHelper.getPadding(args.getByName("padding"), "padding:5% 5% 10% 10%;", env)
        Dim axisLabel As String = args.getValue("axis.cex", env, "font-style: normal; font-size: 24; font-family: Bookman Old Style;")
        Dim axisTickCex As String = args.getValue("tick.cex", env, "font-style: normal; font-size: 16; font-family: Bookman Old Style;")
        Dim legendLabel As String = args.getValue("legend.cex", env, "font-style: normal; font-size: 12; font-family: Bookman Old Style;")
        Dim size As String = InteropArgumentHelper.getSize(args.getByName("size"), env, "1600,1000")
        Dim xlab As String = args.getValue("xlab", env, "Time (s)")
        Dim ylab As String = args.getValue("ylab", env, "Intensity")
        Dim title As String = CLRVector.asScalarCharacter(args.getBySynonyms("title", "main"))
        Dim reorderOverlaps As Boolean = args.getValue("reorder.overlaps", env, [default]:=False)
        Dim legend_split_size As Integer = args.getValue("legend.split_size", env, [default]:=32)
        Dim overlaps As New List(Of NamedCollection(Of ChromatogramTick))
        Dim data As NamedCollection(Of ChromatogramTick)
        Dim driver As Drivers = env.getDriver

        For Each raw In x.overlaps
            If raw.Value Is Nothing OrElse raw.Value.scan_time.IsNullOrEmpty Then
                Call env.AddMessage($"missing chromatogram of {raw.Key}!")
                Continue For
            End If

            data = New NamedCollection(Of ChromatogramTick) With {
                .name = raw.Key,
                .value = raw.Value.GetTicks(isBPC).ToArray
            }
            overlaps.Add(data)
        Next

        If reorderOverlaps Then
            overlaps = overlaps _
                .OrderByDescending(Function(c)
                                       Return Aggregate tick As ChromatogramTick In c Into Sum(tick.Intensity)
                                   End Function) _
                .AsList
        End If

        Return overlaps.ToArray _
            .TICplot(
                fillAlpha:=alpha,
                colorsSchema:=colorSet,
                gridFill:=gridFill,
                showGrid:=True,
                showLabels:=showLabels,
                parallel:=parallel,
                axisStroke:=axisStroke,
                fillCurve:=fill,
                margin:=padding,
                size:=size,
                penStyle:=lineStroke,
                axisLabelFont:=axisLabel,
                legendFontCSS:=legendLabel,
                xlabel:=xlab,
                ylabel:=ylab,
                axisTickFont:=axisTickCex,
                showLegends:=showLegends,
                legend_split:=legend_split_size,
                labelLayoutTicks:=-1,
                ppi:=args.getValue({"dpi", "ppi"}, env, [default]:=200),
                driver:=driver,
                title:=title
            )
    End Function

    ''' <summary>
    ''' plot single TIC
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' 
    <RGenericOverloads("plot")>
    Private Function plotRawChromatogram(x As mzPack, args As list, env As Environment) As Object
        Dim scatter As Boolean = args.getValue("scatter", env, [default]:=False)

        If scatter Then
            Return PlotRawScatter(x, env:=env)
        End If

        Dim chr As Chromatogram = x.Chromatogram

        If chr Is Nothing Then
            chr = New Chromatogram With {
                .scan_time = x.MS.Select(Function(a) a.rt).ToArray,
                .BPC = x.MS.Select(Function(a) a.BPC).ToArray,
                .TIC = x.MS.Select(Function(a) a.TIC).ToArray
            }
        End If

        Return plotChromatogram(chr, args, env)
    End Function

    <RGenericOverloads("plot")>
    <Extension>
    Private Function plotTIC2(x As Chromatogram2DScan, args As list, env As Environment) As Object
        Return x.chromatogram.plotTIC(args, env)
    End Function

    <RGenericOverloads("plot")>
    <Extension>
    Private Function plotTIC(x As ChromatogramTick(), args As list, env As Environment) As Object
        Dim name As String = args.getValue("name", env, [default]:="unknown")
        Dim color As String = args.getValue("color", env, [default]:="skyblue")
        Dim gridFill As String = args.getValue("grid.fill", env, [default]:="white")
        Dim alpha As Integer = args.getValue("opacity", env, [default]:=100)
        Dim xlab As String = args.getValue("xlab", env, "Time (s)")
        Dim ylab As String = args.getValue("ylab", env, "Intensity")
        Dim data As New NamedCollection(Of ChromatogramTick) With {
            .name = name,
            .value = x
        }

        Return data.TICplot(
            colorsSchema:=color,
            gridFill:=gridFill,
            fillAlpha:=alpha,
            showGird:=True,
            xlabel:=xlab,
            ylabel:=ylab,
            driver:=env.getDriver
        )
    End Function

    ''' <summary>  
    ''' plot single TIC
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' 
    <RGenericOverloads("plot")>
    Private Function plotChromatogram(x As Chromatogram, args As list, env As Environment) As Object
        Dim isBPC As Boolean = args.getValue("bpc", env, [default]:=False)
        Dim data As ChromatogramTick() = x.GetTicks(isBPC).ToArray

        Return data.plotTIC(args, env)
    End Function

    <RGenericOverloads("plot")>
    Private Function plotSignal(x As GeneralSignal, args As list, env As Environment) As Object
        Return plotSignal2({x}, args, env)
    End Function

    <RGenericOverloads("plot")>
    Private Function plotSignal2(x As GeneralSignal(), args As list, env As Environment) As Object
        Return PlotUVSignals(x, env:=env)
    End Function

    ''' <summary>
    ''' plot spectrum
    ''' </summary>
    ''' <param name="spectrum"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <RGenericOverloads("plot")>
    Private Function plotMS(spectrum As Object, args As list, env As Environment) As Object
        Dim title As String = args.getValue(Of String)("title", env, Nothing)
        Dim mirror As Boolean = args.getValue("mirror", env, False)
        Dim annotateImages As Dictionary(Of String, Image) = args.getValue("images", env, New Dictionary(Of String, Image))
        Dim labeIntensity As Double = args.getValue("label.intensity", env, 0.2)
        Dim size As String = InteropArgumentHelper.getSize(args!size, env, "1920,900")
        Dim alignment As Object = args.getByName("alignment")
        Dim showAnnotation As Boolean = args.getValue("annotation.show", env, [default]:=True)

        If mirror OrElse Not alignment Is Nothing Then
            ' plot ms alignment mirror plot
            Return Visual.SpectrumPlot(
                spectrum:=spectrum,
                alignment:=alignment,
                title:=If(title, "Mass Spectrum Plot"),
                env:=env,
                bar_width:=args.getValue(Of Single)({"bar_width", "bar.width", "bw"}, env, [default]:=8.0),
                legend_layout:=args.getValue({"legend_layout", "legend.layout"}, env, [default]:="top-right")
            )
        Else
            Dim ms As [Variant](Of Message, LibraryMatrix) = getSpectrum(spectrum, env)

            If ms Like GetType(Message) Then
                Return ms.TryCast(Of Message)
            End If

            ' draw a single ms spectrum plot
            Return PeakAssign.DrawSpectrumPeaks(
                matrix:=ms,
                images:=annotateImages,
                labelIntensity:=labeIntensity,
                size:=size,
                title:=title Or ms.TryCast(Of LibraryMatrix).name.AsDefault,
                showAnnotationText:=showAnnotation,
                driver:=env.getDriver,
                dpi:=300
            )
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="GCxGC"></param>
    ''' <param name="metabolites">
    ''' [name, rt1, rt2]
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("gcxgc_heatmap")>
    Public Function PlotGCxGCHeatMap(GCxGC As list, metabolites As dataframe,
                                     <RRawVectorArgument> Optional rt_width As Object = "60,0.5",
                                     <RRawVectorArgument> Optional space As Object = "5,5",
                                     <RRawVectorArgument> Optional size As Object = "3600,2100",
                                     <RRawVectorArgument> Optional padding As Object = "padding: 200px 600px 250px 250px;",
                                     Optional colorSet As String = "viridis:turbo",
                                     Optional mapLevels As Integer = 64,
                                     Optional labelStyle As String = "font-style: normal; font-size: 16; font-family: " & FontFace.BookmanOldStyle & ";",
                                     Optional env As Environment = Nothing) As Object

        Dim canvas As String = InteropArgumentHelper.getSize(size, env, "3600,2100")
        Dim margin = InteropArgumentHelper.getPadding(padding, [default]:="padding: 200px 600px 250px 250px;")
        Dim margin_grid = InteropArgumentHelper.getSize(space, env, "5,5").SizeParser
        Dim rt_size = InteropArgumentHelper.getSize(rt_width, env, "5,0.5").Split(","c).Select(AddressOf Val).ToArray
        Dim samples = GCxGC.getNames _
            .Select(Function(name)
                        Return New NamedCollection(Of Chromatogram2DScan) With {
                            .name = name,
                            .value = GCxGC.getValue(Of Chromatogram2DScan())(name, env, Nothing)
                        }
                    End Function) _
            .Where(Function(d) Not d.value.IsNullOrEmpty) _
            .ToArray
        Dim names As String() = CLRVector.asCharacter(metabolites("name"))
        Dim rt1 As Double() = CLRVector.asNumeric(metabolites("rt1"))
        Dim rt2 As Double() = CLRVector.asNumeric(metabolites("rt2"))
        Dim points = names _
            .Select(Function(name, i)
                        Return New NamedValue(Of PointF)(name, New PointF(rt1(i), rt2(i)))
                    End Function) _
            .ToArray
        Dim theme As New Theme With {
            .colorSet = colorSet,
            .padding = margin,
            .axisLabelCSS = labelStyle
        }
        Dim app As New GCxGCHeatMap(
            gcxgc:=samples,
            points:=points,
            rt1:=rt_size(0),
            rt2:=rt_size(1),
            mapLevels:=mapLevels,
            marginX:=margin_grid.Width,
            marginY:=margin_grid.Height,
            theme:=theme
        )

        Return app.Plot(canvas)
    End Function

    ''' <summary>
    ''' plot raw scatter matrix based on a given sequence of ms1 scans data
    ''' </summary>
    ''' <param name="ms1_scans">
    ''' a sequence of ms1 scan data, mzkit peakset object or a mzpack data object.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("raw_scatter")>
    Public Function PlotRawScatter(<RRawVectorArgument>
                                   ms1_scans As Object,
                                   <RRawVectorArgument>
                                   Optional colorSet As Object = "darkblue,blue,skyblue,green,orange,red,darkred",
                                   Optional contour As Boolean = False,
                                   <RRawVectorArgument(GetType(String))>
                                   Optional dimension As Object = "default|sum|mean|max|npeaks|<sample_name>",
                                   Optional dpi As Integer = 300,
                                   Optional env As Environment = Nothing) As Object

        Dim schema As String = RColorPalette.getColorSet(colorSet)
        Dim matrix As ms1_scan()

        If TypeOf ms1_scans Is mzPack Then
            matrix = DirectCast(ms1_scans, mzPack) _
                .GetAllScanMs1 _
                .ToArray

            Dim max As Double = Double.MaxValue

            If matrix.Any Then
                ' 20241027 try to reduce the dataset
                max = matrix.Select(Function(a) a.intensity).Max
                matrix = matrix _
                    .Where(Function(a) a.intensity / max >= 0.0001) _
                    .ToArray
            End If
        ElseIf TypeOf ms1_scans Is PeakSet Then
            matrix = DirectCast(ms1_scans, PeakSet) _
                .Ms1Scatter(CLRVector.asCharacter(dimension).DefaultFirst) _
                .ToArray
        Else
            Dim points As pipeline = pipeline.TryCreatePipeline(Of ms1_scan)(ms1_scans, env)

            If points.isError Then
                Return points.getError
            Else
                matrix = points _
                    .populates(Of ms1_scan)(env) _
                    .ToArray
            End If
        End If

        If contour Then
            ' contour
            Return ScanContour.Plot(matrix, colorSet:=schema)
        Else
            ' scatter
            Return RawScatterPlot.Plot(
                samples:=matrix,
                sampleColors:=schema,
                mapLevels:=100,
                ppi:=dpi,
                driver:=env.getDriver
            )
        End If
    End Function

    <Extension>
    Private Function assembleOverlaps(points As pipeline,
                                      mzErr As [Variant](Of Tolerance, Message),
                                      noise_cutoff As Double,
                                      env As Environment) As ChromatogramOverlap

        Dim XIC As New ChromatogramOverlap
        Dim scan As ms1_scan()
        Dim chr As Chromatogram
        Dim rawPoints As ms1_scan() = points _
            .populates(Of ms1_scan)(env) _
            .ToArray
        Dim cutoff As Double = rawPoints _
            .Select(Function(a) a.intensity) _
            .GKQuantile _
            .Query(noise_cutoff)

        For Each mz As NamedCollection(Of ms1_scan) In rawPoints _
            .GroupBy(Function(p) p.mz, mzErr.TryCast(Of Tolerance)) _
            .OrderBy(Function(mzi)
                         Return Val(mzi.name)
                     End Function)

            scan = mz _
                .Where(Function(p) p.intensity >= cutoff) _
                .OrderBy(Function(p) p.scan_time) _
                .ToArray
            chr = New Chromatogram With {
                .scan_time = scan.Select(Function(x) x.scan_time).ToArray,
                .BPC = scan.Select(Function(x) x.intensity).ToArray,
                .TIC = scan.Select(Function(x) x.intensity).ToArray
            }

            If chr.length > 3 Then
                XIC.TIC(Val(mz.name).ToString("F4")) = chr
            End If
        Next

        Return XIC
    End Function

    ''' <summary>
    ''' plot raw XIC matrix based on a given sequence of ms1 scans data
    ''' </summary>
    ''' <param name="ms1_scans">all ms1 scan point data for create XIC overlaps</param>
    ''' <param name="mzwidth">mz tolerance for create XIC data</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("raw_snapshot3D")>
    Public Function Snapshot3D(<RRawVectorArgument>
                               ms1_scans As Object,
                               Optional mzwidth As Object = "da:0.3",
                               Optional noise_cutoff As Double = 0.5,
                               <RRawVectorArgument>
                               Optional size As Object = "1920,1200",
                               <RRawVectorArgument>
                               Optional padding As Object = "padding:100px 300px 125px 150px;",
                               Optional colors As Object = "paper",
                               Optional show_legends As Boolean = True,
                               Optional env As Environment = Nothing) As Object

        Dim points As pipeline = pipeline.TryCreatePipeline(Of ms1_scan)(ms1_scans, env)
        Dim mzErr As [Variant](Of Tolerance, Message) = Math.getTolerance(mzwidth, env)
        Dim XIC As ChromatogramOverlap

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        End If

        If points.isError Then
            If TypeOf ms1_scans Is list AndAlso DirectCast(ms1_scans, list).data _
                .All(Function(xi) TypeOf xi Is MzGroup) Then

                XIC = New ChromatogramOverlap

                For Each group In DirectCast(ms1_scans, list).AsGeneric(Of MzGroup)(env)
                    XIC(group.Key) = group.Value.CreateChromatogram
                Next
            ElseIf TypeOf ms1_scans Is ChromatogramOverlap Then
                XIC = DirectCast(ms1_scans, ChromatogramOverlap)
            Else
                Return points.getError
            End If
        Else
            XIC = points.assembleOverlaps(mzErr, noise_cutoff, env)
        End If

        Dim args As New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"show.labels", False},
                {"show.legends", show_legends},
                {"parallel", True},
                {"colors", colors},
                {"opacity", 60},
                {"size", size},
                {"padding", InteropArgumentHelper.getPadding(padding, "padding:100px 300px 125px 150px;")}
            }
        }

        Return XIC.plotOverlaps(args, env)
    End Function

    ''' <summary>
    ''' Parse the spectrum alignment details from the given string data
    ''' </summary>
    ''' <param name="s">
    ''' A string data that represents the spectrum alignment details.
    ''' </param>
    ''' <returns>
    ''' A collection of the <see cref="SSM2MatrixFragment"/> matrix data
    ''' </returns>
    <ExportAPI("parse.spectrum_alignment")>
    Public Function ParseSpectrumAlignment(s As String) As AlignmentOutput
        Return AlignmentOutput.ParseAlignment(s)
    End Function

    ''' <summary>
    ''' Plot of the mass spectrum
    ''' </summary>
    ''' <param name="spectrum">
    ''' the ms spectrum object, this parameter can be a collection 
    ''' of ms2 object model, or else is a library matrix or peak 
    ''' ms2 model object, or else is a mgf ion object, or else a 
    ''' dataframe with columns ``mz`` and ``into``.
    ''' </param>
    ''' <param name="alignment"></param>
    ''' <param name="title">the main title that display on the chart plot</param>
    ''' <param name="bar_width">
    ''' the column width of the bar plot
    ''' </param>
    ''' <param name="intoCutoff">
    ''' the intensity cutoff value for display the m/z label on the bar
    ''' </param>
    ''' <param name="legend_layout">
    ''' the layout of the legend plot, this parameter value could affects the plot style
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("mass_spectrum.plot")>
    <RApiReturn(GetType(GraphicsData))>
    Public Function SpectrumPlot(spectrum As Object,
                                 Optional alignment As Object = Nothing,
                                 Optional title$ = "Mass Spectrum Plot",
                                 Optional showLegend As Boolean = True,
                                 Optional showGrid As Boolean = True,
                                 Optional tagXFormat$ = "F2",
                                 Optional intoCutoff# = 0.3,
                                 Optional bar_width As Single = 8,
                                 Optional color1 As String = AlignmentPlot.DefaultColor1,
                                 Optional color2 As String = AlignmentPlot.DefaultColor2,
                                 Optional grid_x As Boolean = False,
                                 <RRawVectorArgument(GetType(String))>
                                 Optional legend_layout As Object = "top-right|title|bottom|none",
                                 Optional gridStrokeX As String = PlotAlignmentGroup.DefaultGridXStroke,
                                 Optional gridStrokeY As String = PlotAlignmentGroup.DefaultGridYStroke,
                                 Optional highlight_msn As String = Nothing,
                                 Optional env As Environment = Nothing) As Object

        Dim ms As [Variant](Of Message, LibraryMatrix) = getSpectrum(spectrum, env)
        Dim layouts As String() = CLRVector.asCharacter(legend_layout)
        Dim highlights As NamedValue(Of Double)() = Nothing
        Dim highlightStyle As String = Stroke.StrongHighlightStroke

        If ms Like GetType(Message) Then
            Return ms.TryCast(Of Message)
        End If

        If alignment Is Nothing Then
            Return ms _
                .TryCast(Of LibraryMatrix) _
                .MirrorPlot(
                    plotTitle:=title,
                    drawGrid:=showGrid,
                    tagXFormat:=tagXFormat,
                    labelDisplayIntensity:=intoCutoff,
                    titles:={
                        ms.TryCast(Of LibraryMatrix).name,
                        spectrum.ToString
                    },
                    driver:=env.getDriver,
                    bw:=bar_width,
                    legendLayout:=layouts.ElementAtOrDefault(0, "top-right"),
                    color1:=color1, color2:=color2,
                    DrawGridX:=grid_x,
                    gridStrokeX:=gridStrokeX,
                    gridStrokeY:=gridStrokeY
                )
        Else
            Dim ref As [Variant](Of Message, LibraryMatrix) = getSpectrum(alignment, env)

            If ref Like GetType(Message) Then
                Return ref.TryCast(Of Message)
            End If
            If Not highlight_msn.StringEmpty(, True) Then
                highlightStyle = $"stroke: {highlight_msn}; stroke-width: {bar_width}px; stroke-dash: solid;"
                highlights = ms.TryCast(Of LibraryMatrix).Array _
                    .JoinIterates(ref.TryCast(Of LibraryMatrix).Array) _
                    .Where(Function(mzi)
                               Return Not Strings.Trim(mzi.Annotation).Match("MS\d+", RegexICSng).StringEmpty(, True)
                           End Function) _
                    .Select(Function(i) New NamedValue(Of Double)(i.Annotation.Match("MS\d+", RegexICSng), i.mz)) _
                    .Distinct _
                    .ToArray
            End If

            Return MassSpectra.AlignMirrorPlot(
                query:=ms,
                ref:=ref,
                title:=title,
                drawLegend:=showLegend,
                drawGrid:=showGrid,
                tagXFormat:=tagXFormat,
                labelDisplayIntensity:=intoCutoff,
                bw:=bar_width,
                legendLayout:=layouts.ElementAtOrDefault(0, "top-right"),
                drawGridX:=grid_x,
                color1:=color1,
                color2:=color2,
                gridStrokeX:=gridStrokeX,
                gridStrokeY:=gridStrokeY,
                highlights:=highlights,
                highlightStyle:=highlightStyle
            )
        End If
    End Function

    ''' <summary>
    ''' visual of the UV spectrum
    ''' </summary>
    ''' <param name="timeSignals">should be a collection of the signal data: <see cref="GeneralSignal"/></param>
    ''' <param name="is_spectrum"></param>
    ''' <param name="size"></param>
    ''' <param name="padding"></param>
    ''' <param name="colorSet"></param>
    ''' <param name="pt_size"></param>
    ''' <param name="line_width"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("plot.UV_signals")>
    <RApiReturn(GetType(GraphicsData))>
    Public Function PlotUVSignals(<RRawVectorArgument>
                                  timeSignals As Object,
                                  Optional is_spectrum As Boolean = False,
                                  Optional size As Object = "1600,1200",
                                  Optional padding As Object = g.DefaultPadding,
                                  Optional colorSet As String = "Set1:c8",
                                  Optional pt_size As Single = 8,
                                  Optional line_width As Single = 5,
                                  Optional env As Environment = Nothing) As Object

        Dim signals As pipeline = pipeline.TryCreatePipeline(Of GeneralSignal)(timeSignals, env)

        If signals.isError Then
            Return signals.getError
        End If

        Dim legendTitle As Func(Of Dictionary(Of String, String), String)

        If is_spectrum Then
            legendTitle = Function(a) a!scan_time & " sec"
        Else
            legendTitle = Function(a) a!wavelength & " nm"
        End If

        Return UVsignalPlot.Plot(
            signals:=signals.populates(Of GeneralSignal)(env),
            size:=InteropArgumentHelper.getSize(size, env),
            padding:=InteropArgumentHelper.getPadding(padding),
            colorSet:=colorSet,
            pt_size:=pt_size,
            line_width:=line_width,
            legendTitle:=legendTitle
        )
    End Function

    ''' <summary>
    ''' Plot cluster size histogram on RT dimension
    ''' </summary>
    ''' <param name="mn">the molecular networking graph result</param>
    ''' <param name="size"></param>
    ''' <param name="padding"></param>
    ''' <param name="dpi"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("plotNetworkClusterHistogram")>
    Public Function plotMolecularNetworkingHistogram(mn As NetworkGraph,
                                                     <RRawVectorArgument> Optional size As Object = "2700,2000",
                                                     <RRawVectorArgument> Optional padding As Object = "padding:100px 300px 200px 200px;",
                                                     Optional dpi As Integer = 300,
                                                     Optional env As Environment = Nothing) As Object
        Dim theme As New Theme With {
            .padding = InteropArgumentHelper.getPadding(padding, [default]:=g.DefaultPadding),
            .YaxisTickFormat = "F0",
            .XaxisTickFormat = "F0"
        }
        Dim app As New PlotMNHist(mn, theme) With {
            .xlabel = "retention time",
            .ylabel = "histogram"
        }

        Return app.Plot(size:=InteropArgumentHelper.getSize(size, env, [default]:="2700,2000"), ppi:=dpi)
    End Function

    ''' <summary>
    ''' make visualization of the XIC scatters
    ''' </summary>
    ''' <param name="ms1"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("xic_scatter_density")>
    Public Function XicScatterDensity(<RRawVectorArgument> ms1 As Object,
                                      Optional mz As Double? = Nothing,
                                      Optional mass_err As Object = "ppm:20",
                                      <RListObjectArgument>
                                      Optional args As list = Nothing,
                                      Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of ms1_scan)(ms1, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim scatter = pull.populates(Of ms1_scan)(env).ToArray
        Dim mass_win = Math.getTolerance(mass_err, env)

        If mass_win Like GetType(Message) Then
            Return mass_win.TryCast(Of Message)
        End If

        If mz Is Nothing Then
            mz = scatter.Select(Function(a) a.mz).Average
        End If

        Dim theme As New Theme
        Dim app As New PlotMassWindowXIC(scatter, CDbl(mz), mass_win.TryCast(Of Tolerance), theme)
        Dim dpi As Integer = graphicsPipeline.getDpi(args.slots, env, [default]:=100)
        Dim size As SizeF = graphicsPipeline.getSize(args, env, New SizeF(3600, 2700))

        Return app.Plot($"{size.Width},{size.Height}", dpi, env.getDriver)
    End Function
End Module
