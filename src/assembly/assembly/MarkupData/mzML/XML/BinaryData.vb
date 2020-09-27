#Region "Microsoft.VisualBasic::af82e2dd7b100b28037c19754f0f6a4a, src\assembly\assembly\MarkupData\mzML\XML\BinaryData.vb"

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

    '     Class BinaryData
    ' 
    '         Properties: binaryDataArrayList, defaultArrayLength, id, index
    ' 
    '     Class binaryDataArrayList
    ' 
    '         Properties: list
    ' 
    '     Class binaryDataArray
    ' 
    '         Properties: binary, cvParams, encodedLength
    ' 
    '         Function: GetCompressionType, GetPrecision, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace MarkupData.mzML

    Public Class BinaryData : Inherits Params

        <XmlAttribute> Public Property index As Integer
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property defaultArrayLength As String

        Public Property binaryDataArrayList As binaryDataArrayList

    End Class

    Public Class binaryDataArrayList : Inherits List

        <XmlElement(NameOf(binaryDataArray))>
        Public Property list As binaryDataArray()

    End Class

    Public Class binaryDataArray : Implements IBase64Container

        Public Property encodedLength As Integer

        <XmlElement(NameOf(cvParam))>
        Public Property cvParams As cvParam()
        Public Property binary As String Implements IBase64Container.BinaryArray

        Public Overrides Function ToString() As String
            Return binary
        End Function

        Public Function GetPrecision() As Integer Implements IBase64Container.GetPrecision
            If Not cvParams.KeyItem("64-bit float") Is Nothing Then
                Return 64
            ElseIf Not cvParams.KeyItem("32-bit float") Is Nothing Then
                Return 32
            Else
                Throw New NotImplementedException
            End If
        End Function

        Public Function GetCompressionType() As String Implements IBase64Container.GetCompressionType
            If Not cvParams.KeyItem("zlib compression") Is Nothing Then
                Return "zlib"
            Else
                Throw New NotImplementedException
            End If
        End Function
    End Class
End Namespace
