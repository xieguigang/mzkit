#Region "Microsoft.VisualBasic::acb9b042294ec23d32d0ec4584e7ec0f, src\metadna\metaDNA\Result\MetaDNARawSet.vb"

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

' Class MetaDNARawSet
' 
'     Properties: Inference, TypeComment
' 
'     Function: getCollection, getSize
' 
' /********************************************************************************/

#End Region

Imports System.Xml
Imports System.Xml.Serialization
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Text.Xml.Models

''' <summary>
''' a collection of peak alignment raw matrix data in XML data format
''' </summary>
Public Class MetaDNARawSet : Inherits ListOf(Of Candidate)
    Implements XmlDataModel.IXmlType

    Public Property TypeComment As XmlComment Implements XmlDataModel.IXmlType.TypeComment
        Get
            Return XmlDataModel.CreateTypeReferenceComment(GetType(Candidate))
        End Get
        Set(value As XmlComment)

        End Set
    End Property

    <XmlElement>
    Public Property Inference As Candidate()

    Protected Overrides Function getSize() As Integer
        Return Inference.TryCount
    End Function

    Protected Overrides Function getCollection() As IEnumerable(Of Candidate)
        Return Inference
    End Function
End Class
