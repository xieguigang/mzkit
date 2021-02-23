#Region "Microsoft.VisualBasic::46be4499b4c3c6e501268384fb5fbadc, assembly\MarkupData\mzXML\index.vb"

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

'     Class index
' 
'         Properties: name, offsets
' 
'         Function: ToString
' 
'     Structure offset
' 
'         Properties: id, value
' 
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace MarkupData.mzXML

    Public Class index

        <XmlAttribute>
        Public Property name As String

        <XmlElement("offset")>
        Public Property offsets As offset()

        Public Overrides Function ToString() As String
            Return name
        End Function

        Public Function GetOffsets() As IEnumerable(Of NamedValue(Of Long))
            Return offsets _
                .Select(Function(off)
                            Return New NamedValue(Of Long) With {
                                .Name = off.id,
                                .Value = off.value,
                                .Description = name
                            }
                        End Function)
        End Function

        Friend Shared Iterator Function ParseIndexList(bin As BinaryDataReader, offset As Long) As IEnumerable(Of index)
            Call bin.Seek(offset, SeekOrigin.Begin)


        End Function
    End Class

    Public Structure offset

        <XmlAttribute>
        Public Property id As String
        <XmlText>
        Public Property value As Long

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace
