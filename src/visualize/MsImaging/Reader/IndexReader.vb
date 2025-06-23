﻿#Region "Microsoft.VisualBasic::d53b019ded8f4a481e6804b6d5d8ccee, visualize\MsImaging\Reader\IndexReader.vb"

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
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.IndexedCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel

Namespace Reader

    ''' <summary>
    ''' indexed in-memory mzpack data reader for the ms-imaging render
    ''' </summary>
    Public Class MemoryIndexReader : Inherits PixelReader

        Public Overrides ReadOnly Property resolution As Double
        Public Overrides ReadOnly Property dimension As Size

        Sub New(inMemory As IMZPack, Optional verbose As Boolean = True)

        End Sub

        Protected Overrides Sub release()
            Throw New NotImplementedException()
        End Sub

        Public Overrides Function GetPixel(x As Integer, y As Integer) As PixelScan
            Throw New NotImplementedException()
        End Function

        Public Overrides Function AllPixels() As IEnumerable(Of PixelScan)
            Throw New NotImplementedException()
        End Function

        Public Overrides Function LoadMzArray(ppm As Double) As Double()
            Throw New NotImplementedException()
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
