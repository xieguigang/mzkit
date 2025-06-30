#Region "Microsoft.VisualBasic::1e5ec6be0e8d4b68c8e26bf9a6b5f414, visualize\MsImaging\Drawer.vb"

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

    '   Total Lines: 362
    '    Code Lines: 215 (59.39%)
    ' Comment Lines: 98 (27.07%)
    '    - Xml Docs: 86.73%
    ' 
    '   Blank Lines: 49 (13.54%)
    '     File Size: 14.81 KB


    ' Class Drawer
    ' 
    '     Properties: dimension, pixelReader
    ' 
    '     Constructor: (+4 Overloads) Sub New
    ' 
    '     Function: (+2 Overloads) DrawLayer, GetPixelsMatrix, (+2 Overloads) LoadPixels, ReadXY, RenderSummaryLayer
    '               (+2 Overloads) ScaleLayer, ScalePixels, ShowSummaryRendering
    ' 
    '     Sub: (+2 Overloads) Dispose
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports HeatMapParameters = Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap.HeatMapParameters

#If NET48 Then
Imports Microsoft.VisualBasic.Drawing
Imports Microsoft.VisualBasic.Drawing.Imaging.BitmapImage

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
''' MS-imaging render canvas
''' </summary>
Public Class Drawer : Implements IDisposable

    Dim disposedValue As Boolean

    ''' <summary>
    ''' adapter to the ms-imaging rawdata file
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property pixelReader As PixelReader

    ''' <summary>
    ''' the size of the scan in [width,height]
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' get <see cref="pixelReader.dimension"/>
    ''' </remarks>
    Public ReadOnly Property dimension As Size
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return pixelReader.dimension
        End Get
    End Property

    ''' <summary>
    ''' construct a data render from a local spatial rawdata file.
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="memoryCache"></param>
    ''' <param name="verbose"></param>
    Sub New(file As String, Optional memoryCache As Boolean = False, Optional verbose As Boolean = True)
        If file.ExtensionSuffix("imzML") Then
            pixelReader = New ReadIbd(imzML:=file, memoryCache:=memoryCache)
        ElseIf file.ExtensionSuffix("mzpack") Then
            ' pixelReader = New ReadRawPack(mzpack:=file, verbose:=verbose)
            Throw New NotSupportedException($"Create ms-imaging rendering canvas from read mzPack rawdata file is not supported.")
        Else
            Throw New InvalidProgramException($"unsupported file type: {file.FileName}")
        End If
    End Sub

    ''' <summary>
    ''' Create render from an in-memory dataset
    ''' </summary>
    ''' <param name="mzpack">
    ''' An abstract interface of the in-memory mzPack data object
    ''' </param>
    ''' <param name="verbose"></param>
    Sub New(mzpack As IMZPack, Optional verbose As Boolean = True, Optional indexMemory As Boolean = False)
        If indexMemory Then
            pixelReader = New MemoryIndexReader(mzpack, verbose)
        Else
            pixelReader = New ReadRawPack(mzpack, verbose)
        End If
    End Sub

    ''' <summary>
    ''' Create render from an in-memory dataset
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="verbose"></param>
    Sub New(matrix As mzPackPixel(), Optional verbose As Boolean = True)
        pixelReader = New ReadRawPack(matrix, verbose)
    End Sub

    Sub New(reader As PixelReader)
        pixelReader = reader
    End Sub

    ''' <summary>
    ''' read spectrum data via a given x,y spatial spot
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <returns>
    ''' A spatial data collection, empty if the given spot is not existed.
    ''' </returns>
    Public Function ReadXY(x As Integer, y As Integer) As IEnumerable(Of ms2)
        Dim pixel As PixelScan = pixelReader.GetPixel(x, y)

        If pixel Is Nothing Then
            Return {}
        Else
            Return pixel.GetMsPipe
        End If
    End Function

    ''' <summary>
    ''' Populate all pixels from the MS-Imaging raw data pack
    ''' </summary>
    ''' <returns></returns>
    Public Function LoadPixels() As IEnumerable(Of PixelScan)
        Return pixelReader.AllPixels
    End Function

    ''' <summary>
    ''' This function will populate all of the pixel data which has
    ''' m/z tagged and the mass error is matched with the given
    ''' <paramref name="tolerance"/> value.
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="skipZero"></param>
    ''' <param name="polygonFilter">
    ''' 默认的空值参数表示不做区域筛选
    ''' </param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function LoadPixels(mz As Double(), tolerance As Tolerance,
                               Optional skipZero As Boolean = True,
                               Optional polygonFilter As Point() = Nothing) As IEnumerable(Of PixelData)

        Return pixelReader.LoadPixels(mz, tolerance, skipZero, polygonFilter)
    End Function

    Public Function ShowSummaryRendering(summary As IntensitySummary,
                                         Optional colorSet$ = "Jet",
                                         Optional logE As Boolean = True,
                                         Optional pixelDrawer As Boolean = True,
                                         Optional driver As Drivers = Drivers.Default) As GraphicsData

        Dim layer As PixelScanIntensity() = pixelReader.GetSummary.GetLayer(summary).ToArray
        Dim render As GraphicsData = RenderSummaryLayer(layer, dimension, colorSet, logE, pixelDrawer, driver:=driver)

        Return render
    End Function

    Public Shared Function RenderSummaryLayer(layer As PixelScanIntensity(), dimension As Size,
                                              Optional colorSet$ = "Jet",
                                              Optional pixelDrawer As Boolean = True,
                                              Optional mapLevels As Integer = 25,
                                              Optional background As String = "black",
                                              Optional driver As Drivers = Drivers.Default) As GraphicsData

        Dim pixels As PixelData() = layer _
            .Select(Function(p)
                        Return New PixelData With {
                            .intensity = p.totalIon,
                            .x = p.x,
                            .y = p.y
                        }
                    End Function) _
            .ToArray
        Dim engine As Renderer = If(
            pixelDrawer,
            New PixelRender(heatmapRender:=False),
            New RectangleRender(driver, heatmapRender:=False)
        )
        Dim heatmap As New HeatMapParameters(colorSet, mapLevels, background)

        Return engine.RenderPixels(pixels:=pixels, dimension:=dimension, heatmap:=heatmap)
    End Function

    Public Shared Function ScaleLayer(raw As Bitmap, dimension As Size, dimSize As Size) As Bitmap
        Dim newWidth As Integer = dimension.Width * dimSize.Width
        Dim newHeight As Integer = dimension.Height * dimSize.Height

        Return ScaleLayer(raw, newWidth, newHeight)
    End Function

    ''' <summary>
    ''' scale the layer size
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="newWidth"></param>
    ''' <param name="newHeight"></param>
    ''' <returns></returns>
    Public Shared Function ScaleLayer(raw As Bitmap, newWidth As Integer, newHeight As Integer) As Bitmap
#If NET48 Then
        Return raw.Resize(newWidth, onlyResizeIfWider:=True)
#Else
        Return raw.Resize(newWidth, newHeight)
#End If
    End Function

    ''' <summary>
    ''' apply for metabolite rendering
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="toleranceErr"></param>
    ''' <param name="colorSet"></param>
    ''' <param name="mapLevels%"></param>
    ''' <returns></returns>
    Public Function DrawLayer(mz As Double,
                              Optional toleranceErr As String = "da:0.1",
                              Optional colorSet As String = "YlGnBu:c8",
                              Optional mapLevels% = 25,
                              Optional filter As RasterPipeline = Nothing,
                              Optional background As String = NameOf(Color.Transparent),
                              Optional driver As Drivers = Drivers.Default) As GraphicsData

        Dim tolerance As Tolerance = Tolerance.ParseScript(toleranceErr)

        Call $"loading pixel datas [m/z={mz.ToString("F4")}] with tolerance {tolerance}...".__INFO_ECHO

        Dim pixels As PixelData() = pixelReader.LoadPixels({mz}, tolerance).ToArray
        Dim engine As New RectangleRender(driver, heatmapRender:=False)
        Dim heatmap As New HeatMapParameters(colorSet, mapLevels, background)

        If Not filter Is Nothing Then
            pixels = filter.DoIntensityScale(pixels, dimSize:=dimension)
        End If

        Call $"rendering {pixels.Length} pixel blocks...".__INFO_ECHO

        Return engine.RenderPixels(
            pixels:=pixels,
            dimension:=dimension,
            heatmap:=heatmap
        )
    End Function

    ''' <summary>
    ''' normalized data from raw intensity to [0,1]
    ''' </summary>
    ''' <param name="rawPixels"></param>
    ''' <param name="tolerance"></param>
    ''' <returns></returns>
    Public Shared Function ScalePixels(rawPixels As PixelData(), tolerance As Tolerance) As PixelData()
        Dim pixels As New List(Of PixelData)

        For Each mzi In rawPixels.GroupBy(Function(x) x.mz, tolerance).ToArray
            rawPixels = PixelData.ScalePixels(mzi.ToArray).ToArray
            pixels.AddRange(rawPixels)
        Next

        Return pixels.ToArray
    End Function

    Public Shared Function GetPixelsMatrix(rawPixels As PixelData()) As PixelData()
        Return rawPixels _
            .GroupBy(Function(p) p.x) _
            .AsParallel _
            .Select(Function(x)
                        Return x _
                            .GroupBy(Function(p) p.y) _
                            .Select(Function(point)
                                        ' [x, y] point
                                        ' get the max level pixel
                                        Return (From pt As PixelData
                                                In point
                                                Order By pt.intensity
                                                Descending).First
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToArray
    End Function

    ''' <summary>
    ''' apply for pathway rendering 
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="toleranceErr"></param>
    ''' <param name="colorSet"></param>
    ''' <param name="mapLevels%"></param>
    ''' <returns></returns>
    Public Function DrawLayer(mz As Double(),
                              Optional toleranceErr As String = "da:0.1",
                              Optional colorSet As String = "YlGnBu:c8",
                              Optional mapLevels% = 25,
                              Optional filter As RasterPipeline = Nothing,
                              Optional background As String = NameOf(Color.Transparent),
                              Optional driver As Drivers = Drivers.Default) As GraphicsData

        Dim rawPixels As PixelData()
        Dim tolerance As Tolerance = Tolerance.ParseScript(toleranceErr)

        Call $"loading pixel datas [m/z={mz.Select(Function(mzi) mzi.ToString("F4")).JoinBy(", ")}] with tolerance {tolerance}...".__INFO_ECHO

        rawPixels = pixelReader.LoadPixels(mz, tolerance).ToArray

        If Not filter Is Nothing Then
            rawPixels = filter.DoIntensityScale(rawPixels, dimension)
        End If

        rawPixels = ScalePixels(rawPixels, tolerance)

        Call $"building pixel matrix from {rawPixels.Length} raw pixels...".__INFO_ECHO

        Dim matrix As PixelData() = GetPixelsMatrix(rawPixels)
        Dim engine As Renderer = New RectangleRender(driver, heatmapRender:=False)
        Dim heatmap As New HeatMapParameters(colorSet, mapLevels, background)

        Call $"rendering {matrix.Length} pixel blocks...".__INFO_ECHO

        Return engine.RenderPixels(
            pixels:=matrix,
            dimension:=dimension,
            heatmap:=heatmap
        )
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call pixelReader.Dispose()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
