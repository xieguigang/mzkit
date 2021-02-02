#Region "Microsoft.VisualBasic::2b59f27d18f2657b019d42051d7bd89e, assembly\MarkupData\Base64Decode.vb"

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

    '     Module Base64Decoder
    ' 
    '         Function: Base64Decode
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Net.Http

Namespace MarkupData

    <HideModuleName>
    Public Module Base64Decoder

        ''' <summary>
        ''' 对质谱扫描信号结果进行解码操作
        ''' </summary>
        ''' <param name="stream">Container for the binary base64 string data.</param>
        ''' <returns></returns>
        <Extension>
        Public Function Base64Decode(stream As IBase64Container, Optional networkByteOrder As Boolean = False) As Double()
            Dim bytes As Byte() = Convert.FromBase64String(stream.BinaryArray)
            Dim floats#()
            Dim byteStream As MemoryStream

            Select Case stream.GetCompressionType
                Case CompressionMode.zlib
                    ' 2018-11-15 经过测试，与zlib的结果一致
                    byteStream = bytes.UnZipStream
                Case CompressionMode.gzip
                    byteStream = bytes.UnGzipStream
                Case CompressionMode.none
                    byteStream = New MemoryStream(bytes)
                Case Else
                    Throw New NotImplementedException(stream.GetCompressionType)
            End Select

            Using byteStream
                bytes = byteStream.ToArray
            End Using

            If networkByteOrder AndAlso BitConverter.IsLittleEndian Then
                Call Array.Reverse(bytes)
            End If

            Select Case stream.GetPrecision
                Case 64
                    floats = bytes _
                        .Split(8) _
                        .Select(Function(b) BitConverter.ToDouble(b, Scan0)) _
                        .ToArray
                Case 32
                    floats = bytes _
                        .Split(4) _
                        .Select(Function(b) BitConverter.ToSingle(b, Scan0)) _
                        .Select(Function(s) Val(s)) _
                        .ToArray
                Case Else
                    Throw New NotImplementedException
            End Select

            Return floats
        End Function
    End Module
End Namespace
