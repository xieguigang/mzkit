#Region "Microsoft.VisualBasic::c4e0ccf253ae264cf3f9def982afe714, mzkit\Rscript\Library\mzkit\comprehensive\TissueMorphology.vb"

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

'   Total Lines: 396
'    Code Lines: 274
' Comment Lines: 78
'   Blank Lines: 44
'     File Size: 14.51 KB


' Module TissueMorphology
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: createCDF, createTissueData, createTissueTable, createUMAPsample, createUMAPTable
'               gridding, loadSpatialMapping, loadTissue, loadUMAP, SplitMapping
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.DataMining.DensityQuery
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Invokes
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports RgraphicsDev = SMRUCC.Rsharp.Runtime.Internal.Invokes.graphicsDevice
Imports std = System.Math

''' <summary>
''' spatial tissue region handler
''' 
''' tissue morphology data handler for the internal 
''' bionovogene MS-imaging analysis pipeline.
''' </summary>
<Package("TissueMorphology")>
Module TissueMorphology

    Sub New()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(TissueRegion()), AddressOf createTissueTable)
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(UMAPPoint()), AddressOf createUMAPTable)

        Call Internal.generic.add("plot", GetType(TissueRegion()), AddressOf PlotTissueMap)
    End Sub

    Private Function createTissueTable(tissues As TissueRegion(), args As list, env As Environment) As dataframe
        Dim labels As String() = tissues _
            .Select(Function(i) i.label.Replicate(n:=i.nsize)) _
            .IteratesALL _
            .ToArray
        Dim colors As String() = tissues _
            .Select(Function(i) i.color.ToHtmlColor.Replicate(n:=i.nsize)) _
            .IteratesALL _
            .ToArray
        Dim x As New List(Of Integer)
        Dim y As New List(Of Integer)

        For Each region In tissues
            For Each p As Point In region.points
                x.Add(p.X)
                y.Add(p.Y)
            Next
        Next

        Return New dataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"label", labels},
                {"color", colors},
                {"x", x.ToArray},
                {"y", y.ToArray}
            }
        }
    End Function

    Private Function createUMAPTable(umap As UMAPPoint(), args As list, env As Environment) As dataframe
        Dim px As Integer() = umap.Select(Function(i) i.Pixel.X).ToArray
        Dim py As Integer() = umap.Select(Function(i) i.Pixel.Y).ToArray
        Dim x As Double() = umap.Select(Function(i) i.x).ToArray
        Dim y As Double() = umap.Select(Function(i) i.y).ToArray
        Dim z As Double() = umap.Select(Function(i) i.z).ToArray
        Dim cluster As Integer() = umap _
            .Select(Function(i) Integer.Parse(i.class)) _
            .ToArray

        Return New dataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"px", px},
                {"py", py},
                {"x", x},
                {"y", y},
                {"z", z},
                {"cluster", cluster}
            },
            .rownames = px _
                .Select(Function(xi, i) $"{xi},{py(i)}") _
                .ToArray
        }
    End Function

    Public Function PlotTissueMap(tissue As TissueRegion(), args As list, env As Environment) As Object
        If args.CheckGraphicsDeviceExists Then
            ' draw on current graphics context
            Dim dev As graphicsDevice = RgraphicsDev.GetCurrentDevice
            ' config of the drawing layout
            Dim padding As Padding = InteropArgumentHelper.getPadding(dev.getArgumentValue("layout", args))
            Dim canvas As New GraphicsRegion(dev.g.Size, padding)

            Return dev.g.PlotTissueMap(canvas, tissue, args, env)
        Else
            Dim size As String = InteropArgumentHelper.getSize(args.getByName("size"), env)
            Dim padding As String = InteropArgumentHelper.getPadding(args.getByName("layout"))
            Dim bg As String = RColorPalette.getColor(args.getByName("bg"), "white")

            Return g.GraphicsPlots(
                size.SizeParser,
                padding,
                bg,
                Sub(ByRef g, canvas)
                    Call g.PlotTissueMap(canvas, tissue, args, env)
                End Sub, driver:=env.getDriver)
        End If
    End Function

    <Extension>
    Public Function PlotTissueMap(g As IGraphics, canvas As GraphicsRegion, tissue As TissueRegion(), args As list, env As Environment) As Object
        Dim is_missing_sample As Boolean = args.getValue({"missing"}, env, [default]:=False)
        Dim sample As String = args.getValue({"sample"}, env, [default]:="")
        Dim knn As Boolean = args.getValue({"knn"}, env, [default]:=False)

        If is_missing_sample AndAlso Not sample.StringEmpty Then
            tissue = tissue _
                .Select(Function(t)
                            Dim i = t.tags _
                                .Select(Function(si) si = sample) _
                                .SeqIterator _
                                .Where(Function(ti) ti.value) _
                                .ToArray

                            Return New TissueRegion With {
                                .color = Color.LightGray,
                                .label = sample,
                                .tags = i.Select(Function(a) t.tags(a.i)).ToArray,
                                .points = i.Select(Function(a) t.points(a.i)).ToArray
                            }
                        End Function) _
                .Where(Function(t) t.nsize > 0) _
                .ToArray
        End If

        Dim x = tissue.Select(Function(t) t.points.Select(Function(a) CDbl(a.X))).IteratesALL.Range
        Dim y = tissue.Select(Function(t) t.points.Select(Function(a) CDbl(a.Y))).IteratesALL.Range
        Dim rect = canvas.PlotRegion
        Dim lx = d3js.scale.linear.domain(range:=x).range(integers:={rect.Left, rect.Right})
        Dim ly = d3js.scale.linear.domain(range:=y).range(integers:={rect.Top, rect.Height})
        Dim scale_x As Double = std.Abs(lx(2) - lx(1))
        Dim scale_y As Double = std.Abs(ly(2) - ly(1))
        Dim dotSize As New SizeF(scale_x, scale_y)
        Dim dot As RectangleF
        Dim scaler As New DataScaler() With {.X = lx, .Y = ly, .region = rect}
        Dim fillColor As SolidBrush
        Dim dims As Size = InteropArgumentHelper.getSize(args.getByName("dims"), env, [default]:=Nothing).SizeParser
        Dim interplate As PixelData()

        If dims.IsEmpty Then
            Return Internal.debug.stop("missng of the ms-imaging dimension size value!", env)
        End If

        For Each region As TissueRegion In tissue.OrderBy(Function(r) If(r.label = missing, 0, 1))
            fillColor = New SolidBrush(region.color)
            interplate = region.points _
                .Select(Function(xy) New PixelData(xy) With {.intensity = 1}) _
                .ToArray

            If knn Then
                interplate = interplate.KnnFill(dims, 4, 4)
            End If

            For Each p As PixelData In interplate
                dot = New RectangleF(scaler.Translate(p.x, p.y), dotSize)
                g.FillRectangle(fillColor, dot)
            Next
        Next

        Return Nothing
    End Function

    Const missing As String = NameOf(missing)

    ''' <summary>
    ''' Add tissue region label to the pixels of the ms-imaging data
    ''' </summary>
    ''' <param name="MSI">
    ''' the ms-imaging data to tag the region label, value type of this parameter
    ''' could be a set of point data or a ms-imaging data drawer wrapper
    ''' </param>
    ''' <param name="tissues"></param>
    ''' <param name="trim_suffix"></param>
    ''' <returns></returns>
    <ExportAPI("tag_samples")>
    Public Function tag_samples(<RRawVectorArgument>
                                MSI As Object,
                                tissues As TissueRegion(),
                                Optional trim_suffix As Boolean = False,
                                Optional env As Environment = Nothing) As Object

        If MSI Is Nothing Then
            Call env.AddMessage("the required spatial spot data is nothing!")
            Return Nothing
        End If
        If TypeOf MSI Is Drawer Then
            Return TagSampleLabels(MSI, tissues, trim_suffix)
        Else
            Return GetPointLabels(tissues, MSI, trim_suffix, env)
        End If
    End Function

    Private Function TagSampleLabels(MSI As Drawer, tissues As TissueRegion(), trim_suffix As Boolean) As Object
        Dim reader As PixelReader = MSI.pixelReader

        For Each tissue As TissueRegion In tissues
            Dim tags As New List(Of String)
            Dim pixel As PixelScan

            For Each p As Point In tissue.points
                pixel = reader.GetPixel(p.X, p.Y)

                If Not pixel Is Nothing Then
                    Call tags.Add(If(
                        trim_suffix,
                        pixel.sampleTag.BaseName,
                        pixel.sampleTag
                    ))
                Else
                    tags.Add(missing)
                End If
            Next

            tissue.tags = tags.ToArray
        Next

        Return tissues
    End Function

    Private Function GetPointLabels(tissues As TissueRegion(), pixels As Object, trim_suffix As Boolean, env As Environment) As Object
        Dim spatialLabels = tissues _
            .ExtractSpatialSpots _
            .DoCall(Function(ls)
                        Return Grid(Of PhenographSpot).Create(ls)
                    End Function)

        If TypeOf pixels Is dataframe Then
            Return FillLabels(df:=pixels, spatialLabels, trim_suffix)
        ElseIf TypeOf pixels Is list Then
            Dim list As list = pixels

            If list.hasName("x") AndAlso list.hasName("y") Then
                Dim x As Integer() = CLRVector.asInteger(list.getByName("x"))
                Dim y As Integer() = CLRVector.asInteger(list.getByName("y"))
                Dim df As New dataframe With {
                    .columns = New Dictionary(Of String, Array) From {
                        {"x", x},
                        {"y", y}
                    },
                    .rownames = Nothing
                }

                Return FillLabels(df, spatialLabels, trim_suffix)
            End If
        End If

        Return Message.InCompatibleType(GetType(dataframe), pixels.GetType, env)
    End Function

    Private Function FillLabels(df As dataframe, spatialLabels As Grid(Of PhenographSpot), trim_suffix As Boolean) As Object
        Dim x As Integer() = CLRVector.asInteger(df!x)
        Dim y As Integer() = CLRVector.asInteger(df!y)
        Dim hit As Boolean = False
        Dim label As String() = New String(x.Length - 1) {}
        Dim color As String() = New String(x.Length - 1) {}
        Dim sample As String() = New String(x.Length - 1) {}

        For i As Integer = 0 To x.Length - 1
            Dim q = spatialLabels.GetData(x(i), y(i), hit)

            If hit Then
                label(i) = q.phenograph_cluster
                color(i) = q.color
                sample(i) = If(
                    trim_suffix,
                    q.sample_tag.BaseName,
                    q.sample_tag
                )
            Else
                label(i) = "NA"
                color(i) = "gray"
                sample(i) = "NA"
            End If
        Next

        Call df.add("class", label)
        Call df.add("color", label)
        Call df.add("sample", sample)

        Return df
    End Function

    ''' <summary>
    ''' extract the missing tissue pixels based on the ion layer data
    ''' </summary>
    ''' <param name="layer"></param>
    ''' <param name="tissues"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' this function used for generates the tissue map segment plot data for some special charts
    ''' </remarks>
    <ExportAPI("intersect_layer")>
    <RApiReturn(GetType(TissueRegion))>
    Public Function intersect(layer As SingleIonLayer, tissues As TissueRegion(), Optional trim_suffix As Boolean = False) As Object
        Dim missing As New List(Of Point)
        Dim intersectList As New List(Of TissueRegion)
        Dim MSI = Grid(Of PixelData).Create(layer.MSILayer)
        Dim multiple_samples As Boolean = layer.hasMultipleSamples
        Dim sample_tag As String = layer.sampleTags.FirstOrDefault

        If trim_suffix Then
            sample_tag = sample_tag.BaseName
        End If

        For Each region As TissueRegion In tissues
            Dim region_filter As New List(Of Point)

            If region.tags.IsNullOrEmpty OrElse multiple_samples Then
                For Each point As Point In region.points
                    Dim hit As Boolean = False

                    Call MSI.GetData(point.X, point.Y, hit)

                    If Not hit Then
                        Call missing.Add(point)
                    Else
                        Call region_filter.Add(point)
                    End If
                Next
            Else
                For i As Integer = 0 To region.points.Length - 1
                    Dim point As Point = region.points(i)
                    Dim tag As String = region.tags(i)
                    Dim hit As Boolean = False

                    If tag <> sample_tag Then
                        Continue For
                    End If

                    Call MSI.GetData(point.X, point.Y, hit)

                    If Not hit Then
                        Call missing.Add(point)
                    Else
                        Call region_filter.Add(point)
                    End If
                Next
            End If

            Call intersectList.Add(New TissueRegion With {
                .color = region.color,
                .label = region.label,
                .points = region_filter.ToArray
            })
        Next

        Return intersectList _
            .JoinIterates({New TissueRegion With {
                .color = Color.LightGray,
                .label = TissueMorphology.missing,
                .points = missing.ToArray
            }}) _
            .Where(Function(r) r.nsize > 0) _
            .ToArray
    End Function

    ''' <summary>
    ''' create a collection of the umap sample data
    ''' </summary>
    ''' <param name="points">the spatial points</param>
    ''' <param name="x">umap dimension x</param>
    ''' <param name="y">umap dimension y</param>
    ''' <param name="z">umap dimension z</param>
    ''' <param name="cluster">the cluster id</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("UMAPsample")>
    <RApiReturn(GetType(UMAPPoint))>
    Public Function createUMAPsample(<RRawVectorArgument>
                                     points As Object,
                                     x As Double(),
                                     y As Double(),
                                     z As Double(),
                                     cluster As String(),
                                     Optional is_singlecells As Boolean = False,
                                     Optional env As Environment = Nothing) As Object

        Dim pixels As String() = CLRVector.asCharacter(points)
        Dim umap As UMAPPoint() = pixels _
            .Select(Function(pi, i)
                        Dim sample As UMAPPoint

                        If is_singlecells Then
                            sample = New UMAPPoint With {
                                .[class] = cluster(i),
                                .label = pi,
                                .x = x(i),
                                .y = y(i),
                                .z = z(i)
                            }
                        Else
                            Dim xy As Integer() = pi.Split(","c) _
                                .Select(AddressOf Integer.Parse) _
                                .ToArray

                            sample = New UMAPPoint With {
                                .[class] = cluster(i),
                                .Pixel = New Point(xy(0), xy(1)),
                                .label = pi,
                                .x = x(i),
                                .y = y(i),
                                .z = z(i)
                            }
                        End If

                        Return sample
                    End Function) _
            .ToArray

        Return umap
    End Function

    ''' <summary>
    ''' create a collection of the tissue region dataset
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <param name="labels"></param>
    ''' <param name="colorSet">
    ''' the color set schema name or a list of color data 
    ''' which can be mapping to the given <paramref name="labels"/> 
    ''' list.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("TissueData")>
    <RApiReturn(GetType(TissueRegion))>
    Public Function createTissueData(x As Integer(),
                                     y As Integer(),
                                     labels As String(),
                                     Optional colorSet As Object = "Paper",
                                     Optional env As Environment = Nothing) As Object

        Dim labelClass As String() = labels.Distinct.ToArray
        Dim colors As New Dictionary(Of String, Color)
        Dim regions As New Dictionary(Of String, List(Of Point))

        If TypeOf colorSet Is list Then
            Dim list As list = DirectCast(colorSet, list)

            For Each name As String In list.getNames
                Call colors.Add(name, RColorPalette.GetRawColor(list.getByName(name)))
            Next
        Else
            Dim colorList = Designer.GetColors(colorSet, labelClass.Length)
            Dim i As i32 = Scan0

            For Each label As String In labelClass
                Call colors.Add(label, colorList(++i))
            Next
        End If

        For Each label As String In labelClass
            Call regions.Add(label, New List(Of Point))
        Next

        For i As Integer = 0 To labels.Length - 1
            Call regions(labels(i)).Add(New Point(x(i), y(i)))
        Next

        Return regions _
            .Select(Function(r, i)
                        Return New TissueRegion With {
                            .color = colors(r.Key),
                            .label = r.Key,
                            .points = r.Value.ToArray
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' export the tissue data as cdf file
    ''' </summary>
    ''' <param name="tissueMorphology"></param>
    ''' <param name="file"></param>
    ''' <param name="umap"></param>
    ''' <param name="dimension">The dimension size of the ms-imaging slide sample data</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("writeCDF")>
    <RApiReturn(TypeCodes.boolean)>
    Public Function createCDF(tissueMorphology As TissueRegion(),
                              file As Object,
                              Optional umap As UMAPPoint() = Nothing,
                              <RRawVectorArgument>
                              Optional dimension As Object = Nothing,
                              Optional env As Environment = Nothing) As Object

        Dim saveBuf = SMRUCC.Rsharp.GetFileStream(file, IO.FileAccess.Write, env)
        Dim dimSize = InteropArgumentHelper _
            .getSize(dimension, env, "0,0") _
            .SizeParser

        If saveBuf Like GetType(Message) Then
            Return saveBuf.TryCast(Of Message)
        End If

        Using buffer As Stream = saveBuf.TryCast(Of Stream)
            Return tissueMorphology.WriteCDF(
                file:=buffer,
                umap:=umap,
                dimension:=dimSize
            )
        End Using
    End Function

    ''' <summary>
    ''' load tissue region polygon data
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="id">
    ''' the region id, which could be used for load specific 
    ''' region polygon data. default nothing means load all
    ''' tissue region polygon data
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' a collection of tissue polygon region objects.
    ''' </returns>
    <ExportAPI("loadTissue")>
    <RApiReturn(GetType(TissueRegion))>
    Public Function loadTissue(<RRawVectorArgument>
                               file As Object,
                               Optional id As String = "*",
                               Optional env As Environment = Nothing) As Object

        Dim readBuf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If readBuf Like GetType(Message) Then
            Return readBuf.TryCast(Of Message)
        End If

        Using buffer As Stream = readBuf.TryCast(Of Stream)
            If id.StringEmpty OrElse id = "*" Then
                Return buffer _
                    .ReadTissueMorphology _
                    .ToArray
            Else
                Return buffer _
                    .ReadTissueMorphology _
                    .Where(Function(r) r.label = id) _
                    .ToArray
            End If
        End Using
    End Function

    ''' <summary>
    ''' load UMAP data
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("loadUMAP")>
    <RApiReturn(GetType(UMAPPoint))>
    Public Function loadUMAP(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim readBuf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If readBuf Like GetType(Message) Then
            Return readBuf.TryCast(Of Message)
        End If

        Using buffer As Stream = readBuf.TryCast(Of Stream)
            Return buffer.ReadUMAP
        End Using
    End Function

    ''' <summary>
    ''' read spatial mapping data of STdata mapping to SMdata
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the spatial mapping xml dataset file 
    ''' </param>
    ''' <param name="remove_suffix">
    ''' removes of the numeric suffix of the STdata barcode?
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("read.spatialMapping")>
    <RApiReturn(GetType(SpatialMapping))>
    Public Function loadSpatialMapping(file As String, Optional remove_suffix As Boolean = False, Optional env As Environment = Nothing) As Object
        Dim mapping = file.LoadXml(Of SpatialMapping)(throwEx:=False)

        If mapping Is Nothing Then
            Return Internal.debug.stop({
                $"the required spatial mapping data which is loaded from the file location ({file}) is nothing, this could be some reasons:",
                $"file is exists on location: {file}",
                $"or invalid xml file format"
            }, env)
        ElseIf remove_suffix Then
            mapping = New SpatialMapping With {
                .label = mapping.label,
                .transform = mapping.transform,
                .spots = mapping.spots _
                    .Select(Function(f)
                                Return New SpotMap With {
                                    .barcode = f.barcode.StringReplace("[-]\d+", ""),
                                    .flag = f.flag,
                                    .physicalXY = f.physicalXY,
                                    .SMX = f.SMX,
                                    .SMY = f.SMY,
                                    .spotXY = f.spotXY,
                                    .STX = f.STX,
                                    .STY = f.STY
                                }
                            End Function) _
                    .ToArray
            }
        End If

        Return mapping
    End Function

    ''' <summary>
    ''' Split the spatial mapping by tissue label data
    ''' </summary>
    ''' <param name="mapping"></param>
    ''' <returns>A tuple list of the <see cref="SpatialMapping"/> object</returns>
    <ExportAPI("splitMapping")>
    <RApiReturn(GetType(list))>
    Public Function SplitMapping(mapping As SpatialMapping) As Object
        Dim list As New Dictionary(Of String, Object)
        Dim groups = mapping.spots _
            .GroupBy(Function(r) Strings.Trim(r.TissueMorphology)) _
            .ToArray

        For Each group In groups
            list(group.Key) = New SpatialMapping With {
                .color = mapping.color,
                .label = group.Key,
                .transform = mapping.transform,
                .spots = group.ToArray
            }
        Next

        Return New list With {
            .slots = list
        }
    End Function

    ''' <summary>
    ''' create a spatial grid for the spatial spot data
    ''' </summary>
    ''' <param name="mapping"></param>
    ''' <param name="gridSize"></param>
    ''' <param name="label">
    ''' the parameter value will overrides the internal
    ''' label of the mapping if this parameter string 
    ''' value is not an empty string.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("gridding")>
    <RApiReturn(GetType(SpotMap))>
    Public Function gridding(mapping As SpatialMapping,
                             Optional gridSize As Integer = 6,
                             Optional label As String = Nothing) As Object

        Dim spotGrid As Grid(Of SpotMap) = Grid(Of SpotMap).Create(mapping.spots)
        Dim blocks = spotGrid.WindowSize(gridSize, gridSize).Gridding.ToArray
        Dim grids As New list With {.slots = New Dictionary(Of String, Object)}
        Dim tag As String = mapping.label

        If Not label.StringEmpty Then
            tag = label
        End If
        If tag.StringEmpty Then
            tag = label
        End If
        If tag.StringEmpty Then
            tag = "block"
        End If

        For i As Integer = 0 To blocks.Length - 1
            If blocks(i).Length > 0 Then
                Call grids.add($"{tag}_{i + 1}", blocks(i))
            End If
        Next

        Return grids
    End Function

End Module
