#Region "Microsoft.VisualBasic::b24d9d667f01098cfe16b08e0d90d9e0, Rscript\Library\mzkit.plot\MsImaging.vb"

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

' Module MsImaging
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: AutoScaleMax, averageStep, FilterMz, flatten, GetIonLayer
'               getMSIIons, GetMsMatrx, GetPixel, layer, LoadPixels
'               MSICoverage, openIndexedCacheFile, plotMSI, quartileRange, renderRowScans
'               RGB, testLayer, viewer, writeIndexCacheFile, WriteXICCache
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Imaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.IndexedCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.DataMining.DensityQuery
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Invokes
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' Visual MS imaging data(*.imzML)
''' </summary>
<Package("MsImaging")>
Module MsImaging

    Sub New()
        Call Internal.generic.add("plot", GetType(SingleIonLayer), AddressOf plotMSI)
    End Sub

    ''' <summary>
    ''' do MSI rendering
    ''' </summary>
    ''' <param name="ion"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    Private Function plotMSI(ion As SingleIonLayer, args As list, env As Environment) As Object
        Dim theme As New Theme With {
            .padding = InteropArgumentHelper.getPadding(args!padding, "padding: 100px 700px 300px 300px"),
            .gridFill = RColorPalette.getColor(args.getByName("grid.fill"), "white"),
            .colorSet = RColorPalette.getColorSet(args.getByName("colorSet"), "Jet")
        }
        Dim cutoff As Double() = args.getValue("into.cutoff", env, {0.1, 0.75})
        Dim scale As String = InteropArgumentHelper.getSize(args!scale, env, "8,8")
        Dim pixelDrawer As Boolean = args.getValue("pixelDrawer", env, False)
        Dim region As String() = args.getValue(Of String())("region", env, Nothing)

        If Not region.IsNullOrEmpty Then
            Dim xy As Double()() = region _
                .Select(Function(s)
                            Return s _
                                .Split(","c) _
                                .Select(AddressOf Val) _
                                .ToArray
                        End Function) _
                .ToArray
            Dim x As Double() = xy.Select(Function(r) r(0)).ToArray
            Dim y As Double() = xy.Select(Function(r) r(1)).ToArray
            Dim polygon As New Polygon2D(x, y)

            ion = ion.Take(polygon, scale.SizeParser)
        End If

        Dim app As New MSIPlot(ion, scale.SizeParser, cutoff, pixelDrawer, theme)
        Dim size As Size = app.MeasureSize

        Return app.Plot($"{size.Width},{size.Height}")
    End Function

    ''' <summary>
    ''' Contrast optimization of mass
    ''' spectrometry imaging(MSI) data
    ''' visualization by threshold intensity
    ''' quantization (TrIQ)
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="q"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("TrIQ")>
    <RApiReturn(GetType(Double))>
    Public Function TrIQRange(<RRawVectorArgument>
                              data As Object,
                              Optional q As Double = 0.6,
                              Optional levels As Integer = 100,
                              Optional env As Environment = Nothing) As Object

        Dim TrIQ As New TrIQThreshold With {.levels = levels}
        Dim into As Double()

        If TypeOf data Is SingleIonLayer Then
            into = DirectCast(data, SingleIonLayer).GetIntensity
        Else
            Dim dataPip As pipeline = pipeline.TryCreatePipeline(Of Double)(data, env)

            If dataPip.isError Then
                Return dataPip.getError
            Else
                into = dataPip.populates(Of Double)(env).ToArray
            End If
        End If

        Dim range As Double = TrIQ.ThresholdValue(into, qcut:=q)

        Return {0, range}
    End Function

    <ExportAPI("write.MSI_XIC")>
    <RApiReturn(GetType(XICWriter))>
    Public Function WriteXICCache(<RRawVectorArgument> pixels As Object, ibd As ibdReader, Optional env As Environment = Nothing) As Object
        Dim pixelData As pipeline = pipeline.TryCreatePipeline(Of ScanData)(pixels, env)

        If pixelData.isError Then
            Return pixelData.getError
        End If

        Dim allPixels As ScanData() = pixelData.populates(Of ScanData)(env).ToArray
        Dim width As Integer = Aggregate p In allPixels Into Max(p.x)
        Dim height As Integer = Aggregate p In allPixels Into Max(p.y)
        Dim cache As New XICWriter(width, height, sourceName:=ibd.fileName Or "n/a".AsDefault)

        For Each pixel As ScanData In allPixels
            Call cache.WritePixels(New ibdPixel(ibd, pixel))
        Next

        Call cache.Flush()

        Return cache
    End Function

    <ExportAPI("write.MSI")>
    Public Function writeIndexCacheFile(cache As XICWriter, file As Object, Optional env As Environment = Nothing) As Object
        If file Is Nothing Then
            Return Internal.debug.stop("the required target file can not be nothing!", env)
        End If

        Dim stream As Stream
        Dim autoClose As Boolean = False

        If TypeOf file Is String Then
            stream = DirectCast(file, String).Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            autoClose = True
        ElseIf TypeOf file Is Stream Then
            stream = file
        Else
            Return Message.InCompatibleType(GetType(Stream), file.GetType, env)
        End If

        Call XICIndex.WriteIndexFile(cache, stream)
        'Call stream.Flush()

        'If autoClose Then
        '    Call stream.Close()
        'End If

        Return True
    End Function

    <ExportAPI("open.MSI")>
    <RApiReturn(GetType(XICReader))>
    Public Function openIndexedCacheFile(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim stream = ApiArgumentHelpers.GetFileStream(file, FileAccess.Read, env)

        If stream Like GetType(Message) Then
            Return stream.TryCast(Of Message)
        End If

        Return New XICReader(stream.TryCast(Of Stream))
    End Function

    <ExportAPI("FilterMz")>
    <RApiReturn(GetType(LibraryMatrix))>
    Public Function FilterMz(viewer As Drawer, mz As Double(),
                             Optional tolerance As Object = "ppm:20",
                             Optional title As String = "FilterMz",
                             Optional env As Environment = Nothing) As Object

        Dim errors As [Variant](Of Tolerance, Message) = Math.getTolerance(tolerance, env)

        If errors Like GetType(Message) Then
            Return errors.TryCast(Of Message)
        End If

        Dim rawPixels As PixelScan() = viewer.pixelReader _
            .FindMatchedPixels(mz, errors.TryCast(Of Tolerance)) _
            .ToArray
        Dim ms1 As ms2() = rawPixels _
            .Select(Function(p) p.GetMs) _
            .IteratesALL _
            .ToArray

        Return New LibraryMatrix With {
            .centroid = True,
            .ms2 = ms1 _
                .Centroid(errors.TryCast(Of Tolerance), New RelativeIntensityCutoff(0.01)) _
                .ToArray,
            .name = title
        }
    End Function

    <ExportAPI("MS1")>
    <RApiReturn(GetType(LibraryMatrix))>
    Public Function GetMsMatrx(viewer As Drawer, x As Integer(), y As Integer(),
                               Optional tolerance As Object = "da:0.1",
                               Optional threshold As Double = 0.01,
                               Optional env As Environment = Nothing) As Object

        Dim ms As New List(Of ms2)
        Dim errors As [Variant](Of Tolerance, Message) = Math.getTolerance(tolerance, env)

        If errors Like GetType(Message) Then
            Return errors.TryCast(Of Message)
        ElseIf x.Length <> y.Length Then
            Return Internal.debug.stop("the vector size of x should be equals to vector y!", env)
        End If

        For i As Integer = 0 To x.Length - 1
            ms += viewer.ReadXY(x(i), y(i))
        Next

        Return New LibraryMatrix With {
            .centroid = True,
            .ms2 = ms.ToArray _
                .Centroid(errors.TryCast(Of Tolerance), New RelativeIntensityCutoff(threshold)) _
                .ToArray,
            .name = "MS1"
        }
    End Function

    ''' <summary>
    ''' load imzML data into the ms-imaging render
    ''' </summary>
    ''' <param name="file">
    ''' *.imzML;*.mzPack
    ''' </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' this function will load entire MSI matrix raw data into memory.
    ''' </remarks>
    <ExportAPI("viewer")>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <RApiReturn(GetType(Drawer))>
    Public Function viewer(file As Object, Optional env As Environment = Nothing) As Object
        If file Is Nothing Then
            Return Internal.debug.stop("the required file data can not be nothing!", env)
        ElseIf TypeOf file Is String Then
            Return New Drawer(file:=DirectCast(file, String))
        ElseIf TypeOf file Is mzPack Then
            Return New Drawer(DirectCast(file, mzPack))
        Else
            Return Message.InCompatibleType(GetType(mzPack), file.GetType, env)
        End If
    End Function

    <ExportAPI("pixel")>
    Public Function GetPixel(data As XICReader, x As Integer, y As Integer) As ibdPixel
        Return data.GetPixel(x, y)
    End Function

    ''' <summary>
    ''' load the raw pixels data from imzML file 
    ''' </summary>
    ''' <param name="mz">a collection of ion m/z value for rendering on one image</param>
    ''' <param name="tolerance">m/z tolerance error for get layer data</param>
    ''' <param name="skip_zero"></param>
    ''' <returns></returns>
    <ExportAPI("ionLayers")>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <RApiReturn(GetType(PixelData))>
    Public Function LoadPixels(imzML As Object, mz As Double(),
                               Optional tolerance As Object = "da:0.1",
                               Optional skip_zero As Boolean = True,
                               Optional env As Environment = Nothing) As pipeline

        Dim errors As [Variant](Of Tolerance, Message) = Math.getTolerance(tolerance, env)

        If errors Like GetType(Message) Then
            Return errors.TryCast(Of Message)
        End If

        If imzML Is Nothing Then
            Return Internal.debug.stop("the required imzML data can not be nothing!", env)
        ElseIf TypeOf imzML Is Drawer Then
            Return DirectCast(imzML, Drawer) _
                .LoadPixels(mz, errors, skip_zero) _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        ElseIf TypeOf imzML Is XICReader Then
            Return mz _
                .Select(Function(mzi)
                            Return DirectCast(imzML, XICReader).GetIonLayer(mzi, errors)
                        End Function) _
                .IteratesALL _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        Else
            Return Message.InCompatibleType(GetType(Drawer), imzML.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' rendering ions MSI in (R,G,B) color channels
    ''' </summary>
    ''' <param name="viewer"></param>
    ''' <param name="r"></param>
    ''' <param name="g"></param>
    ''' <param name="b"></param>
    ''' <param name="pixelSize"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("rgb")>
    <RApiReturn(GetType(Bitmap))>
    Public Function RGB(viewer As Drawer, r As Double, g As Double, b As Double,
                        Optional background As String = "black",
                        <RRawVectorArgument>
                        Optional pixelSize As Object = "5,5",
                        Optional tolerance As Object = "da:0.1",
                        Optional pixelDrawer As Boolean = True,
                        Optional maxCut As Double = 0.75,
                        Optional TrIQ As Boolean = True,
                        Optional env As Environment = Nothing) As Object

        Dim errors As [Variant](Of Tolerance, Message) = Math.getTolerance(tolerance, env)
        Dim psize As Size = InteropArgumentHelper.getSize(pixelSize, env, "5,5").SizeParser

        If errors Like GetType(Message) Then
            Return errors.TryCast(Of Message)
        End If

        Dim pr As PixelData() = viewer.LoadPixels({r}, errors.TryCast(Of Tolerance)).ToArray
        Dim pg As PixelData() = viewer.LoadPixels({g}, errors.TryCast(Of Tolerance)).ToArray
        Dim pb As PixelData() = viewer.LoadPixels({b}, errors.TryCast(Of Tolerance)).ToArray
        Dim engine As Renderer = If(pixelDrawer, New PixelRender(heatmapRender:=False), New RectangleRender(heatmapRender:=False))
        Dim qcut As QuantizationThreshold = If(TrIQ, New TrIQThreshold(maxCut), New RankQuantileThreshold(maxCut))
        Dim cut As IQuantizationThreshold = AddressOf qcut.ThresholdValue
        Dim qr As DoubleRange = {0, cut(pr.Select(Function(p) p.intensity).ToArray)}
        Dim qg As DoubleRange = {0, cut(pg.Select(Function(p) p.intensity).ToArray)}
        Dim qb As DoubleRange = {0, cut(pb.Select(Function(p) p.intensity).ToArray)}

        Return engine.ChannelCompositions(pr, pg, pb, viewer.dimension, psize, cut:=(qr, qg, qb), background:=background)
    End Function

    ''' <summary>
    ''' get MSI pixels layer via given ``m/z`` value. 
    ''' </summary>
    ''' <param name="viewer"></param>
    ''' <param name="mz"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("MSIlayer")>
    <RApiReturn(GetType(SingleIonLayer))>
    Public Function GetIonLayer(viewer As Drawer, mz As Double,
                                Optional tolerance As Object = "da:0.1",
                                Optional env As Environment = Nothing) As Object

        Dim mzErr As [Variant](Of Message, Tolerance) = Math.getTolerance(tolerance, env)

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        Else
            Return SingleIonLayer.GetLayer(mz, viewer, mzErr.TryCast(Of Tolerance))
        End If
    End Function

    <ExportAPI("intensity")>
    <RApiReturn(GetType(Double))>
    Public Function GetIntensityData(layer As Object,
                                     Optional summary As IntensitySummary = IntensitySummary.Total,
                                     Optional env As Environment = Nothing) As Object

        If TypeOf layer Is SingleIonLayer Then
            Return DirectCast(layer, SingleIonLayer).GetIntensity
        ElseIf TypeOf layer Is MSISummary Then
            Return DirectCast(layer, MSISummary).GetLayer(summary).Select(Function(i) i.totalIon).ToArray
        Else
            Return Message.InCompatibleType(GetType(SingleIonLayer), layer.GetType, envir:=env)
        End If
    End Function

    <ExportAPI("knnFill")>
    <RApiReturn(GetType(SingleIonLayer), GetType(MSISummary))>
    Public Function KnnFill(layer As Object,
                            Optional gridSize As Integer = 3,
                            Optional q As Double = 0.8,
                            Optional env As Environment = Nothing) As Object

        Call base.print($"Knn interpolation fill with grid size: [{gridSize},{gridSize}]", , env)
        Call base.print($"Pixels qcut: {q}",, env)

        If TypeOf layer Is SingleIonLayer Then
            Return DirectCast(layer, SingleIonLayer).KnnFill(gridSize, gridSize, q)
        ElseIf TypeOf layer Is MSISummary Then
            Return DirectCast(layer, MSISummary).KnnFill(gridSize, gridSize, q)
        Else
            Return Message.InCompatibleType(GetType(SingleIonLayer), layer.GetType, env)
        End If
    End Function

    <ExportAPI("MSI_coverage")>
    <Extension>
    Public Function MSICoverage(layer As SingleIonLayer, xy As Index(Of String), Optional samplingRegion As Boolean = True) As Double
        Dim layerXy As String() = layer.MSILayer.Select(Function(p) $"{p.x},{p.y}").ToArray
        Dim union As Integer = If(samplingRegion, xy.Count, xy.Objects.JoinIterates(layerXy).Distinct.Count)
        Dim intersect As Integer = layerXy.Where(Function(i) i Like xy).Count

        Return intersect / union
    End Function

    ''' <summary>
    ''' test of a given MSI layer is target? 
    ''' </summary>
    ''' <param name="layer"></param>
    ''' <param name="xy">a character vector of ``x,y``</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("assert")>
    Public Function testLayer(layer As SingleIonLayer,
                              xy As Index(Of String),
                              Optional cutoff As Double = 0.8,
                              Optional samplingRegion As Boolean = True) As Boolean

        Return layer.MSICoverage(xy, samplingRegion) >= cutoff
    End Function

    ''' <summary>
    ''' render a ms-imaging layer by a specific ``m/z`` scan.
    ''' </summary>
    ''' <param name="viewer"></param>
    ''' <param name="mz"></param>
    ''' <param name="pixelSize"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="color$"></param>
    ''' <param name="levels%"></param>
    ''' <returns></returns>
    <ExportAPI("layer")>
    <RApiReturn(GetType(Bitmap))>
    Public Function layer(viewer As Drawer, mz As Double(),
                          <RRawVectorArgument>
                          Optional pixelSize As Object = "5,5",
                          Optional tolerance As Object = "da:0.1",
                          Optional color$ = "YlGnBu:c8",
                          Optional levels% = 30,
                          <RRawVectorArgument(GetType(Double))>
                          Optional cutoff As Object = "0.1,0.75",
                          Optional pixelDrawer As Boolean = True,
                          Optional env As Environment = Nothing) As Object

        Dim errors As [Variant](Of Tolerance, Message) = Math.getTolerance(tolerance, env)

        If errors Like GetType(Message) Then
            Return errors.TryCast(Of Message)
        Else
            cutoff = ApiArgumentHelpers.GetDoubleRange(cutoff, env, "0.1,0.75")

            If TryCast(cutoff, [Variant](Of DoubleRange, Message)) Like GetType(Message) Then
                Return TryCast(cutoff, [Variant](Of DoubleRange, Message)).TryCast(Of Message)
            Else
                cutoff = TryCast(cutoff, [Variant](Of DoubleRange, Message)).TryCast(Of DoubleRange)
            End If
        End If

        If mz.IsNullOrEmpty Then
            Return Nothing
        ElseIf mz.Length = 1 Then
            Return viewer.DrawLayer(
                mz:=mz(Scan0),
                pixelSize:=InteropArgumentHelper.getSize(pixelSize, env, "5,5"),
                toleranceErr:=errors.TryCast(Of Tolerance).GetScript,
                colorSet:=color,
                mapLevels:=levels,
                cutoff:=cutoff,
                pixelDrawer:=pixelDrawer
            )
        Else
            Return viewer.DrawLayer(
                mz:=mz,
                pixelSize:=InteropArgumentHelper.getSize(pixelSize, env, "5,5"),
                toleranceErr:=errors.TryCast(Of Tolerance).GetScript,
                colorSet:=color,
                mapLevels:=levels,
                cutoff:=cutoff,
                pixelDrawer:=pixelDrawer
            )
        End If
    End Function

    <Extension>
    Private Function averageStep(dims As Double()) As Double
        If dims.IsNullOrEmpty Then
            Return 1
        End If

        Dim asc As Double() = dims.Distinct.OrderBy(Function(d) d).ToArray
        Dim deltas As New List(Of Double)
        Dim base As Double = asc(Scan0)

        For i As Integer = 1 To asc.Length - 1
            deltas.Add(asc(i) - base)
            base = asc(i)
        Next

        Return deltas.Average
    End Function

    <ExportAPI("MSI_summary.scaleMax")>
    Public Function AutoScaleMax(data As MSISummary,
                                 intensity As IntensitySummary,
                                 Optional qcut As Double = 0.75,
                                 Optional TrIQ As Boolean = True) As Double

        Dim into As Double() = data.GetLayer(intensity).Select(Function(p) p.totalIon).ToArray
        Dim cut As QuantizationThreshold = If(TrIQ, New TrIQThreshold(qcut), New RankQuantileThreshold(qcut))
        Dim scale As Double = cut.ThresholdValue(into)

        Return scale
    End Function

    ''' <summary>
    ''' MS-imaging of the MSI summary data result.
    ''' </summary>
    ''' <param name="data">
    ''' 1. <see cref="MSISummary"/>
    ''' 2. <see cref="SingleIonLayer"/>
    ''' </param>
    ''' <param name="intensity"></param>
    ''' <param name="colorSet"><see cref="ScalerPalette"/></param>
    ''' <param name="defaultFill"></param>
    ''' <param name="pixelSize"></param>
    ''' <param name="cutoff"></param>
    ''' <param name="background">
    ''' all of the pixels in this index parameter data value will 
    ''' be treated as background pixels and removed from the MSI 
    ''' rendering.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("render")>
    <RApiReturn(GetType(Bitmap))>
    Public Function renderRowScans(data As Object,
                                   Optional intensity As IntensitySummary = IntensitySummary.Total,
                                   Optional colorSet$ = "viridis:turbo",
                                   Optional defaultFill As String = "Transparent",
                                   <RRawVectorArgument>
                                   Optional pixelSize As Object = "6,6",
                                   <RRawVectorArgument(GetType(Double))>
                                   Optional cutoff As Object = "0.1,0.75",
                                   Optional pixelDrawer As Boolean = True,
                                   Optional background As String() = Nothing,
                                   <RRawVectorArgument>
                                   Optional dims As Object = Nothing,
                                   Optional env As Environment = Nothing) As Object

        Dim regionPts As Integer()() = background _
            .SafeQuery _
            .Where(Function(str) Not str.StringEmpty) _
            .Select(Function(str)
                        Return str _
                            .Split(","c) _
                            .Select(AddressOf Integer.Parse) _
                            .ToArray
                    End Function) _
            .ToArray
        Dim polygon As New Polygon2D(
            x:=regionPts.Select(Function(p) CDbl(p(0))).ToArray,
            y:=regionPts.Select(Function(p) CDbl(p(1))).ToArray
        )
        Dim unionSize As New Size With {
            .Width = polygon.xpoints.averageStep,
            .Height = polygon.ypoints.averageStep
        }
        Dim dataSize As Size
        Dim pixels As PixelData()

        If TypeOf data Is MSISummary Then
            Dim layer As PixelScanIntensity() = DirectCast(data, MSISummary) _
                .GetLayer(intensity) _
                .TrimRegion(polygon, unionSize) _
                .ToArray

            dataSize = DirectCast(data, MSISummary).size
            pixels = layer _
                .Select(Function(p)
                            Return New PixelData With {
                                .intensity = p.totalIon,
                                .x = p.x,
                                .y = p.y
                            }
                        End Function) _
                .ToArray
        ElseIf TypeOf data Is SingleIonLayer Then
            dataSize = DirectCast(data, SingleIonLayer).DimensionSize
            pixels = DirectCast(data, SingleIonLayer).MSILayer
        Else
            Return Message.InCompatibleType(GetType(MSISummary), data.GetType, env)
        End If

        Dim cutoffRange = ApiArgumentHelpers.GetDoubleRange(cutoff, env, "0.1,0.75")
        Dim engine As Renderer = If(pixelDrawer, New PixelRender(heatmapRender:=False), New RectangleRender(heatmapRender:=False))
        Dim dimSize As Size = InteropArgumentHelper _
            .getSize(dims, env, [default]:=$"{dataSize.Width},{dataSize.Height}") _
            .SizeParser
        Dim pointSize As Size = InteropArgumentHelper.getSize(pixelSize, env, "6,6").SizeParser

        If cutoffRange Like GetType(Message) Then
            Return cutoffRange.TryCast(Of Message)
        End If

        Return engine.RenderPixels(
            pixels:=pixels,
            dimension:=dimSize,
            dimSize:=pointSize,
            colorSet:=colorSet,
            defaultFill:=defaultFill,
            cutoff:=cutoffRange.TryCast(Of DoubleRange)
        )
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="gridSize"></param>
    ''' <param name="mzdiff"></param>
    ''' <param name="keepsLayer">
    ''' if the options is set to false, then this function just returns the ions mz vector.
    ''' otherwise, returns a dataframe that contains m/z, density value and ion layer 
    ''' objects.
    ''' </param>
    ''' <param name="densityCut"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("MeasureMSIions")>
    <RApiReturn(GetType(Double), GetType(dataframe))>
    Public Function getMSIIons(raw As mzPack,
                               Optional gridSize As Integer = 6,
                               Optional mzdiff As Object = "da:0.1",
                               Optional keepsLayer As Boolean = False,
                               Optional densityCut As Double = 0.1,
                               Optional qcut As Double = 0.01,
                               Optional intoCut As Double = 0,
                               Optional env As Environment = Nothing) As Object

        Dim mzErr = Math.getTolerance(mzdiff, env)

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        End If

        Dim layers As DoubleTagged(Of SingleIonLayer)() = raw.GetMSIIons(mzErr, gridSize, qcut:=qcut, intoCut:=intoCut).ToArray
        Dim layerCuts = layers _
            .Where(Function(d) Val(d.TagStr) > densityCut) _
            .OrderByDescending(Function(d) Val(d.TagStr)) _
            .ToArray

        If Not keepsLayer Then
            Return layerCuts.Select(Function(d) d.Tag).ToArray
        End If

        Dim mz As New List(Of Double)
        Dim density As New List(Of Double)
        Dim layerList As New List(Of SingleIonLayer)

        For Each layer As DoubleTagged(Of SingleIonLayer) In layerCuts
            mz.Add(layer.Tag)
            density.Add(layer.TagStr)
            layerList.Add(layer.Value)
        Next

        Return New dataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"mz", mz.ToArray},
                {"density", density.ToArray},
                {"layer", layerList.ToArray}
            },
            .rownames = mz _
                .Select(Function(mzi)
                            Return mzi.ToString("F4")
                        End Function) _
                .ToArray
        }
    End Function
End Module
