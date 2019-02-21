Imports System.Xml.Serialization

Namespace NCBI.PubChem

    <XmlType("Record", [Namespace]:="http://pubchem.ncbi.nlm.nih.gov/pug_view")>
    Public Class PugView

        Public Property RecordType As String
        Public Property RecordNumber As String

        <XmlElement("Section")>
        Public Property Sections As Section()

    End Class

    Public Class Reference
        Public Property ReferenceNumber As String
        Public Property SourceName As String
        Public Property SourceID As String
        Public Property Name As String
        Public Property URL As String
    End Class

    Public Class Section
        Public Property TOCHeading As String
        Public Property Description As String
        Public Property HintGroupSubsectionsByReference As Boolean
        <XmlElement("Information")>
        Public Property Information As Information()
        <XmlElement("Section")>
        Public Property Sections As Section()
    End Class

    Public Class Information
        Public Property ReferenceNumber As String
        Public Property Name As String
        Public Property BoolValue As Boolean
        Public Property Description As String
        Public Property NumValue As String
        Public Property StringValue As String
        Public Property ValueUnit As String
        Public Property URL As String
        Public Property ExternalDataURL As String
        Public Property ExternalDataMimeType As String
    End Class

    Public Class Table

        <XmlElement("ColumnName")>
        Public Property ColumnNames As String()
        <XmlElement("Row")>
        Public Property Rows As Row()

    End Class

    Public Class Row

        <XmlElement("Cell")>
        Public Property Cells As Information()
    End Class

End Namespace