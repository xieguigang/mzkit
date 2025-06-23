#Region "Microsoft.VisualBasic::d53b019ded8f4a481e6804b6d5d8ccee, visualize\MsImaging\Reader\IndexReader.vb"

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

'   Total Lines: 57
'    Code Lines: 38 (66.67%)
' Comment Lines: 8 (14.04%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 11 (19.30%)
'     File Size: 1.65 KB


'     Class IndexReader
' 
'         Properties: dimension, resolution
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
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.IndexedCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace Reader

    ''' <summary>
    ''' indexed in-memory mzpack data reader for the ms-imaging render
    ''' </summary>
    Public Class MemoryIndexReader : Inherits PixelReader

        Public Overrides ReadOnly Property resolution As Double
        Public Overrides ReadOnly Property dimension As Size

        ''' <summary>
        ''' [x, y[]]
        ''' </summary>
        Dim pixels As Dictionary(Of String, IndexedMzPackMemory())

        Sub New(inMemory As IMZPack, Optional verbose As Boolean = True)
            Dim t0 = Now
            Dim n_threads As Integer = 4

            Call inMemory.MS _
                .SplitIterator(inMemory.MS.Length / n_threads) _
                .ToArray _
                .AsParallel _
                .WithDegreeOfParallelism(n_threads + 1) _
                .Select(Function(pixels) As IndexedMzPackMemory()
                            Return (From i As ScanMS1
                                    In pixels
                                    Select New IndexedMzPackMemory(i)).ToArray
                        End Function) _
                .IteratesALL _
                .DoCall(Sub(ls) loadPixelsArray(ls, verbose))

            Dim dt = Now - t0

            If verbose Then
                Call VBDebugger.EchoLine($"build in-memory index of the rawdata used {dt.Lanudry}")
            End If

            Call ReadRawPack.ReadDimensions(mzpack:=inMemory, pixels.Select(Function(pr) pr.Value).IteratesALL, verbose, _resolution, _dimension)
        End Sub

        ''' <summary>
        ''' build a [x,y] matrix
        ''' </summary>
        ''' <param name="pixels"></param>
        Private Sub loadPixelsArray(pixels As IEnumerable(Of IndexedMzPackMemory), verbose As Boolean)
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

        Protected Overrides Sub release()
            Call pixels.Clear()
        End Sub

        Public Overrides Function GetPixel(x As Integer, y As Integer) As PixelScan
            If Not pixels.ContainsKey(x.ToString) Then
                Return Nothing
            Else
                Return (From p As IndexedMzPackMemory
                        In pixels(key:=x.ToString)
                        Where p.Y = y).FirstOrDefault
            End If
        End Function

        Public Overrides Function AllPixels() As IEnumerable(Of PixelScan)
            Return pixels _
                .Select(Function(x) x.Value) _
                .IteratesALL _
                .Select(Function(p)
                            Return DirectCast(p, PixelScan)
                        End Function)
        End Function

        Public Overrides Function LoadMzArray(ppm As Double) As Double()
            Dim mzlist As Double() = pixels _
                .Select(Function(x) x.Value) _
                .IteratesALL _
                .Select(Function(p) p.scan.mz) _
                .IteratesALL _
                .ToArray
            Dim groups As Double() = mzlist _
                .GroupBy(Function(mz) mz, Tolerance.PPM(ppm)) _
                .Select(Function(mz) Val(mz.name)) _
                .OrderBy(Function(mzi) mzi) _
                .ToArray

            Return groups
        End Function
    End Class

    ''' <summary>
    ''' handling of the xic index file
    ''' </summary>
    Public Class IndexReader : Inherits PixelReader

        Public Overrides ReadOnly Property dimension As Size
            Get
                Return reader.dimension
            End Get
        End Property

        Public Overrides ReadOnly Property resolution As Double
            Get
                Return 17
            End Get
        End Property

        Dim reader As XICReader

        Sub New(reader As XICReader)
            Me.reader = reader
        End Sub

        Protected Overrides Sub release()
            Call reader.Dispose()
        End Sub

        Public Overrides Function GetPixel(x As Integer, y As Integer) As PixelScan
            Return reader.GetPixel(x, y)
        End Function

        ''' <summary>
        ''' load all mz
        ''' </summary>
        ''' <param name="ppm"></param>
        ''' <returns></returns>
        Public Overrides Function LoadMzArray(ppm As Double) As Double()
            Return reader.GetMz
        End Function

        Public Overrides Iterator Function AllPixels() As IEnumerable(Of PixelScan)
            Dim dims As Size = dimension

            For x As Integer = 1 To dims.Width
                For y As Integer = 1 To dims.Height
                    Yield reader.GetPixel(x, y)
                Next
            Next
        End Function
    End Class
End Namespace
