#Region "Microsoft.VisualBasic::d13551e5a44bbcb73a32fcb840029bf2, mzkit\src\visualize\MsImaging\Reader\ReadRawPack.vb"

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

'   Total Lines: 116
'    Code Lines: 93
' Comment Lines: 3
'   Blank Lines: 20
'     File Size: 4.03 KB


'     Class ReadRawPack
' 
'         Properties: dimension
' 
'         Constructor: (+3 Overloads) Sub New
' 
'         Function: AllPixels, GetPixel, GetScans, LoadMzArray
' 
'         Sub: loadPixelsArray, ReadDimensions, release
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace Reader

    Public Class ReadPixelPack : Inherits PixelReader

        Public Overrides ReadOnly Property dimension As Size
        Public ReadOnly Property pixels As PixelData()

        Dim matrix As Grid(Of InMemoryVectorPixel)

        Sub New(pixels As IEnumerable(Of PixelData))
            Me.matrix = pixels _
                .GroupBy(Function(i) $"{i.x},{i.y}") _
                .Select(Function(i)
                            Dim mz As Double() = i.Select(Function(x) x.mz).ToArray
                            Dim into As Double() = i.Select(Function(x) x.intensity).ToArray

                            Return New InMemoryVectorPixel(i.First.x, i.First.y, mz, into, i.Key)
                        End Function) _
                .DoCall(AddressOf Grid(Of InMemoryVectorPixel).Create)
            Me.pixels = pixels
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

    Public Class ReadRawPack : Inherits PixelReader

        Public Overrides ReadOnly Property dimension As Size

        ''' <summary>
        ''' [x, y[]]
        ''' </summary>
        Dim pixels As Dictionary(Of String, mzPackPixel())

        Sub New(mzpack As Assembly.mzPack)
            Call mzpack.MS _
                .Select(Function(pixel)
                            Return New mzPackPixel(pixel)
                        End Function) _
                .DoCall(AddressOf loadPixelsArray)

            Call ReadDimensions()
        End Sub

        Sub New(mzpack As String)
            Using file As Stream = mzpack.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Call Assembly.mzPack _
                    .ReadAll(file).MS _
                    .Select(Function(pixel)
                                Return New mzPackPixel(pixel)
                            End Function) _
                    .DoCall(AddressOf loadPixelsArray)

                Call ReadDimensions()
            End Using
        End Sub

        Sub New(pixels As IEnumerable(Of mzPackPixel), MsiDim As Size)
            Me.dimension = MsiDim

            Call loadPixelsArray(pixels)
        End Sub

        Public Iterator Function GetScans() As IEnumerable(Of ScanMS1)
            For Each row In pixels.Values.IteratesALL
                Yield row.scan
            Next
        End Function

        Private Sub loadPixelsArray(pixels As IEnumerable(Of mzPackPixel))
            Me.pixels = pixels _
                .GroupBy(Function(p) p.X) _
                .ToDictionary(Function(p) p.Key.ToString,
                              Function(p)
                                  Return p.ToArray
                              End Function)
        End Sub

        Public Overrides Function GetPixel(x As Integer, y As Integer) As PixelScan
            If Not pixels.ContainsKey(x.ToString) Then
                Return Nothing
            Else
                Return (From p As mzPackPixel In pixels(key:=x.ToString) Where p.Y = y).FirstOrDefault
            End If
        End Function

        Private Overloads Sub ReadDimensions()
            Dim width As Integer = pixels.Select(Function(pr) Aggregate p In pr.Value Into Max(p.X)).Max
            Dim height As Integer = pixels.Select(Function(pr) Aggregate p In pr.Value Into Max(p.Y)).Max

            _dimension = New Size(width, height)
        End Sub

        Protected Overrides Sub release()
            For Each pr In pixels
                For Each p In pr.Value
                    Call p.release()
                Next
            Next

            pixels.Clear()
        End Sub

        Public Overrides Function LoadMzArray(ppm As Double) As Double()
            Dim mzlist As Double() = pixels _
                .Select(Function(x) x.Value) _
                .IteratesALL _
                .Select(Function(p) p.mz) _
                .IteratesALL _
                .ToArray
            Dim groups As Double() = mzlist _
                .GroupBy(Function(mz) mz, Tolerance.PPM(ppm)) _
                .Select(Function(mz) Val(mz.name)) _
                .OrderBy(Function(mzi) mzi) _
                .ToArray

            Return groups
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function AllPixels() As IEnumerable(Of PixelScan)
            Return pixels _
                .Select(Function(x) x.Value) _
                .IteratesALL _
                .Select(Function(p)
                            Return DirectCast(p, PixelScan)
                        End Function)
        End Function
    End Class
End Namespace
