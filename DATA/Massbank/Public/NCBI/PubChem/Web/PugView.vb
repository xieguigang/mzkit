Imports System.Xml.Serialization
Imports MetaInfo = SMRUCC.MassSpectrum.DATA.MetaLib.MetaLib

Namespace NCBI.PubChem

    <XmlRoot("Record", [Namespace]:="http://pubchem.ncbi.nlm.nih.gov/pug_view")>
    Public Class PugViewRecord

        Public Property RecordType As String
        Public Property RecordNumber As String

        <XmlElement("Section")>
        Public Property Sections As Section()

        Public Function GetMetaInfo() As MetaInfo

        End Function
    End Class
End Namespace