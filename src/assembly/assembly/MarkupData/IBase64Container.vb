#Region "Microsoft.VisualBasic::ea074387e17450eaa8b5e6e5865ca8c7, src\assembly\assembly\MarkupData\IBase64Container.vb"

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

'     Interface IBase64Container
' 
'         Properties: BinaryArray
' 
'         Function: GetCompressionType, GetPrecision
' 
'     Module Base64Extensions
' 
'         Function: Base64Decode
' 
' 
' /********************************************************************************/

#End Region

Namespace MarkupData

    Public Interface IBase64Container

        Property BinaryArray As String

        Function GetPrecision() As Integer

        ''' <summary>
        ''' 1. zlib
        ''' 2. gzip
        ''' 3. none
        ''' </summary>
        ''' <returns></returns>
        Function GetCompressionType() As String

    End Interface
End Namespace
