Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary

Namespace MarkupData.mzML

    Public Class scanSettingsList : Inherits List

        <XmlElement("scanSettings")> Public Property scanSettings As scanSettings()

    End Class

    Public Class scanSettings : Inherits Params

        <XmlAttribute> Public Property id As String

    End Class
End Namespace