#Region "Microsoft.VisualBasic::694e5d6371254ebb2ce5bb7d2b71971a, src\visualize\MsImaging\Reader\IndexReader.vb"

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

    '     Class IndexReader
    ' 
    '         Properties: dimension
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
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.IndexedCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel

Namespace Reader

    Public Class IndexReader : Inherits PixelReader

        Public Overrides ReadOnly Property dimension As Size
            Get
                Return New Size(reader.meta.width, reader.meta.height)
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
            For x As Integer = 1 To reader.meta.width
                For y As Integer = 1 To reader.meta.height
                    Yield reader.GetPixel(x, y)
                Next
            Next
        End Function
    End Class
End Namespace
