#Region "Microsoft.VisualBasic::58eebdb24350b3c245dd8221150780f5, mzkit\src\mzmath\SingleCells\Deconvolute\MzMatrix.vb"

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

'   Total Lines: 54
'    Code Lines: 26
' Comment Lines: 18
'   Blank Lines: 10
'     File Size: 1.67 KB


'     Class MzMatrix
' 
'         Properties: matrix, mz, tolerance
' 
'         Function: ExportCsvSheet
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text

Namespace Deconvolute

    ''' <summary>
    ''' a data matrix object in format of row is pixel and 
    ''' column is mz intensity value across different 
    ''' pixels
    ''' </summary>
    Public Class MzMatrix

        ''' <summary>
        ''' m/z vector in numeric format of round to digit 4 
        ''' </summary>
        ''' <returns></returns>
        Public Property mz As Double()

        ''' <summary>
        ''' the script string of the mz diff tolerance for <see cref="mz"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property tolerance As String

        ''' <summary>
        ''' MS-imaging pixel data matrix or the 
        ''' cells data matrix in a single cells raw data 
        ''' </summary>
        ''' <returns></returns>
        Public Property matrix As PixelData()

        Public ReadOnly Property featureSize As Integer
            Get
                Return mz.TryCount
            End Get
        End Property

        ''' <summary>
        ''' Create a dataset matrix of spatial spot id or single cell id in rows and ions mz features in columns. 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <returns></returns>
        Public Iterator Function ExportSpatial(Of T As {New, INamedValue, DynamicPropertyBase(Of Double)})() As IEnumerable(Of T)
            Dim mzId As String() = mz _
                .Select(Function(mzi) mzi.ToString("F4")) _
                .ToArray

            For Each spot As PixelData In matrix
                Dim ds As New T With {
                   .Key = $"{spot.X},{spot.Y}"
                }
                Dim ms As Dictionary(Of String, Double) = ds.Properties

                For i As Integer = 0 To mzId.Length - 1
                    Call ms.Add(mzId(i), spot.intensity(i))
                Next

                ds.Properties = ms

                Yield ds
            Next
        End Function

        ''' <summary>
        ''' Export a table file of the spatial pixel spot in rows and ions m/z features in columns 
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        Public Function ExportCsvSheet(file As Stream) As Boolean
            Dim text As New StreamWriter(file, Encodings.ASCII.CodePage) With {
                .NewLine = vbLf
            }
            Call text.WriteLine("Pixels," & mz.JoinBy(","))

            For Each pixelLine As String In matrix _
                .AsParallel _
                .Select(Function(pixel)
                            Return $"""{pixel.X},{pixel.Y}"",{pixel.intensity.JoinBy(",")}"
                        End Function)

                Call text.WriteLine(pixelLine)
            Next

            Call text.Flush()

            Return True
        End Function
    End Class
End Namespace
