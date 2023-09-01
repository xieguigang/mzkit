#Region "Microsoft.VisualBasic::85adcb71e40aabb06132caa56ac913af, mzkit\Rscript\Library\mzkit.plot\MsImaging.vb"

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

'   Total Lines: 823
'    Code Lines: 559
' Comment Lines: 163
'   Blank Lines: 101
'     File Size: 33.22 KB


' Module MsImaging
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: asPixels, AutoScaleMax, averageStep, defaultFilter, FilterMz
'               GetIntensityData, GetIonLayer, getMSIIons, GetMsMatrx, GetPixel
'               KnnFill, layer, LimitIntensityRange, LoadPixels, MSICoverage
'               openIndexedCacheFile, plotMSI, printLayer, renderRowScans, RGB
'               testLayer, TrIQRange, viewer, WriteXICCache
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.IndexedCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Invokes
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports PixelData = BioNovoGene.Analytical.MassSpectrometry.MsImaging.PixelData
Imports Point2D = System.Drawing.Point

''' <summary>
''' ### Visual MS imaging data(*.imzML)
''' 
''' Mass spec imaging (MSI) is a technique measuring chemical composition and 
''' linking it to spatial coordinates on a surface.  The chemical composition
''' is determined by mass spectrometry, which measures the mass-to-charge ratios
''' (m/z's) of any ions that can be generated from the surface.  Most commonly,
''' the surface is a tissue section on a microscope slide; however, any flat 
''' surface could be analyzed given it has suitable dimensions and is properly 
''' prepared. 
''' </summary>
<Package("MsImaging")>
Module MsImaging

    Sub New()
        Call Internal.generic.add("plot", GetType(SingleIonLayer), AddressOf plotMSI)
        Call Internal.generic.add("split", GetType(SingleIonLayer), AddressOf splitLayer)

        Call Internal.ConsolePrinter.AttachConsoleFormatter(Of SingleIonLayer)(AddressOf printLayer)
    End Sub

    ''' <summary>
    ''' split the ms-imaging layer into multiple parts
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="args">
    ''' default is split layer into multiple sample source
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("split.layer")>
    Public Function splitLayer(<RRawVectorArgument> x As Object,
                               <RListObjectArgument>
                               args As list,
                               Optional env As Environment = Nothing) As Object

        If TypeOf x Is SingleIonLayer Then
            Dim layer As SingleIonLayer = x
            Dim splits = layer.MSILayer _
                .GroupBy(Function(a) a.sampleTag) _
                .ToDictionary(Function(si)
                                  Return si.Key
                              End Function,
                              Function(si)
                                  Return CObj(New SingleIonLayer With {
                                     .DimensionSize = layer.DimensionSize,
                                     .IonMz = layer.IonMz,
                                     .MSILayer = si.ToArray
                                  })
                              End Function)

            Return New list With {.slots = splits}
        Else
            Return Internal.debug.stop(New NotImplementedException, envir:=env)
        End If
    End Function

    Private Function printLayer(ion As SingleIonLayer) As String
        Dim sb As New StringBuilder
        Dim into = ion.GetIntensity

        Call sb.AppendLine($"m/z {ion.IonMz} has {ion.MSILayer.Length} pixels@[{ion.DimensionSize.Width},{ion.DimensionSize.Height}]")
        Call sb.AppendLine("----------------------------------")
        Call sb.AppendLine($"  max intensity: {into.Max}")
        Call sb.AppendLine($"  min intensity: {into.Min}")

        Return sb.ToString
    End Function

    ''' <summary>
    ''' do MSI rendering
    ''' </summary>
    ''' <param name="ion"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    Private Function plotMSI(ion As SingleIonLayer, args As list, env As Environment) As Object
        Dim theme As New Theme With {
            .padding = InteropArgumentHelper.getPadding(args!padding, "padding:200px 500px 200px 200px"),
            .gridFill = RColorPalette.getColor(args.getByName("grid.fill"), "white"),
            .colorSet = RColorPalette.getColorSet(args.getByName("colorSet"), "Jet")
        }
        Dim cutoff As Double() = args.getValue("into.cutoff", env, {0.1, 0.75})
        Dim scale As String = InteropArgumentHelper.getSize(args!scale, env, "3,3")
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

        Dim app As New MSIPlot(ion, scale.SizeParser, pixelDrawer, theme, driver:=env.getDriver)
        Dim size As Size = app.MeasureSize

        Return app.Plot($"{size.Width},{size.Height}", driver:=env.getDriver)
    End Function

    ''' <summary>
    ''' Contrast optimization of mass
    ''' spectrometry imaging(MSI) data
    ''' visualization by threshold intensity
    ''' quantization (TrIQ)
    ''' </summary>
    ''' <param name="data">
    ''' A ms-imaging ion layer data or a numeric vector of the intensity data.
    ''' </param>
    ''' <param name="q">cutoff threshold of the intensity numeric vector</param>
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

    ''' <summary>
    ''' trim the intensity data value in a pixels of a ion MS-Imaging layer
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="max"></param>
    ''' <param name="min"></param>
    ''' <returns></returns>
    <ExportAPI("intensityLimits")>
    Public Function LimitIntensityRange(data As SingleIonLayer, max As Double, Optional min As Double = 0) As SingleIonLayer
        data.MSILayer = data.MSILayer _
            .Select(Function(p)
                        If p.intensity > max Then
                            p.intensity = max
                        ElseIf p.intensity < min Then
                            p.intensity = min
                        End If

                        Return p
                    End Function) _
            .ToArray

        Return data
    End Function

    ''' <summary>
    ''' write mzImage data file
    ''' </summary>
    ''' <param name="pixels"></param>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("write.mzImage")>
    <RApiReturn(GetType(Boolean))>
    Public Function WriteXICCache(<RRawVectorArgument>
                                  pixels As Object,
                                  file As Object,
                                  Optional da As Double = 0.01,
                                  Optional spares As Double = 0.2,
                                  Optional env As Environment = Nothing) As Object


        Dim buffer = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        End If

        If TypeOf pixels Is Drawer Then
            pixels = DirectCast(pixels, Drawer).pixelReader
        End If

        If TypeOf pixels Is PixelReader Then
            Call XICPackWriter.IndexRawData(
                raw:=DirectCast(pixels, PixelReader),
                file:=buffer.TryCast(Of Stream),
                da:=da,
                spares:=spares
            )
        Else
            Dim pixelData As pipeline = pipeline.TryCreatePipeline(Of PixelScan)(pixels, env)

            If pixelData.isError Then
                Return pixelData.getError
            End If

            Dim allPixels As PixelScan() = pixelData.populates(Of PixelScan)(env).ToArray
            Dim width As Integer = Aggregate p In allPixels Into Max(p.X)
            Dim height As Integer = Aggregate p In allPixels Into Max(p.Y)
            Dim data As Stream = buffer.TryCast(Of Stream)

            Call XICPackWriter.IndexRawData(
                pixels:=allPixels,
                dims:=New Size(width, height),
                file:=data,
                da:=da,
                spares:=spares
            )
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' open the existed mzImage cache file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("read.mzImage")>
    <RApiReturn(GetType(XICReader))>
    Public Function openIndexedCacheFile(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim stream = ApiArgumentHelpers.GetFileStream(file, FileAccess.Read, env)

        If stream Like GetType(Message) Then
            Return stream.TryCast(Of Message)
        End If

        Return New XICReader(stream.TryCast(Of Stream))
    End Function

    ''' <summary>
    ''' Extract a spectrum matrix object from MSI data by a given set of m/z values
    ''' </summary>
    ''' <param name="viewer"></param>
    ''' <param name="mz"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="title"></param>
    ''' <param name="env"></param>
    ''' <returns>A spectrum matrix data of m/z value assocated with the intensity value</returns>
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

    ''' <summary>
    ''' get the ms1 spectrum data in a specific pixel position
    ''' </summary>
    ''' <param name="viewer"></param>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="threshold"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
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

    ''' <summary>
    ''' get a pixel data
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <returns></returns>
    <ExportAPI("pixel")>
    <RApiReturn(GetType(ibdPixel))>
    Public Function GetPixel(data As Object, x As Integer(), y As Integer(), Optional env As Environment = Nothing) As Object
        If x.Length = 1 AndAlso y.Length = 1 Then
            If TypeOf data Is XICReader Then
                Return DirectCast(data, XICReader).GetPixel(x(0), y(0))
            ElseIf TypeOf data Is MSISummary Then
                Return DirectCast(data, MSISummary).GetPixel(x(0), y(0))
            Else
                Return Message.InCompatibleType(GetType(XICReader), data.GetType, env)
            End If
        Else
            If TypeOf data Is XICReader Then
                With DirectCast(data, XICReader)
                    Return x _
                        .Select(Function(xi, i) .GetPixel(xi, y(i))) _
                        .ToArray
                End With
            ElseIf TypeOf data Is MSISummary Then
                With DirectCast(data, MSISummary)
                    Return x _
                        .Select(Function(xi, i) .GetPixel(xi, y(i))) _
                        .ToArray
                End With
            Else
                Return Message.InCompatibleType(GetType(XICReader), data.GetType, env)
            End If
        End If
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
                            Return DirectCast(imzML, XICReader).GetLayer(mzi, errors)
                        End Function) _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        Else
            Return Message.InCompatibleType(GetType(Drawer), imzML.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' set cluster tags to the pixel tag property data
    ''' </summary>
    ''' <param name="layer"></param>
    ''' <param name="segments"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("tag_layers")>
    Public Function tagLayers(layer As SingleIonLayer, <RRawVectorArgument> segments As Object, Optional env As Environment = Nothing) As Object
        Dim pointCluster As pipeline = pipeline.TryCreatePipeline(Of TissueRegion)(segments, env)

        If pointCluster.isError Then
            Return pointCluster.getError
        End If

        Dim raster = Grid(Of PixelData).Create(layer.MSILayer)

        For Each cluster As TissueRegion In pointCluster.populates(Of TissueRegion)(env)
            For Each point As Point2D In cluster.points
                Dim hit As Boolean = False
                Dim p As PixelData = raster.GetData(point.X, point.Y, hit:=hit)

                If hit Then
                    p.sampleTag = cluster.label
                End If
            Next
        Next

        Return layer
    End Function

    ''' <summary>
    ''' merge multiple layers via intensity sum
    ''' </summary>
    ''' <param name="layers"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("sum_layers")>
    Public Function sumLayer(<RRawVectorArgument> layers As Object,
                             Optional tolerance As Object = "da:0.1",
                             Optional intocutoff As Double = 0.3,
                             Optional env As Environment = Nothing) As Object

        Dim pixels = pipeline.TryCreatePipeline(Of PixelData)(layers, env)
        Dim mzdiff = Math.getTolerance(tolerance, env)

        If mzdiff Like GetType(Message) Then
            Return mzdiff.TryCast(Of Message)
        End If
        If pixels.isError Then
            Return pixels.getError
        End If

        Dim layer_groups = pixels _
            .populates(Of PixelData)(env) _
            .GroupBy(Function(x) x.mz, mzdiff.TryCast(Of Tolerance)) _
            .ToArray
        Dim filter As New List(Of PixelData)
        Dim all As New List(Of PixelData)

        For Each i In layer_groups
            Dim q = i.Select(Function(p) p.intensity).GKQuantile
            Dim cutoff As Double = q.Query(intocutoff)

            Call all.AddRange(i)
            Call filter.AddRange(From p As PixelData In i Where p.intensity >= cutoff)
        Next

        Dim polygon As New Polygon2D(all.ToArray)
        ' re-assembly a new layer object
        Dim layerPixels = filter.GroupBy(Function(p) $"{p.x}+{p.y}") _
            .AsParallel _
            .Select(Function(p)
                        Dim intensity As Double = Aggregate pi In p Into Sum(pi.intensity)
                        Dim copy = p.First
                        Dim tag As String = p.Select(Function(xi) xi.sampleTag) _
                            .Where(Function(si) Not si.StringEmpty) _
                            .FirstOrDefault

                        Return New PixelData With {
                            .x = copy.x,
                            .y = copy.y,
                            .sampleTag = tag,
                            .intensity = intensity,
                            .level = 0,
                            .mz = -1
                        }
                    End Function) _
            .ToArray

        Return New SingleIonLayer With {
            .IonMz = Nothing,
            .MSILayer = layerPixels,
            .DimensionSize = polygon.GetDimension
        }
    End Function

    ''' <summary>
    ''' rendering ions MSI in (R,G,B) color channels
    ''' </summary>
    ''' <param name="viewer"></param>
    ''' <param name="r"></param>
    ''' <param name="g"></param>
    ''' <param name="b"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("rgb")>
    <RApiReturn(GetType(Bitmap))>
    Public Function RGB(viewer As Drawer, r As Double, g As Double, b As Double,
                        Optional background As String = "black",
                        Optional tolerance As Object = "da:0.1",
                        Optional maxCut As Double = 0.75,
                        Optional TrIQ As Boolean = True,
                        <RRawVectorArgument(GetType(Integer))>
                        Optional pixelSize As Object = "5,5",
                        Optional env As Environment = Nothing) As Object

        Dim errors As [Variant](Of Tolerance, Message) = Math.getTolerance(tolerance, env)
        Dim psize As Size = InteropArgumentHelper.getSize(pixelSize, env, "5,5").SizeParser

        If errors Like GetType(Message) Then
            Return errors.TryCast(Of Message)
        End If

        Dim pr As PixelData() = viewer.LoadPixels({r}, errors.TryCast(Of Tolerance)).ToArray
        Dim pg As PixelData() = viewer.LoadPixels({g}, errors.TryCast(Of Tolerance)).ToArray
        Dim pb As PixelData() = viewer.LoadPixels({b}, errors.TryCast(Of Tolerance)).ToArray
        Dim engine As New RectangleRender(env.getDriver(), heatmapRender:=False)
        Dim qcut As QuantizationThreshold = If(TrIQ, New TrIQThreshold(maxCut), New RankQuantileThreshold(maxCut))
        'Dim cut As IQuantizationThreshold = AddressOf qcut.ThresholdValue
        'Dim qr As DoubleRange = {0, cut(pr.Select(Function(p) p.intensity).ToArray)}
        'Dim qg As DoubleRange = {0, cut(pg.Select(Function(p) p.intensity).ToArray)}
        'Dim qb As DoubleRange = {0, cut(pb.Select(Function(p) p.intensity).ToArray)}

        Return engine.ChannelCompositions(pr, pg, pb, viewer.dimension, background:=background)
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
    Public Function GetIonLayer(viewer As Drawer, mz As Double(),
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

    ''' <summary>
    ''' do pixel interpolation for run MS-imaging
    ''' </summary>
    ''' <param name="layer"></param>
    ''' <param name="gridSize">
    ''' knn size
    ''' </param>
    ''' <param name="q"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
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
                          Optional color$ = "viridis:turbo",
                          Optional levels% = 30,
                          <RRawVectorArgument(GetType(Double))>
                          Optional cutoff As Object = "0.1,0.75",
                          <RRawVectorArgument>
                          Optional background As Object = NameOf(Color.Transparent),
                          Optional env As Environment = Nothing) As Object

        Dim errors As [Variant](Of Tolerance, Message) = Math.getTolerance(tolerance, env)
        Dim pixel_size As Size = InteropArgumentHelper.getSize(pixelSize, env, "5,5").SizeParser

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

        Dim imaging As Image

        If mz.IsNullOrEmpty Then
            Return Nothing
        ElseIf mz.Length = 1 Then
            imaging = viewer.DrawLayer(
                mz:=mz(Scan0),
                toleranceErr:=errors.TryCast(Of Tolerance).GetScript,
                colorSet:=color,
                mapLevels:=levels,
                background:=RColorPalette.getColor(background, "Translate"),
                driver:=Drivers.GDI
            ).AsGDIImage
        Else
            imaging = viewer.DrawLayer(
                mz:=mz,
                toleranceErr:=errors.TryCast(Of Tolerance).GetScript,
                colorSet:=color,
                mapLevels:=levels,
                background:=RColorPalette.getColor(background, "Translate"),
                driver:=Drivers.GDI
            ).AsGDIImage
        End If

        Return New RasterScaler(imaging).Scale(imaging.Width * pixel_size.Width, imaging.Height * pixel_size.Height)
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
                                 Optional TrIQ As Boolean = True,
                                 Optional env As Environment = Nothing) As Double

        Dim into As Double() = data.GetLayer(intensity).Select(Function(p) p.totalIon).ToArray
        Dim cut As QuantizationThreshold = If(TrIQ, New TrIQThreshold(qcut), New RankQuantileThreshold(qcut))
        Dim println As Action(Of Object) = env.WriteLineHandler

        Call println("Create intensity cutoff with:")
        Call println(cut)

        Dim scale As Double = cut.ThresholdValue(into)

        Return scale
    End Function

    ''' <summary>
    ''' get the default ms-imaging filter pipeline
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("defaultFilter")>
    Public Function defaultFilter() As RasterPipeline
        ' denoise_scale() > TrIQ_scale(0.8) > knn_scale() > soften_scale()
        Return New DenoiseScaler() _
            .Then(New TrIQScaler) _
            .Then(New KNNScaler) _
            .Then(New SoftenScaler)
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
    ''' <param name="background">
    ''' all of the pixels in this index parameter data value will 
    ''' be treated as background pixels and removed from the MSI 
    ''' rendering.
    ''' </param>
    ''' <param name="size">
    ''' do size overrides, default parameter value nothing means the
    ''' size is evaluated based on the dimension <paramref name="dims"/> 
    ''' of the ms-imaging raw data and the <paramref name="pixelSize"/>
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
                                   Optional filter As RasterPipeline = Nothing,
                                   Optional background As String() = Nothing,
                                   <RRawVectorArgument>
                                   Optional size As Object = Nothing,
                                   Optional colorLevels As Integer = 255,
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

        Dim engine As New RectangleRender(env.getDriver, heatmapRender:=False)
        Dim dimSize As Size = InteropArgumentHelper _
            .getSize(dims, env, [default]:=$"{dataSize.Width},{dataSize.Height}") _
            .SizeParser
        Dim pointSize As Size = InteropArgumentHelper.getSize(pixelSize, env, "6,6").SizeParser

        If Not filter Is Nothing Then
            pixels = filter.DoIntensityScale(pixels, dimSize)
        End If

        Dim image As Image = engine.RenderPixels(
            pixels:=pixels,
            dimension:=dimSize,
            colorSet:=colorSet,
            defaultFill:=defaultFill,
            mapLevels:=colorLevels
        ).AsGDIImage
        Dim scaleSize As New Size(image.Width * pointSize.Width, image.Height * pointSize.Height)

        If Not size Is Nothing Then
            ' 20230109 some raw data its size aspect ratio may be very different with
            ' the size ratio 1:1
            ' this size overrides may solve this problem
            Dim sizeVal = InteropArgumentHelper.getSize(size, env, [default]:="0,0").SizeParser

            If Not sizeVal.IsEmpty Then
                scaleSize = sizeVal
            End If
        End If

        Return New RasterScaler(image).Scale(scaleSize.Width, scaleSize.Height)
    End Function

    ''' <summary>
    ''' extract the pixel [x,y] information for all of
    ''' the points in the target <paramref name="layer"/>
    ''' </summary>
    ''' <param name="layer"></param>
    ''' <param name="character">
    ''' the function will returns the character vector 
    ''' when this parameter value is set to TRUE
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("as.pixels")>
    <RApiReturn(GetType(String), GetType(Point2D))>
    Public Function asPixels(layer As SingleIonLayer, Optional character As Boolean = True) As Object
        If character Then
            Return layer.MSILayer _
                .Select(Function(p) $"{p.x},{p.y}") _
                .ToArray
        Else
            Return layer.MSILayer _
                .Select(Function(p) New Point2D(p.x, p.y)) _
                .ToArray
        End If
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
