Imports System.Xml.Serialization
Imports MetaInfo = SMRUCC.MassSpectrum.DATA.MetaLib.MetaLib

Namespace NCBI.PubChem

    <XmlRoot("Record", [Namespace]:="http://pubchem.ncbi.nlm.nih.gov/pug_view")>
    Public Class PugViewRecord

        Public Property RecordType As String
        Public Property RecordNumber As String

        <XmlElement("Section")>
        Public Property Sections As Section()
            Get
                Return sectionTable.Values.ToArray
            End Get
            Set(value As Section())
                sectionTable = value.ToDictionary(Function(sec) sec.TOCHeading)
            End Set
        End Property

        Dim sectionTable As Dictionary(Of String, Section)

        Public Function GetMetaInfo() As MetaInfo
            Dim identifier = sectionTable("Names and Identifiers")

            Return New MetaInfo ' With {}
        End Function

        Public Overrides Function ToString() As String
            Return RecordNumber
        End Function
    End Class
End Namespace