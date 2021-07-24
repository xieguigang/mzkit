Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary

Namespace MarkupData.nmrML

    <XmlRoot("nmrML", [Namespace]:=nmrML.XML.namespace)>
    Public Class XML

        Public Const [namespace] As String = "http://nmrml.org/schema"

        <XmlAttribute> Public Property accession As String
        <XmlAttribute> Public Property accession_url As String
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property version As String

        Public Property cvList As cvList
        Public Property fileDescription As fileDescription
        Public Property contactList As contactList
        Public Property sourceFileList As sourceFileList

        <XmlElement(NameOf(acquisition))>
        Public Property acquisition As acquisition()

    End Class

    Public Class contactList

        <XmlElement(NameOf(contact))>
        Public Property contacts As contact()

    End Class

    Public Class contact

        <XmlAttribute> Public Property email As String
        <XmlAttribute> Public Property fullname As String
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property organization As String

    End Class

    Public Class sourceFileList

        <XmlElement(NameOf(sourceFile))>
        Public Property sourceFiles As sourceFile()

    End Class

    Public Class sourceFile : Inherits Params

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property location As String
        <XmlAttribute> Public Property name As String

    End Class
End Namespace