Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Imaging

Namespace MarkupData.imzML

    ''' <summary>
    ''' the MSI pixel data spot model of [x,z,intensity]
    ''' </summary>
    Public Class PixelScanIntensity : Implements IMSIPixel, RasterPixel, Pixel

        ''' <summary>
        ''' TIC
        ''' </summary>
        ''' <returns></returns>
        Public Property totalIon As Double Implements IMSIPixel.intensity, Pixel.Scale
        Public Property x As Integer Implements IMSIPixel.x, RasterPixel.X
        Public Property y As Integer Implements IMSIPixel.y, RasterPixel.Y

        Sub New()
        End Sub

        Sub New(x As Integer, y As Integer, intensity As Double)
            Me.x = x
            Me.y = y
            Me.totalIon = intensity
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetPoint() As Point
            Return New Point(x, y)
        End Function

        Public Overrides Function ToString() As String
            Return $"({x},{y}) {totalIon}"
        End Function

        Public Shared Function GetBuffer(summary As PixelScanIntensity()) As Byte()
            Using buf As New MemoryStream, file As New BinaryDataWriter(buf)
                Call file.Write(summary.Length)
                Call file.Write(summary.Select(Function(i) i.x).ToArray)
                Call file.Write(summary.Select(Function(i) i.y).ToArray)
                Call file.Write(summary.Select(Function(i) i.totalIon).ToArray)
                Call file.Flush()

                Return buf.ToArray
            End Using
        End Function

        Public Shared Function Parse(buffer As Byte()) As PixelScanIntensity()
            Using file As New BinaryDataReader(New MemoryStream(buffer))
                Dim size As Integer = file.ReadInt32
                Dim x As Integer() = file.ReadInt32s(size)
                Dim y As Integer() = file.ReadInt32s(size)
                Dim ions As Double() = file.ReadDoubles(size)

                Return ions _
                    .Select(Function(into, i)
                                Return New PixelScanIntensity With {
                                    .x = x(i),
                                    .y = y(i),
                                    .totalIon = ions(i)
                                }
                            End Function) _
                    .ToArray
            End Using
        End Function
    End Class

End Namespace