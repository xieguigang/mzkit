Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Math
Imports Application = Microsoft.VisualBasic.Parallel

''' <summary>
''' MS-imaging render canvas
''' </summary>
Public Class Drawer : Implements IDisposable

    Dim disposedValue As Boolean
    Dim pixels As ScanData()

    Public ReadOnly Property ibd As ibdReader
    Public ReadOnly Property dimension As Size

    Public ReadOnly Property UUID As String
        Get
            Return ibd.UUID
        End Get
    End Property

    Sub New(imzML As String)
        ibd = ibdReader.Open(imzML.ChangeSuffix("ibd"))
        pixels = XML.LoadScans(file:=imzML).ToArray
        dimension = New Size With {
            .Width = pixels.Select(Function(p) p.x).Max,
            .Height = pixels.Select(Function(p) p.y).Max
        }
    End Sub

    Public Function LoadMzArray(ppm As Double) As Double()
        Dim mzlist = pixels _
            .Select(Function(p) Application.DoEvents(Function() ibd.ReadArray(p.MzPtr))) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim groups = mzlist _
            .GroupBy(Function(mz) mz, Tolerance.PPM(ppm)) _
            .Select(Function(mz) Val(mz.name)) _
            .ToArray

        Return groups
    End Function

    Public Iterator Function LoadPixels(mz As Double, ppm As Double, Optional skipZero As Boolean = True) As IEnumerable(Of PixelData)
        Dim pixel As PixelData

        For Each point As ScanData In Me.pixels
            Dim msScan As ms2() = ibd.GetMSMS(point)
            Dim into As ms2 = msScan _
                .Where(Function(mzi) PPMmethod.PPM(mzi.mz, mz) <= ppm) _
                .OrderByDescending(Function(mzi) mzi.intensity) _
                .FirstOrDefault

            If skipZero AndAlso into Is Nothing Then
                Continue For
            Else
                pixel = New PixelData With {
                    .x = point.x,
                    .y = point.y,
                    .intensity = If(into Is Nothing, 0, into.intensity)
                }
            End If

            Yield pixel
        Next
    End Function

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
    ''' <param name="ppm"></param>
    ''' <param name="colorSet"></param>
    ''' <param name="mapLevels%"></param>
    ''' <returns></returns>
    Public Function DrawLayer(mz As Double,
                              Optional threshold As Double = 0.1,
                              Optional pixelSize$ = "5,5",
                              Optional ppm As Double = 5,
                              Optional colorSet As String = "YlGnBu:c8",
                              Optional mapLevels% = 25) As Bitmap

        Dim dimSize As Size = pixelSize.SizeParser

        Call $"loading pixel datas [m/z={mz.ToString("F4")}]...".__INFO_ECHO

        Dim pixels As PixelData() = LoadPixels(mz, ppm).ToArray

        Call $"rendering {pixels.Length} pixel blocks...".__INFO_ECHO

        Return RenderPixels(pixels, dimension, dimSize, colorSet, mapLevels, threshold)
    End Function

    ''' <summary>
    ''' apply for pathway rendering 
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="threshold"></param>
    ''' <param name="pixelSize$"></param>
    ''' <param name="ppm"></param>
    ''' <param name="colorSet"></param>
    ''' <param name="mapLevels%"></param>
    ''' <returns></returns>
    Public Function DrawLayer(mz As Double(),
                              Optional threshold As Double = 0.1,
                              Optional pixelSize$ = "5,5",
                              Optional ppm As Double = 5,
                              Optional colorSet As String = "YlGnBu:c8",
                              Optional mapLevels% = 25) As Bitmap

        Dim dimSize As Size = pixelSize.SizeParser
        Dim pixels As New List(Of PixelData)
        Dim rawPixels As PixelData()

        For Each mzi As Double In mz
            Call $"loading pixel datas [m/z={mzi.ToString("F4")}]...".__INFO_ECHO

            rawPixels = LoadPixels(mzi, ppm).ToArray
            rawPixels = PixelData.ScalePixels(rawPixels)

            Call pixels.AddRange(rawPixels)
        Next

        Call $"building pixel matrix from {pixels.Count} raw pixels...".__INFO_ECHO

        Dim matrix As PixelData() = pixels _
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

        Call $"rendering {matrix.Length} pixel blocks...".__INFO_ECHO

        Return RenderPixels(matrix, dimension, dimSize, colorSet, mapLevels, threshold)
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call ibd.Dispose()
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
