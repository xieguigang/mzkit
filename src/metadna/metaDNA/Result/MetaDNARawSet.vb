Imports System.Xml
Imports System.Xml.Serialization
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Text.Xml.Models

Public Class MetaDNARawSet : Inherits ListOf(Of CandidateInfer)
    Implements XmlDataModel.IXmlType

    Public Property TypeComment As XmlComment Implements XmlDataModel.IXmlType.TypeComment
        Get
            Return XmlDataModel.CreateTypeReferenceComment(GetType(CandidateInfer))
        End Get
        Set(value As XmlComment)

        End Set
    End Property

    <XmlElement>
    Public Property Inference As CandidateInfer()

    Protected Overrides Function getSize() As Integer
        Return Inference.TryCount
    End Function

    Protected Overrides Function getCollection() As IEnumerable(Of CandidateInfer)
        Return Inference
    End Function
End Class
