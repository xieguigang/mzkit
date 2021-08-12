#Region "Microsoft.VisualBasic::e0b3b7b168c738f8d6fecc9b5a1e4765, src\visualize\MsImaging\Drawer.vb"

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

' Class Drawer
' 
'     Properties: dimension, pixelReader
' 
'     Constructor: (+3 Overloads) Sub New
' 
'     Function: ChannelCompositions, (+2 Overloads) DrawLayer, GetPixelChannelReader, GetPixelsMatrix, LoadPixels
'               ReadXY, RenderPixels, ScaleLayer, ScalePixels, ShowSummaryRendering
' 
'     Sub: (+2 Overloads) Dispose
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.Runtime

''' <summary>
''' MS-imaging render canvas
''' </summary>
Public Class Drawer : Implements IDisposable

    Dim disposedValue As Boolean

    Public ReadOnly Property pixelReader As PixelReader

    ''' <summary>
    ''' the size of the scan in [width,height]
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property dimension As Size
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return pixelReader.dimension
        End Get
    End Property

    Sub New(file As String, Optional memoryCache As Boolean = False)
        If file.ExtensionSuffix("imzML") Then
            pixelReader = New ReadIbd(imzML:=file, memoryCache:=memoryCache)
        ElseIf file.ExtensionSuffix("mzpack") Then
            pixelReader = New ReadRawPack(mzpack:=file)
        Else
            Throw New InvalidProgramException($"unsupported file type: {file.FileName}")
        End If
    End Sub

    Sub New(mzpack As mzPack)
        pixelReader = New ReadRawPack(mzpack)
    End Sub

    Sub New(reader As PixelReader)
        pixelReader = reader
    End Sub

    Public Function ReadXY(x As Integer, y As Integer) As IEnumerable(Of ms2)
        Dim pixel As PixelScan = pixelReader.GetPixel(x, y)

        If pixel Is Nothing Then
            Return {}
        Else
            Return pixel.GetMsPipe
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function LoadPixels(mz As Double(), tolerance As Tolerance,
                               Optional skipZero As Boolean = True,
                               Optional polygonFilter As Point() = Nothing) As IEnumerable(Of PixelData)

        Return pixelReader.LoadPixels(mz, tolerance, skipZero, polygonFilter)
    End Function

    Public Function ShowSummaryRendering(summary As IntensitySummary,
                                         Optional cutoff As DoubleRange = Nothing,
                                         Optional colorSet$ = "Jet",
                                         Optional pixelSize$ = "3,3",
                                         Optional logE As Boolean = True) As Bitmap

        Dim layer = pixelReader.GetSummary.GetLayer(summary).ToArray
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
            dimension:=dimension,
            dimSize:=pixelSize.SizeParser,
            colorSet:=colorSet,
            defaultFill:="black",
            cutoff:=cutoff,
            logE:=logE
        )
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="R"></param>
    ''' <param name="G"></param>
    ''' <param name="B"></param>
    ''' <param name="dimension"></param>
    ''' <param name="dimSize">
    ''' set this parameter to value nothing to returns
    ''' the raw image without any scale operation.
    ''' </param>
    ''' <param name="scale"></param>
    ''' <returns></returns>
    Public Shared Function ChannelCompositions(R As PixelData(), G As PixelData(), B As PixelData(),
                                               dimension As Size,
                                               Optional dimSize As Size = Nothing,
                                               Optional scale As InterpolationMode = InterpolationMode.Bilinear) As Bitmap

        Dim raw As New Bitmap(dimension.Width, dimension.Height, PixelFormat.Format32bppArgb)
        Dim Rchannel = GetPixelChannelReader(R)
        Dim Gchannel = GetPixelChannelReader(G)
        Dim Bchannel = GetPixelChannelReader(B)

        Using buffer As BitmapBuffer = BitmapBuffer.FromBitmap(raw, ImageLockMode.WriteOnly)
            For x As Integer = 1 To dimension.Width
                For y As Integer = 1 To dimension.Height
                    Dim bR As Byte = Rchannel(x, y)
                    Dim bG As Byte = Gchannel(x, y)
                    Dim bB As Byte = Bchannel(x, y)
                    Dim color As Color = Color.FromArgb(bR, bG, bB)

                    ' imzXML里面的坐标是从1开始的
                    ' 需要减一转换为.NET中从零开始的位置
                    Call buffer.SetPixel(x - 1, y - 1, color)
                Next
            Next
        End Using

        If dimSize.Width = 0 OrElse dimSize.Height = 0 Then
            Return raw
        Else
            Return ScaleLayer(raw, dimension, dimSize, scale)
        End If
    End Function

    Private Shared Function GetPixelChannelReader(channel As PixelData()) As Func(Of Integer, Integer, Byte)
        If channel.IsNullOrEmpty Then
            Return Function(x, y) CByte(0)
        End If

        Dim intensityRange As DoubleRange = channel.Select(Function(p) p.intensity).ToArray
        Dim byteRange As DoubleRange = {0, 255}
        Dim xy = channel _
            .GroupBy(Function(p) p.x) _
            .ToDictionary(Function(p) p.Key,
                          Function(x)
                              Return x _
                                  .GroupBy(Function(p) p.y) _
                                  .ToDictionary(Function(p) p.Key,
                                                Function(p)
                                                    Return p.Select(Function(pm) pm.intensity).Max
                                                End Function)
                          End Function)

        Return Function(x, y) As Byte
                   If Not xy.ContainsKey(x) Then
                       Return 0
                   End If

                   Dim ylist = xy.Item(x)

                   If Not ylist.ContainsKey(y) Then
                       Return 0
                   End If

                   Return CByte(intensityRange.ScaleMapping(ylist.Item(y), byteRange))
               End Function
    End Function

    Public Shared Function ScaleLayer(raw As Bitmap, dimension As Size, dimSize As Size, scale As InterpolationMode) As Bitmap
        Dim newWidth As Integer = dimension.Width * dimSize.Width
        Dim newHeight As Integer = dimension.Height * dimSize.Height

        Return ScaleLayer(raw, newWidth, newHeight, scale)
    End Function

    ''' <summary>
    ''' scale the layer size
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="newWidth"></param>
    ''' <param name="newHeight"></param>
    ''' <param name="scale"></param>
    ''' <returns></returns>
    Public Shared Function ScaleLayer(raw As Bitmap, newWidth As Integer, newHeight As Integer, scale As InterpolationMode) As Bitmap
        Dim newSize As New Rectangle(0, 0, newWidth, newHeight)
        Dim rawSize As New Rectangle(0, 0, raw.Width, raw.Height)

        If scale = InterpolationMode.Invalid Then
            scale = InterpolationMode.Default
        End If

        Using layer As Graphics2D = New Bitmap(newWidth, newHeight)
            layer.InterpolationMode = scale
            layer.SmoothingMode = SmoothingMode.HighQuality
            layer.DrawImage(raw, newSize, rawSize, GraphicsUnit.Pixel)

            Return layer.ImageResource
        End Using
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pixels"></param>
    ''' <param name="dimension">the scan size</param>
    ''' <param name="dimSize">pixel size</param>
    ''' <param name="colorSet"></param>
    ''' <param name="mapLevels"></param>
    ''' <param name="logE"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' <paramref name="dimSize"/> value set to nothing for returns the raw image
    ''' </remarks>
    Public Shared Function RenderPixels(pixels As PixelData(), dimension As Size, dimSize As Size,
                                        Optional colorSet As String = "YlGnBu:c8",
                                        Optional mapLevels% = 25,
                                        Optional logE As Boolean = False,
                                        Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                        Optional defaultFill As String = "Transparent",
                                        Optional cutoff As DoubleRange = Nothing) As Bitmap
        Dim color As Color
        Dim colors As Color() = Designer.GetColors(colorSet, mapLevels)
        Dim index As Integer
        Dim level As Double
        Dim indexrange As DoubleRange = New Double() {0, colors.Length - 1}
        Dim levelRange As DoubleRange = New Double() {0, 1}
        Dim raw As New Bitmap(dimension.Width, dimension.Height, PixelFormat.Format32bppArgb)
        Dim defaultColor As Color = defaultFill.TranslateColor

        Using buffer As BitmapBuffer = BitmapBuffer.FromBitmap(raw, ImageLockMode.WriteOnly)
            For Each point As PixelData In PixelData.ScalePixels(pixels, cutoff, logE)
                level = point.level

                If level <= 0.0 Then
                    color = defaultColor
                Else
                    index = levelRange.ScaleMapping(level, indexrange)

                    If index < 0 Then
                        index = 0
                    End If

                    color = colors(index)
                End If

                ' imzXML里面的坐标是从1开始的
                ' 需要减一转换为.NET中从零开始的位置
                Call buffer.SetPixel(point.x - 1, point.y - 1, color)
            Next
        End Using

        If dimSize.Width = 0 OrElse dimSize.Height = 0 Then
            Return raw
        Else
            Return ScaleLayer(raw, dimension, dimSize, scale)
        End If
    End Function

    ''' <summary>
    ''' apply for metabolite rendering
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="pixelSize$"></param>
    ''' <param name="toleranceErr"></param>
    ''' <param name="colorSet"></param>
    ''' <param name="mapLevels%"></param>
    ''' <returns></returns>
    Public Function DrawLayer(mz As Double,
                              Optional pixelSize$ = "5,5",
                              Optional toleranceErr As String = "da:0.1",
                              Optional colorSet As String = "YlGnBu:c8",
                              Optional mapLevels% = 25,
                              Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                              Optional cutoff As DoubleRange = Nothing) As Bitmap

        Dim dimSize As Size = pixelSize.SizeParser
        Dim tolerance As Tolerance = Tolerance.ParseScript(toleranceErr)

        Call $"loading pixel datas [m/z={mz.ToString("F4")}] with tolerance {tolerance}...".__INFO_ECHO

        Dim pixels As PixelData() = pixelReader.LoadPixels({mz}, tolerance).ToArray

        Call $"rendering {pixels.Length} pixel blocks...".__INFO_ECHO

        Return RenderPixels(pixels, dimension, dimSize, colorSet, mapLevels, scale:=scale, cutoff:=cutoff)
    End Function

    Public Shared Function ScalePixels(rawPixels As PixelData(), tolerance As Tolerance) As PixelData()
        Dim pixels As New List(Of PixelData)

        For Each mzi In rawPixels.GroupBy(Function(x) x.mz, tolerance).ToArray
            rawPixels = PixelData.ScalePixels(mzi.ToArray)
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
                                                Order By pt.level
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
    ''' <param name="pixelSize$"></param>
    ''' <param name="toleranceErr"></param>
    ''' <param name="colorSet"></param>
    ''' <param name="mapLevels%"></param>
    ''' <returns></returns>
    Public Function DrawLayer(mz As Double(),
                              Optional pixelSize$ = "5,5",
                              Optional toleranceErr As String = "da:0.1",
                              Optional colorSet As String = "YlGnBu:c8",
                              Optional mapLevels% = 25,
                              Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                              Optional cutoff As DoubleRange = Nothing) As Bitmap

        Dim dimSize As Size = pixelSize.SizeParser
        Dim rawPixels As PixelData()
        Dim tolerance As Tolerance = Tolerance.ParseScript(toleranceErr)

        Call $"loading pixel datas [m/z={mz.Select(Function(mzi) mzi.ToString("F4")).JoinBy(", ")}] with tolerance {tolerance}...".__INFO_ECHO

        rawPixels = pixelReader.LoadPixels(mz, tolerance).ToArray
        rawPixels = ScalePixels(rawPixels, tolerance)

        Call $"building pixel matrix from {rawPixels.Count} raw pixels...".__INFO_ECHO

        Dim matrix As PixelData() = GetPixelsMatrix(rawPixels)

        Call $"rendering {matrix.Length} pixel blocks...".__INFO_ECHO

        Return RenderPixels(matrix, dimension, dimSize, colorSet, mapLevels, scale:=scale, cutoff:=cutoff)
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
