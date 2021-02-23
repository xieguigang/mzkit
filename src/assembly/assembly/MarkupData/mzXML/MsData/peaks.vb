#Region "Microsoft.VisualBasic::03661671e65e891e5faa00823974e020, assembly\MarkupData\mzXML\MsData\peaks.vb"

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

    '     Class peaks
    ' 
    '         Properties: byteOrder, compressedLen, compressionType, contentType, precision
    '                     value
    ' 
    '         Function: GetCompressionType, GetPrecision, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace MarkupData.mzXML

    Public Class peaks : Implements IBase64Container

        ''' <summary>
        ''' 1. zlib
        ''' 2. gzip
        ''' 3. none
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property compressionType As String
        <XmlAttribute> Public Property compressedLen As Integer
        <XmlAttribute> Public Property precision As Double
        <XmlAttribute> Public Property byteOrder As String
        <XmlAttribute> Public Property contentType As String

        <XmlText>
        Public Property value As String Implements IBase64Container.BinaryArray

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Function GetPrecision() As Integer Implements IBase64Container.GetPrecision
            Return precision
        End Function

        Public Function GetCompressionType() As CompressionMode Implements IBase64Container.GetCompressionType
            If charToModes.ContainsKey(compressionType) Then
                Return charToModes(compressionType)
            Else
                Throw New NotImplementedException(compressionType)
            End If
        End Function
    End Class
End Namespace
