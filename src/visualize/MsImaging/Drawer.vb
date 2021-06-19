#Region "Microsoft.VisualBasic::9ca112abf8edd8b301badaf61568927b, src\visualize\MsImaging\Drawer.vb"

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
'     Properties: dimension, ibd, UUID
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: (+2 Overloads) DrawLayer, GetPixelsMatrix, LoadMzArray, LoadPixels, RenderPixels
'               ScalePixels
' 
'     Sub: (+2 Overloads) Dispose
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
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

    Public ReadOnly Property dimension As Size
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return pixelReader.dimension
        End Get
    End Property

    Sub New(file As String)
        If file.ExtensionSuffix("imzML") Then
            pixelReader = New ReadIbd(imzML:=file)
        ElseIf file.ExtensionSuffix("mzpack") Then
            pixelReader = New ReadRawPack(mzpack:=file)
        Else
            Throw New InvalidProgramException($"unsupported file type: {file.FileName}")
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function LoadPixels(mz As Double(), tolerance As Tolerance, Optional skipZero As Boolean = True) As IEnumerable(Of PixelData)
        Return pixelReader.LoadPixels(mz, tolerance, skipZero)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pixels"></param>
    ''' <param name="dimension">the scan size</param>
    ''' <param name="dimSize">pixel size</param>
    ''' <param name="colorSet"></param>
    ''' <param name="mapLevels"></param>
    ''' <param name="threshold"></param>
    ''' <returns></returns>
    Public Shared Function RenderPixels(pixels As PixelData(), dimension As Size, dimSize As Size,
                                        Optional colorSet As String = "YlGnBu:c8",
                                        Optional mapLevels% = 25,
                                        Optional threshold As Double = 0.1) As Bitmap
        Dim color As SolidBrush
        Dim colors As SolidBrush() = Designer _
            .GetColors(colorSet, mapLevels) _
            .Select(Function(c) New SolidBrush(c)) _
            .ToArray
        Dim index As Integer
        Dim level As Double
        Dim rect As Rectangle
        Dim pos As Point
        Dim indexrange As DoubleRange = New Double() {0, colors.Length - 1}
        Dim levelRange As DoubleRange = New Double() {0, 1}

        Using layer As Graphics2D = New Bitmap(dimension.Width * dimSize.Width, dimension.Height * dimSize.Height)
            For Each point As PixelData In PixelData.ScalePixels(pixels)
                level = point.level

                If level < threshold Then
                    color = Brushes.Transparent
                Else
                    index = levelRange.ScaleMapping(level, indexrange)
                    color = colors(index)
                End If

                pos = New Point((point.x - 1) * dimSize.Width, (point.y - 1) * dimSize.Height)
                rect = New Rectangle(pos, dimSize)
                layer.FillRectangle(color, rect)
            Next

            Return layer.ImageResource
        End Using
    End Function

    ''' <summary>
    ''' apply for metabolite rendering
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="threshold"></param>
    ''' <param name="pixelSize$"></param>
    ''' <param name="toleranceErr"></param>
    ''' <param name="colorSet"></param>
    ''' <param name="mapLevels%"></param>
    ''' <returns></returns>
    Public Function DrawLayer(mz As Double,
                              Optional threshold As Double = 0.1,
                              Optional pixelSize$ = "5,5",
                              Optional toleranceErr As String = "da:0.1",
                              Optional colorSet As String = "YlGnBu:c8",
                              Optional mapLevels% = 25) As Bitmap

        Dim dimSize As Size = pixelSize.SizeParser
        Dim tolerance As Tolerance = Tolerance.ParseScript(toleranceErr)

        Call $"loading pixel datas [m/z={mz.ToString("F4")}] with tolerance {tolerance}...".__INFO_ECHO

        Dim pixels As PixelData() = pixelReader.LoadPixels({mz}, tolerance).ToArray

        Call $"rendering {pixels.Length} pixel blocks...".__INFO_ECHO

        Return RenderPixels(pixels, dimension, dimSize, colorSet, mapLevels, threshold)
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
                                        Return (From pt In point Order By pt.level Descending).First
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToArray
    End Function

    ''' <summary>
    ''' apply for pathway rendering 
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="threshold"></param>
    ''' <param name="pixelSize$"></param>
    ''' <param name="toleranceErr"></param>
    ''' <param name="colorSet"></param>
    ''' <param name="mapLevels%"></param>
    ''' <returns></returns>
    Public Function DrawLayer(mz As Double(),
                              Optional threshold As Double = 0.1,
                              Optional pixelSize$ = "5,5",
                              Optional toleranceErr As String = "da:0.1",
                              Optional colorSet As String = "YlGnBu:c8",
                              Optional mapLevels% = 25) As Bitmap

        Dim dimSize As Size = pixelSize.SizeParser
        Dim rawPixels As PixelData()
        Dim tolerance As Tolerance = Tolerance.ParseScript(toleranceErr)

        Call $"loading pixel datas [m/z={mz.Select(Function(mzi) mzi.ToString("F4")).JoinBy(", ")}] with tolerance {tolerance}...".__INFO_ECHO

        rawPixels = pixelReader.LoadPixels(mz, tolerance).ToArray
        rawPixels = ScalePixels(rawPixels, tolerance)

        Call $"building pixel matrix from {rawPixels.Count} raw pixels...".__INFO_ECHO

        Dim matrix As PixelData() = GetPixelsMatrix(rawPixels)

        Call $"rendering {matrix.Length} pixel blocks...".__INFO_ECHO

        Return RenderPixels(matrix, dimension, dimSize, colorSet, mapLevels, threshold)
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

