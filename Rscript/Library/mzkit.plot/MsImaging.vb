#Region "Microsoft.VisualBasic::76248b05807c093d28c7062c0a8875d4, Rscript\Library\mzkit.plot\MsImaging.vb"

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
'     Function: FilterMz, flatten, GetMsMatrx, GetPixel, layer
'               LoadPixels, openIndexedCacheFile, renderRowScans, RGB, viewer
'               writeIndexCacheFile, WriteXICCache
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.IndexedCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
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

    Private Function plotMSI(ion As SingleIonLayer, args As list, env As Environment) As Object
        Dim theme As New Theme With {
            .padding = InteropArgumentHelper.getPadding(args!padding)
        }
        Dim scale As String = InteropArgumentHelper.getSize(args!scale, env, "8,8")
        Dim app As New MSIPlot(ion, scale.SizeParser, theme)
        Dim size As Size = app.MeasureSize

        Return app.Plot($"{size.Width},{size.Height}")
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
    ''' <param name="imzML">
    ''' *.imzML;*.mzPack
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("viewer")>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function viewer(imzML As String) As Drawer
        Return New Drawer(file:=imzML)
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
                        <RRawVectorArgument>
                        Optional pixelSize As Object = "5,5",
                        Optional tolerance As Object = "da:0.1",
                        Optional env As Environment = Nothing) As Object

        Dim errors As [Variant](Of Tolerance, Message) = Math.getTolerance(tolerance, env)
        Dim psize As Size = InteropArgumentHelper.getSize(pixelSize, env, "5,5").SizeParser

        If errors Like GetType(Message) Then
            Return errors.TryCast(Of Message)
        End If

        Dim pr As PixelData() = viewer.LoadPixels({r}, errors.TryCast(Of Tolerance)).ToArray
        Dim pg As PixelData() = viewer.LoadPixels({g}, errors.TryCast(Of Tolerance)).ToArray
        Dim pb As PixelData() = viewer.LoadPixels({b}, errors.TryCast(Of Tolerance)).ToArray

        Return Drawer.ChannelCompositions(pr, pg, pb, viewer.dimension, psize)
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
        Dim mzErr = Math.getTolerance(tolerance, env)

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        End If

        Dim pixels As PixelData() = viewer _
            .LoadPixels({mz}, mzErr.TryCast(Of Tolerance)) _
            .ToArray

        Return New SingleIonLayer With {
            .IonMz = mz,
            .DimensionSize = viewer.dimension,
            .MSILayer = pixels
        }
    End Function

    ''' <summary>
    ''' render a ms-imaging layer by a specific ``m/z`` scan.
    ''' </summary>
    ''' <param name="viewer"></param>
    ''' <param name="mz"></param>
    ''' <param name="threshold"></param>
    ''' <param name="pixelSize"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="color$"></param>
    ''' <param name="levels%"></param>
    ''' <returns></returns>
    <ExportAPI("layer")>
    <RApiReturn(GetType(Bitmap))>
    Public Function layer(viewer As Drawer, mz As Double(),
                          Optional threshold As Double = 0.05,
                          <RRawVectorArgument>
                          Optional pixelSize As Object = "5,5",
                          Optional tolerance As Object = "da:0.1",
                          Optional color$ = "YlGnBu:c8",
                          Optional levels% = 30,
                          Optional env As Environment = Nothing) As Object

        Dim errors As [Variant](Of Tolerance, Message) = Math.getTolerance(tolerance, env)

        If errors Like GetType(Message) Then
            Return errors.TryCast(Of Message)
        End If

        If mz.IsNullOrEmpty Then
            Return Nothing
        ElseIf mz.Length = 1 Then
            Return viewer.DrawLayer(
                mz:=mz(Scan0),
                threshold:=threshold,
                pixelSize:=InteropArgumentHelper.getSize(pixelSize, env, "5,5"),
                toleranceErr:=errors.TryCast(Of Tolerance).GetScript,
                colorSet:=color,
                mapLevels:=levels
            )
        Else
            Return viewer.DrawLayer(
                mz:=mz,
                threshold:=threshold,
                pixelSize:=InteropArgumentHelper.getSize(pixelSize, env, "5,5"),
                toleranceErr:=errors.TryCast(Of Tolerance).GetScript,
                colorSet:=color,
                mapLevels:=levels
            )
        End If
    End Function

    ''' <summary>
    ''' MS-imaging of the MSI summary data result.
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="intensity"></param>
    ''' <param name="colorSet$"></param>
    ''' <param name="defaultFill"></param>
    ''' <param name="pixelSize$"></param>
    ''' <param name="cutoff"></param>
    ''' <param name="logE"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("render")>
    Public Function renderRowScans(data As MSISummary, intensity As IntensitySummary,
                                   Optional colorSet$ = "Jet",
                                   Optional defaultFill As String = "Transparent",
                                   Optional pixelSize$ = "5,5",
                                   Optional cutoff As Double = 1,
                                   Optional logE As Boolean = True,
                                   Optional env As Environment = Nothing) As Object

        Dim layer = data.GetLayer(intensity).ToArray
        Dim pixels As PixelData() = layer _
            .Select(Function(p)
                        Return New PixelData With {
                            .intensity = p.totalIon,
                            .x = p.x,
                            .y = p.y
                        }
                    End Function) _
            .ToArray

        Return Drawer.RenderPixels(
            pixels:=pixels,
            dimension:=data.size,
            dimSize:=pixelSize.SizeParser,
            colorSet:=colorSet,
            logE:=logE,
            defaultFill:=defaultFill,
            cutoff:=cutoff
        )
    End Function

    ''' <summary>
    ''' flatten image layers
    ''' </summary>
    ''' <param name="layers">
    ''' layer bitmaps should be all in equal size
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("flatten")>
    Public Function flatten(layers As Bitmap(), Optional bg$ = "white") As Bitmap
        Using g As Graphics2D = New Bitmap(layers(Scan0).Width, layers(Scan0).Height)
            If Not bg.StringEmpty Then
                Call g.Clear(bg.GetBrush)
            End If

            ' 在这里是反向叠加图层的
            ' 向量中最开始的图层表示为最上层的图层，即最后进行绘制的图层
            For Each layer As Bitmap In layers.Reverse
                Call g.DrawImageUnscaled(layer, New Point)
            Next

            Return g.ImageResource
        End Using
    End Function
End Module
