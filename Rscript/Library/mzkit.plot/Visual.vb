#Region "Microsoft.VisualBasic::31e4946c91456505885032ecbb6de299, Library\mzkit.plot\Visual.vb"

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

' Module Visual
' 
'     Function: addOverlaps, getSpectrum, plotChromatogram, plotChromatogram2, plotMS
'               PlotRawScatter, plotSignal, plotSignal2, PlotUVSignals, SpectrumPlot
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("visual")>
Module Visual

    Sub Main()
        Call Internal.generic.add("plot", GetType(GeneralSignal), AddressOf plotSignal)
        Call Internal.generic.add("plot", GetType(GeneralSignal()), AddressOf plotSignal2)
        Call Internal.generic.add("plot", GetType(MGF.Ions), AddressOf plotMS)
        Call Internal.generic.add("plot", GetType(PeakMs2), AddressOf plotMS)
        Call Internal.generic.add("plot", GetType(LibraryMatrix), AddressOf plotMS)
        Call Internal.generic.add("plot", GetType(Chromatogram), AddressOf plotChromatogram)
        Call Internal.generic.add("plot", GetType(ChromatogramOverlap), AddressOf plotOverlaps)
    End Sub

    ''' <summary>
    ''' plot TIC overlaps
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' 
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
        Dim padding As String = args.getValue("padding", env, "padding:100px 100px 150px 250px;")
        Dim axisLabel As String = args.getValue("axis.cex", env, "font-style: normal; font-size: 24; font-family: Bookman Old Style;")
        Dim axisTickCex As String = args.getValue("tick.cex", env, "font-style: normal; font-size: 16; font-family: Bookman Old Style;")
        Dim legendLabel As String = args.getValue("legend.cex", env, "font-style: normal; font-size: 12; font-family: Bookman Old Style;")
        Dim size As String = args.getValue("size", env, "1600,1000")
        Dim xlab As String = args.getValue("xlab", env, "Time (s)")
        Dim ylab As String = args.getValue("ylab", env, "Intensity")
        Dim reorderOverlaps As Boolean = args.getValue("reorder.overlaps", env, [default]:=False)
        Dim overlaps As New List(Of NamedCollection(Of ChromatogramTick))
        Dim data As NamedCollection(Of ChromatogramTick)

        For Each raw In x.overlaps
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
                showLegends:=showLegends
            )
    End Function

    ''' <summary>
    ''' plot single TIC
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    Private Function plotChromatogram(x As Chromatogram, args As list, env As Environment) As Object
        Dim isBPC As Boolean = args.getValue("bpc", env, [default]:=False)
        Dim name As String = args.getValue("name", env, [default]:="unknown")
        Dim color As String = args.getValue("color", env, [default]:="skyblue")
        Dim gridFill As String = args.getValue("grid.fill", env, [default]:="white")
        Dim alpha As Integer = args.getValue("opacity", env, [default]:=100)
        Dim xlab As String = args.getValue("xlab", env, "Time (s)")
        Dim ylab As String = args.getValue("ylab", env, "Intensity")
        Dim data As New NamedCollection(Of ChromatogramTick) With {
            .name = name,
            .value = x.GetTicks(isBPC).ToArray
        }

        Return data.TICplot(
            colorsSchema:=color,
            gridFill:=gridFill,
            fillAlpha:=alpha,
            showGird:=True,
            xlabel:=xlab,
            ylabel:=ylab
        )
    End Function

    Private Function plotSignal(x As GeneralSignal, args As list, env As Environment) As Object
        Return plotSignal2({x}, args, env)
    End Function

    Private Function plotSignal2(x As GeneralSignal(), args As list, env As Environment) As Object
        Return PlotUVSignals(x, env:=env)
    End Function

    Private Function plotMS(spectrum As Object, args As list, env As Environment) As Object
        Dim title As String = args.getValue("title", env, "Mass Spectrum Plot")

        Return SpectrumPlot(spectrum, title:=title)
    End Function

    ''' <summary>
    ''' plot raw scatter matrix based on a given sequence of ms1 scans data
    ''' </summary>
    ''' <param name="ms1_scans">
    ''' a sequence of ms1 scan data.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("raw_scatter")>
    Public Function PlotRawScatter(<RRawVectorArgument>
                                   ms1_scans As Object,
                                   Optional env As Environment = Nothing) As Object

        Dim points As pipeline = pipeline.TryCreatePipeline(Of ms1_scan)(ms1_scans, env)

        If points.isError Then
            Return points.getError
        End If

        Return RawScatterPlot.Plot(points.populates(Of ms1_scan)(env))
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
                               Optional env As Environment = Nothing) As Object

        Dim points As pipeline = pipeline.TryCreatePipeline(Of ms1_scan)(ms1_scans, env)
        Dim mzErr = Math.getTolerance(mzwidth, env)

        If points.isError Then
            Return points.getError
        ElseIf mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        End If

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

        Dim args As New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"show.labels", False},
                {"show.legends", False},
                {"parallel", True},
                {"colors", "YlGnBu:c8"},
                {"opacity", 60}
            }
        }

        Return XIC.plotOverlaps(args, env)
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
                                 Optional env As Environment = Nothing) As Object

        Dim ms As [Variant](Of Message, LibraryMatrix) = getSpectrum(spectrum, env)

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
                    }
                )
        Else
            Dim ref As [Variant](Of Message, LibraryMatrix) = getSpectrum(alignment, env)

            If ref Like GetType(Message) Then
                Return ref.TryCast(Of Message)
            End If

            Return MassSpectra.AlignMirrorPlot(
                query:=ms,
                ref:=ref,
                title:=title,
                drawLegend:=showLegend,
                drawGrid:=showGrid,
                tagXFormat:=tagXFormat,
                labelDisplayIntensity:=intoCutoff
            )
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data">
    ''' <see cref="ms2"/>[], <see cref="LibraryMatrix"/>, <see cref="MGF.Ions"/>, <see cref="PeakMs2"/> and <see cref="dataframe"/>
    ''' 
    ''' <see cref="dataframe"/> object should contains 
    ''' ``mz`` and ``into`` these two column data at 
    ''' least.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    Private Function getSpectrum(data As Object, env As Environment) As [Variant](Of Message, LibraryMatrix)
        Dim type As Type = data.GetType

        Select Case type
            Case GetType(ms2())
                Return New LibraryMatrix With {.ms2 = data, .name = "Mass Spectrum"}
            Case GetType(LibraryMatrix)
                Return DirectCast(data, LibraryMatrix)
            Case GetType(MGF.Ions)
                Return DirectCast(data, MGF.Ions).GetLibrary
            Case GetType(dataframe)
                Dim matrix As dataframe = DirectCast(data, dataframe)

                If Not matrix.hasName("mz") Then
                    Return Message.SymbolNotFound(env, "mz", TypeCodes.double)
                ElseIf Not matrix.hasName("into") Then
                    Return Message.SymbolNotFound(env, "into", TypeCodes.double)
                End If

                Dim mz As Double() = REnv.asVector(Of Double)(matrix.getColumnVector("mz"))
                Dim into As Double() = REnv.asVector(Of Double)(matrix.getColumnVector("into"))
                Dim ms2 As ms2() = mz _
                    .Select(Function(m, i)
                                Return New ms2 With {
                                    .mz = m,
                                    .intensity = into(i)
                                }
                            End Function) _
                    .ToArray

                Return New LibraryMatrix With {.ms2 = ms2, .name = "Mass Spectrum"}
            Case GetType(PeakMs2)
                Dim name As String = DirectCast(data, PeakMs2).lib_guid

                If name.StringEmpty Then
                    name = $"M{CInt(DirectCast(data, PeakMs2).mz)}T{CInt(DirectCast(data, PeakMs2).rt) + 1}"
                End If

                Return New LibraryMatrix With {
                    .ms2 = DirectCast(data, PeakMs2).mzInto,
                    .name = name
                }
            Case Else
                Return Internal.debug.stop(New NotImplementedException(type.FullName), env)
        End Select
    End Function

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
            size:=InteropArgumentHelper.getSize(size),
            padding:=InteropArgumentHelper.getPadding(padding),
            colorSet:=colorSet,
            pt_size:=pt_size,
            line_width:=line_width,
            legendTitle:=legendTitle
        )
    End Function
End Module
