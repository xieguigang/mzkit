#Region "Microsoft.VisualBasic::9f45b573fe832598780f5b325ee27edc, visualize\MsImaging\Blender\Renderer\RectangleRender.vb"

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

    '   Total Lines: 296
    '    Code Lines: 228 (77.03%)
    ' Comment Lines: 30 (10.14%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 38 (12.84%)
    '     File Size: 13.36 KB


    '     Class RectangleRender
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: (+2 Overloads) ChannelCompositions, (+2 Overloads) LayerOverlaps, (+2 Overloads) RenderPixels
    ' 
    '         Sub: (+2 Overloads) ChannelCompositions, FillLayerInternal, RenderPixels
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Html.CSS

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

Namespace Blender

    ''' <summary>
    ''' Do ms-imaging rendering via graphics draw rectangle method in heatmap rendering 
    ''' </summary>
    Public Class RectangleRender : Inherits Renderer

        ReadOnly driver As Drivers

        ''' <summary>
        ''' Construct a ms-imaging data render based on the graphics draw rectangle method
        ''' </summary>
        ''' <param name="driver"></param>
        ''' <param name="heatmapRender"></param>
        ''' <param name="overlaps"></param>
        ''' <remarks>
        ''' could be works for the gdi+ bitmap raster image/svg+pdf vector image file
        ''' </remarks>
        <DebuggerStepThrough>
        Public Sub New(driver As Drivers, heatmapRender As Boolean, Optional overlaps As Image = Nothing)
            MyBase.New(heatmapRender, overlaps)
            Me.driver = driver
        End Sub

        Public Overloads Sub ChannelCompositions(gr As IGraphics, region As GraphicsRegion,
                                                 R() As PixelData, G() As PixelData, B() As PixelData,
                                                 dimension As Size,
                                                 Optional background As String = "black")

            Dim defaultBackground As Color = background.TranslateColor
            Dim rgb As New RenderRGB(defaultBackground, heatmapMode) With {
                .Bchannel = GetPixelChannelReader(B),
                .Rchannel = GetPixelChannelReader(R),
                .Gchannel = GetPixelChannelReader(G),
                .dimension = dimension
            }

            Call rgb.Render(gr, region)
        End Sub

        Public Overloads Sub ChannelCompositions(g As IGraphics, region As GraphicsRegion,
                                                 C() As PixelData, M() As PixelData, Y() As PixelData, K() As PixelData,
                                                 dimension As Size,
                                                 Optional background As String = "black")

            Dim defaultBackground As Color = background.TranslateColor
            Dim cmyk As New RenderCMYK(defaultBackground, heatmapMode) With {
                .Cchannel = GetPixelChannelReader(C),
                .Mchannel = GetPixelChannelReader(M),
                .Ychannel = GetPixelChannelReader(Y),
                .Kchannel = GetPixelChannelReader(K),
                .dimension = dimension
            }

            Call cmyk.Render(g, region)
        End Sub

        Public Overrides Function ChannelCompositions(C() As PixelData, M() As PixelData, Y() As PixelData, K() As PixelData,
                                                      dimension As Size,
                                                      Optional background As String = "black") As GraphicsData

            Dim defaultBackground As Color = background.TranslateColor
            Dim Cchannel = GetPixelChannelReader(C)
            Dim Mchannel = GetPixelChannelReader(M)
            Dim Ychannel = GetPixelChannelReader(Y)
            Dim Kchannel = GetPixelChannelReader(K)
            Dim w As Integer = dimension.Width
            Dim h As Integer = dimension.Height
            Dim rgb As New RenderCMYK(defaultBackground, heatmapMode) With {
                .Cchannel = Cchannel,
                .dimension = dimension,
                .Mchannel = Mchannel,
                .Ychannel = Ychannel,
                .Kchannel = Kchannel
            }

            Return Drawing2D.g.GraphicsPlots(
                size:=New Size(w, h),
                padding:=New Padding,
                bg:=NameOf(Color.Transparent),
                driver:=driver,
                plotAPI:=AddressOf rgb.Render
            )
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="R"></param>
        ''' <param name="G"></param>
        ''' <param name="B"></param>
        ''' <param name="dimension">
        ''' the dimension size of the ms-imaging rawdata
        ''' </param>
        ''' <param name="background"></param>
        ''' <returns>
        ''' the size of the generated raster image is specificed by the <paramref name="dimension"/> parameter.
        ''' </returns>
        Public Overrides Function ChannelCompositions(R() As PixelData, G() As PixelData, B() As PixelData,
                                                      dimension As Size,
                                                      Optional background As String = "black") As GraphicsData

            Dim defaultBackground As Color = background.TranslateColor
            Dim Rchannel = GetPixelChannelReader(R)
            Dim Gchannel = GetPixelChannelReader(G)
            Dim Bchannel = GetPixelChannelReader(B)
            Dim w As Integer = dimension.Width
            Dim h As Integer = dimension.Height
            Dim rgb As New RenderRGB(defaultBackground, heatmapMode) With {
                .Bchannel = Bchannel,
                .dimension = dimension,
                .Gchannel = Gchannel,
                .Rchannel = Rchannel
            }

            Return Drawing2D.g.GraphicsPlots(
                size:=New Size(w, h),
                padding:=New Padding,
                bg:=NameOf(Color.Transparent),
                driver:=driver,
                plotAPI:=AddressOf rgb.Render
            )
        End Function

        Public Overloads Sub RenderPixels(g As IGraphics, offset As Point, pixels() As PixelData, colorSet() As SolidBrush)
            Dim heatmap As New HeatMapBrushes(colorSet, colorSet(0).Color.ToHtmlColor)
            Dim scale As ValueScaleColorProfile = heatmap.GetMapping(From p As PixelData In pixels Select p.intensity)

            Call FillLayerInternal(g, pixels, scale, offset)
        End Sub

        Public Overrides Function RenderPixels(pixels() As PixelData, dimension As Size, heatmap As HeatMapBrushes) As GraphicsData
            Dim defaultColor As Brush = heatmap.defaultFill.GetBrush
            Dim w = dimension.Width
            Dim h = dimension.Height
            Dim scale As ValueScaleColorProfile = heatmap.GetMapping(From p As PixelData In pixels Select p.intensity)

            If TypeOf defaultColor Is TextureBrush Then
                ' the background is a gdi image 
                ' so the default fill color should be transparent
                defaultColor = Brushes.Transparent
            End If

            Return g.GraphicsPlots(
                size:=New Size(w, h),
                padding:=New Padding,
                bg:=heatmap.defaultFill,
                driver:=driver,
                plotAPI:=Sub(ByRef g, region)
                             Call FillLayerInternal(g, pixels, scale, Nothing)
                         End Sub)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pixels"></param>
        ''' <param name="dimension">
        ''' the ms-imaging canvas size
        ''' </param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function RenderPixels(pixels() As PixelData, dimension As Size, heatmap As HeatMapParameters) As GraphicsData
            Return RenderPixels(pixels, dimension, heatmap.CreateBrushParameters)
        End Function

        Private Sub FillLayerInternal(g As IGraphics, pixels() As PixelData, scale As ValueScaleColorProfile, Offset As Point)
            Dim color As Brush
            Dim defaultColor As New SolidBrush(scale.DefaultColor)
            Dim dimSize As New SizeF(1, 1)

            If Not overlaps Is Nothing Then
                Call g.DrawImage(overlaps, New Rectangle(New Point, g.Size))
            End If
            ' skip rendering for empty data collection
            If pixels.IsNullOrEmpty Then
                Return
            End If

            Dim intensityRange As Double() = scale.ValueMinMax
            Dim is_transparentBg As Boolean = scale.DefaultColor.IsTransparent

            scale = scale.ReScaleToValueRange(0, 1)

            For Each point As PixelData In PixelData.ScalePixels(pixels, setRange:=intensityRange)
                Dim level As Double = point.level
                Dim pos As New PointF With {
                    .X = (point.x - 1) + Offset.X,
                    .Y = (point.y - 1) + Offset.Y
                }
                Dim rect As New RectangleF(pos, dimSize)
                Dim index As Integer

                color = scale.GetSolidColor(level, index)

                If level <= 0.0 OrElse index <= 0 Then
                    If is_transparentBg Then
                        ' 20250405 skip of current pixels rectangle drawing
                        Continue For
                    End If

                    color = defaultColor
                End If

                ' imzXML里面的坐标是从1开始的
                ' 需要减一转换为.NET中从零开始的位置
                Call g.FillRectangle(color, rect)
            Next
        End Sub

        Public Overrides Function LayerOverlaps(layers()() As PixelData, dimension As Size, colorSet As MzLayerColorSet,
                                                Optional defaultFill As String = "Transparent",
                                                Optional mapLevels As Integer = 25) As GraphicsData
            Dim i As i32 = Scan0
            Dim w = dimension.Width
            Dim h = dimension.Height

            Return g.GraphicsPlots(
                size:=New Size(w, h),
                padding:=New Padding,
                bg:=defaultFill,
                driver:=driver,
                plotAPI:=Sub(ByRef g, region)
                             For Each layer As PixelData() In layers
                                 Dim baseColor As Color = colorSet(++i)
                                 Dim colors As SolidBrush() = seq(50, 255, (255 - 30) / mapLevels) _
                                     .Select(Function(a) New SolidBrush(baseColor.Alpha(a))) _
                                     .ToArray
                                 Dim heatmap As New HeatMapBrushes(colors, defaultFill)
                                 Dim scale As ValueScaleColorProfile = heatmap.GetMapping(From p As PixelData In layer Select p.intensity)

                                 Call FillLayerInternal(g, layer, scale, Nothing)
                             Next
                         End Sub)
        End Function

        Public Overloads Function LayerOverlaps(pixels() As PixelData, dimension As Size, colorSet As MzLayerColorSet,
                                                Optional cut As DoubleRange = Nothing,
                                                Optional defaultFill As String = "Transparent",
                                                Optional mapLevels As Integer = 25) As GraphicsData

            Dim layers = colorSet.SelectGroup(pixels).ToArray
            Dim defaultColor As SolidBrush = defaultFill.GetBrush
            Dim w = dimension.Width
            Dim h = dimension.Height

            Return g.GraphicsPlots(
                size:=New Size(w, h),
                padding:=New Padding,
                bg:=defaultColor.Color.ToHtmlColor,
                driver:=driver,
                plotAPI:=Sub(ByRef g, region)
                             For Each layer As NamedCollection(Of PixelData) In layers
                                 Dim baseColor As Color = colorSet.FindColor(Val(layer.name))
                                 Dim colors As SolidBrush() = seq(0, 255, 255 / mapLevels) _
                                      .Select(Function(a)
                                                  Return New SolidBrush(baseColor.Alpha(a))
                                              End Function) _
                                      .ToArray
                                 Dim heatmap As New HeatMapBrushes(colors, defaultFill)
                                 Dim scale As ValueScaleColorProfile = heatmap.GetMapping(From p As PixelData In layer Select p.intensity)

                                 Call FillLayerInternal(g, layer.value, scale, Nothing)
                             Next
                         End Sub)
        End Function
    End Class
End Namespace
