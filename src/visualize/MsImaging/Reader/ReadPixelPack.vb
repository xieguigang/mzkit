#Region "Microsoft.VisualBasic::9c65f6ceebeb1170f599c2ac8fe5e013, visualize\MsImaging\Reader\ReadPixelPack.vb"

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

    '   Total Lines: 71
    '    Code Lines: 55 (77.46%)
    ' Comment Lines: 3 (4.23%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 13 (18.31%)
    '     File Size: 2.60 KB


    '     Class ReadPixelPack
    ' 
    '         Properties: dimension, pixels, resolution
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: AllPixels, GetPixel, LoadMzArray
    ' 
    '         Sub: release
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace Reader

    ''' <summary>
    ''' handling of the cdf matrix file
    ''' </summary>
    Public Class ReadPixelPack : Inherits PixelReader

        Public Overrides ReadOnly Property dimension As Size
        Public ReadOnly Property pixels As PixelData()

        Public Overrides ReadOnly Property resolution As Double
            Get
                Return 17
            End Get
        End Property

        Dim matrix As Grid(Of InMemoryVectorPixel)

        Sub New(pixels As IEnumerable(Of PixelData), Optional dims As Size? = Nothing)
            Me.matrix = pixels _
                .GroupBy(Function(i) $"{i.x},{i.y}") _
                .Select(Function(i)
                            Dim mz As Double() = i.Select(Function(x) x.mz).ToArray
                            Dim into As Double() = i.Select(Function(x) x.intensity).ToArray

                            Return New InMemoryVectorPixel(i.First.x, i.First.y, mz, into, i.Key)
                        End Function) _
                .DoCall(AddressOf Grid(Of InMemoryVectorPixel).Create)
            Me.pixels = pixels

            If dims Is Nothing Then
                Me.dimension = New Size(matrix.width, matrix.height)
            Else
                Me.dimension = dims
            End If
        End Sub

        Protected Overrides Sub release()
            Erase _pixels
        End Sub

        Public Overrides Function GetPixel(x As Integer, y As Integer) As PixelScan
            Return matrix(x, y)
        End Function

        Public Overrides Function AllPixels() As IEnumerable(Of PixelScan)
            Return matrix.EnumerateData.Select(Function(i) DirectCast(i, PixelScan))
        End Function

        Public Overrides Function LoadMzArray(ppm As Double) As Double()
            Return pixels _
                .GroupBy(Function(d) d.mz, Tolerance.PPM(ppm)) _
                .Select(Function(d)
                            Return d _
                                .OrderByDescending(Function(i) i.intensity) _
                                .First _
                                .mz
                        End Function) _
                .ToArray
        End Function
    End Class

End Namespace
