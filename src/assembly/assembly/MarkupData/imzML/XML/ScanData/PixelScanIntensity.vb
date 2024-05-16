#Region "Microsoft.VisualBasic::5ff881d2d982513d40ab702cd66659f4, assembly\assembly\MarkupData\imzML\XML\ScanData\PixelScanIntensity.vb"

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

    '   Total Lines: 81
    '    Code Lines: 54
    ' Comment Lines: 15
    '   Blank Lines: 12
    '     File Size: 2.93 KB


    '     Class PixelScanIntensity
    ' 
    '         Properties: totalIon, x, y
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetBuffer, GetPoint, Parse, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
        ''' <summary>
        ''' the x axis position
        ''' </summary>
        ''' <returns></returns>
        Public Property x As Integer Implements IMSIPixel.x, RasterPixel.X
        ''' <summary>
        ''' the y axis position
        ''' </summary>
        ''' <returns></returns>
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
