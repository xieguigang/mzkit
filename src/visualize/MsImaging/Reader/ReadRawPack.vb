﻿#Region "Microsoft.VisualBasic::bd0aaa370450230e8c349a283d30232a, visualize\MsImaging\Reader\ReadRawPack.vb"

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

    '   Total Lines: 154
    '    Code Lines: 103 (66.88%)
    ' Comment Lines: 26 (16.88%)
    '    - Xml Docs: 53.85%
    ' 
    '   Blank Lines: 25 (16.23%)
    '     File Size: 6.15 KB


    '     Class ReadRawPack
    ' 
    '         Properties: dimension, resolution
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
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace Reader

    ''' <summary>
    ''' handling of the mzpack data file
    ''' </summary>
    Public Class ReadRawPack : Inherits PixelReader

        Public Overrides ReadOnly Property dimension As Size
        Public Overrides ReadOnly Property resolution As Double

        ''' <summary>
        ''' [x, y[]]
        ''' </summary>
        Dim pixels As Dictionary(Of String, mzPackPixel())

        Sub New(mzpack As IMZPack, Optional verbose As Boolean = True)
            Call mzpack.MS _
                .Select(Function(pixel)
                            Return New mzPackPixel(pixel)
                        End Function) _
                .DoCall(Sub(ls) loadPixelsArray(ls, verbose))

            Call ReadDimensions(mzpack, pixels.Select(Function(pr) pr.Value).IteratesALL, verbose, _resolution, _dimension)
        End Sub

        Sub New(raw As mzPackPixel(), Optional verbose As Boolean = True)
            Call loadPixelsArray(raw, verbose)
            Call ReadDimensions(mzpack:=Nothing, pixels.Select(Function(pr) pr.Value).IteratesALL, verbose, _resolution, _dimension)
        End Sub

        ' load from mzpack rawdata file
        'Sub New(mzpack As String, Optional verbose As Boolean = True)
        '    Using file As Stream = mzpack.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
        '        Call Assembly.mzPack _
        '            .ReadAll(file).MS _
        '            .Select(Function(pixel)
        '                        Return New mzPackPixel(pixel)
        '                    End Function) _
        '            .DoCall(Sub(ls) loadPixelsArray(ls, verbose))

        '        Call ReadDimensions(mzpack:=Nothing, verbose)
        '    End Using
        'End Sub

        Sub New(pixels As IEnumerable(Of mzPackPixel), MsiDim As Size, resolution As Double, Optional verbose As Boolean = True)
            Me.dimension = MsiDim
            Me.resolution = resolution

            Call loadPixelsArray(pixels, verbose)
        End Sub

        Public Iterator Function GetScans() As IEnumerable(Of ScanMS1)
            For Each row In pixels.Values.IteratesALL
                Yield row.scan
            Next
        End Function

        ''' <summary>
        ''' build a [x,y] matrix
        ''' </summary>
        ''' <param name="pixels"></param>
        Private Sub loadPixelsArray(pixels As IEnumerable(Of mzPackPixel), verbose As Boolean)
            If verbose Then
                Call RunSlavePipeline.SendMessage("create grid data...")
            End If

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

        ''' <summary>
        ''' the required mzpack object could be nothing
        ''' </summary>
        ''' <param name="mzpack"></param>
        Friend Overloads Shared Sub ReadDimensions(mzpack As IMZPack, pixels As IEnumerable(Of PixelScan), verbose As Boolean, ByRef resolution As Double, ByRef dimension As Size)
            Dim metadata As Dictionary(Of String, String)
            Dim polygon As New Polygon2D(pixels.Select(Function(p) New Point(p.X, p.Y)))

            If mzpack Is Nothing OrElse mzpack.metadata.IsNullOrEmpty Then
                metadata = New Dictionary(Of String, String)
            Else
                metadata = mzpack.metadata
            End If

            If verbose Then
                Call RunSlavePipeline.SendMessage("detect canvas dimensions...")
            End If

            Dim width As Integer = Val(metadata.TryGetValue("width", [default]:=polygon.xpoints.Max))
            Dim height As Integer = Val(metadata.TryGetValue("height", [default]:=polygon.ypoints.Max))

            resolution = Val(metadata.TryGetValue("resolution", [default]:=17))
            dimension = New Size(width, height)
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
