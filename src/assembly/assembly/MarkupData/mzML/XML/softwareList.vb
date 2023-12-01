Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary

Namespace MarkupData.mzML

    Public Class softwareList : Inherits List

        <XmlElement("software")>
        Public Property softwares As software()

    End Class

    Public Class software : Inherits Params

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property version As String

    End Class
End Namespace