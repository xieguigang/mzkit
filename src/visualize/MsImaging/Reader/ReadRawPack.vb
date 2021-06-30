#Region "Microsoft.VisualBasic::89af53a77fc77f036c08144a5ff048b6, src\visualize\MsImaging\Reader\ReadRawPack.vb"

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

    '     Class ReadRawPack
    ' 
    '         Properties: dimension
    ' 
    '         Constructor: (+3 Overloads) Sub New
    ' 
    '         Function: AllPixels, GetPixel, LoadMzArray
    ' 
    '         Sub: ReadDimensions, release
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace Reader

    Public Class ReadRawPack : Inherits PixelReader

        Public Overrides ReadOnly Property dimension As Size

        Dim pixels As mzPackPixel()

        Sub New(mzpack As Assembly.mzPack)
            Me.pixels = mzpack.MS _
                .Select(Function(pixel)
                            Return New mzPackPixel(pixel)
                        End Function) _
                .ToArray

            Call ReadDimensions()
        End Sub

        Sub New(mzpack As String)
            Using file As Stream = mzpack.Open
                Me.pixels = Assembly.mzPack _
                    .ReadAll(file).MS _
                    .Select(Function(pixel)
                                Return New mzPackPixel(pixel)
                            End Function) _
                    .ToArray
            End Using

            Call ReadDimensions()
        End Sub

        Public Overrides Function GetPixel(x As Integer, y As Integer) As PixelScan
            Return pixels _
                .Where(Function(p) p.X = x AndAlso p.Y = y) _
                .FirstOrDefault
        End Function

        Private Sub ReadDimensions()
            Dim width As Integer = pixels.Select(Function(p) p.X).Max
            Dim height As Integer = pixels.Select(Function(p) p.Y).Max

            _dimension = New Size(width, height)
        End Sub

        Sub New(pixels As IEnumerable(Of mzPackPixel), MsiDim As Size)
            Me.dimension = MsiDim
            Me.pixels = pixels.ToArray
        End Sub

        Protected Overrides Sub release()
            For Each p In pixels
                Call p.release()
            Next

            Erase pixels
        End Sub

        Public Overrides Function LoadMzArray(ppm As Double) As Double()
            Dim mzlist As Double() = pixels _
                .Select(Function(p) p.mz) _
                .IteratesALL _
                .ToArray
            Dim groups = mzlist _
                .GroupBy(Function(mz) mz, Tolerance.PPM(ppm)) _
                .Select(Function(mz) Val(mz.name)) _
                .OrderBy(Function(mzi) mzi) _
                .ToArray

            Return groups
        End Function

        Protected Overrides Function AllPixels() As IEnumerable(Of PixelScan)
            Return pixels.Select(Function(p) DirectCast(p, PixelScan))
        End Function
    End Class
End Namespace
