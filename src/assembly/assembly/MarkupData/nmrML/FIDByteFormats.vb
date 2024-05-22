#Region "Microsoft.VisualBasic::b020f9efd6aa54b410bf20ba510062e7, assembly\assembly\MarkupData\nmrML\FIDByteFormats.vb"

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

    '   Total Lines: 64
    '    Code Lines: 48 (75.00%)
    ' Comment Lines: 10 (15.62%)
    '    - Xml Docs: 30.00%
    ' 
    '   Blank Lines: 6 (9.38%)
    '     File Size: 2.58 KB


    '     Module FIDByteFormats
    ' 
    '         Function: DecodeBytes, DecodeDouble, DecodeInteger, decodeStream
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Net.Http

Namespace MarkupData.nmrML

    ''' <summary>
    ''' https://github.com/nmrML/nmrML/blob/99b854e7dc61ca85fcae16a068691dde160fa012/tools/Parser_and_Converters/R/nmRIO/R/readNMRMLFID.R#L31
    ''' </summary>
    Public Module FIDByteFormats

        <Extension>
        Public Function DecodeBytes(fidData As fidData) As Double()
            Select Case fidData.byteFormat.ToLower
                Case "complex64", "complex128"
                    Return fidData.DecodeDouble
                'Case "complex128"
                '    Return fidData.DecodeDouble _
                '        .Split(2) _
                '        .Select(Function(i) i(1)) _
                '        .ToArray
                Case "integer32", "complex32int", "class java.lang.integer"
                    Return fidData.DecodeInteger
                Case Else
                    Throw New NotImplementedException(fidData.byteFormat)
            End Select
        End Function

        <Extension>
        Public Function DecodeDouble(fidData As fidData) As Double()
            Using bytes As MemoryStream = fidData.decodeStream
                Return bytes.ToArray _
                    .Split(8) _
                    .Select(Function(byts)
                                'Array.Reverse(byts)
                                Return BitConverter.ToDouble(byts, Scan0)
                            End Function) _
                    .ToArray
            End Using
        End Function

        <Extension>
        Private Function decodeStream(fidData As fidData) As MemoryStream
            If fidData.compressed = "false" OrElse fidData.compressed = "none" Then
                Return New MemoryStream(Convert.FromBase64String(fidData.base64))
            Else
                Return Convert.FromBase64String(fidData.base64).UnZipStream(noMagic:=True)
            End If
        End Function

        <Extension>
        Public Function DecodeInteger(fidData As fidData) As Double()
            Using bytes As MemoryStream = fidData.decodeStream
                Return bytes.ToArray _
                    .Split(4) _
                    .Select(Function(byts)
                                'Array.Reverse(byts)
                                Return CDbl(BitConverter.ToInt32(byts, Scan0))
                            End Function) _
                    .ToArray
            End Using
        End Function
    End Module
End Namespace
