#Region "Microsoft.VisualBasic::e7a70a0f68c2f472306db687f9cc2b79, src\visualize\MsImaging\PixelsSampler.vb"

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

    ' Class PixelsSampler
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: CreateColumns, (+2 Overloads) GetBlock, Sampling
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Public Class PixelsSampler

    ''' <summary>
    ''' pixels[x][y]
    ''' </summary>
    ReadOnly col_scans As PixelScan()()
    ReadOnly dims As Size

    Sub New(reader As PixelReader)
        dims = reader.dimension
        col_scans = CreateColumns(reader, dims).ToArray
    End Sub

    Private Shared Iterator Function CreateColumns(reader As PixelReader, dims As Size) As IEnumerable(Of PixelScan())
        For x As Integer = 1 To dims.Width
            Dim column As PixelScan() = New PixelScan(dims.Height - 1) {}

            For y As Integer = 1 To dims.Height
                column(y - 1) = reader.GetPixel(x, y)
            Next

            Yield column
        Next
    End Function

    Public Function GetBlock(o As Point, width As Integer, height As Integer) As IEnumerable(Of PixelScan)
        Return GetBlock(o.X, o.Y, width, height)
    End Function

    Public Iterator Function GetBlock(px As Integer, py As Integer, width As Integer, height As Integer) As IEnumerable(Of PixelScan)
        For x As Integer = px To px + width
            If x > dims.Width Then
                Exit For
            End If

            For y As Integer = py To py + height
                If y > dims.Height Then
                    Exit For
                End If

                If Not col_scans(x - 1)(y - 1) Is Nothing Then
                    Yield col_scans(x - 1)(y - 1)
                End If
            Next
        Next
    End Function

    ''' <summary>
    ''' scale to new size for reduce data size for downstream analysis by
    ''' pixels sampling
    ''' </summary>
    ''' <param name="samplingSize"></param>
    ''' <returns></returns>
    Public Iterator Function Sampling(samplingSize As Size, tolerance As Tolerance) As IEnumerable(Of InMemoryVectorPixel)
        Dim dw As Integer = samplingSize.Width
        Dim dh As Integer = samplingSize.Height
        Dim block As NamedCollection(Of ms2)()

        For x As Integer = 1 To dims.Width Step dw
            For y As Integer = 1 To dims.Height Step dh
                If dw = 1 AndAlso dh = 1 Then
                    block = col_scans(x - 1)(y - 1) _
                        .GetMsPipe _
                        .GroupBy(tolerance) _
                        .ToArray
                Else
                    block = GetBlock(x, y, dw, dh) _
                        .Select(Function(p) p.GetMsPipe) _
                        .IteratesALL _
                        .GroupBy(tolerance) _
                        .ToArray
                End If

                Dim mz As Double() = block.Select(Function(d) Aggregate p In d Into Average(p.mz)).ToArray
                Dim into As Double() = block.Select(Function(d) Aggregate p In d Into Max(p.intensity)).ToArray

                Yield New InMemoryVectorPixel(x, y, mz, into)
            Next

            Call Console.Write(x & vbTab)
        Next
    End Function
End Class

