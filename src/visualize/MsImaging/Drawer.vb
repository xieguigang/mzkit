Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Scripting.Runtime

Public Class Drawer : Implements IDisposable

    Dim disposedValue As Boolean
    Dim ibd As ibdReader
    Dim pixels As ScanData()
    Dim dimension As Size

    Sub New(imzML As String)
        ibd = ibdReader.Open(imzML.ChangeSuffix("ibd"))
        pixels = XML.LoadScans(file:=imzML).ToArray
        dimension = New Size With {
            .Width = pixels.Select(Function(p) p.x).Max,
            .Height = pixels.Select(Function(p) p.y).Max
        }
    End Sub

    Public Iterator Function LoadPixels(mz As Double, ppm As Double) As IEnumerable(Of PixelData)
        Dim pixel As PixelData

        For Each point As ScanData In Me.pixels
            Dim msScan As ms2() = ibd.GetMSMS(point)
            Dim into As ms2 = msScan _
                .Where(Function(mzi) PPMmethod.PPM(mzi.mz, mz) <= ppm) _
                .OrderByDescending(Function(mzi) mzi.intensity) _
                .FirstOrDefault

            pixel = New PixelData With {
                .x = point.x,
                .y = point.y,
                .intensity = If(into Is Nothing, 0, into.intensity)
            }

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
        Dim levelRange As DoubleRange = New Double() {0, 1}
        Dim level As Double
        Dim rect As Rectangle
        Dim pos As Point
        Dim indexrange As DoubleRange = New Double() {0, colors.Length - 1}
        Dim intensityRange As DoubleRange = pixels _
            .Select(Function(p) p.intensity) _
            .Range

        Using layer As Graphics2D = New Bitmap(dimension.Width * dimSize.Width, dimension.Height * dimSize.Height)
            For Each point As PixelData In pixels
                level = intensityRange.ScaleMapping(point.intensity, levelRange)

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

    Public Function DrawLayer(mz As Double,
                              Optional threshold As Double = 0.1,
                              Optional pixelSize$ = "5,5",
                              Optional ppm As Double = 5,
                              Optional colorSet As String = "YlGnBu:c8",
                              Optional mapLevels% = 25) As Bitmap

        Dim dimSize As Size = pixelSize.SizeParser

        Call "loading pixel datas...".__INFO_ECHO

        Dim pixels As PixelData() = LoadPixels(mz, ppm)

        Call "rendering pixel blocks...".__INFO_ECHO

        Return RenderPixels(pixels, dimension, dimSize, colorSet, mapLevels, threshold)
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
