Imports System.Runtime.Serialization

Namespace NCBI.PubChem.DataSources

    Public Class Annotation

        Public Property SourceName As String
        Public Property SourceID As String
        Public Property Name As String
        Public Property Description As String
        Public Property URL As String
        Public Property LicenseNote As String
        Public Property LicenseURL As String
        Public Property ANID As String
        Public Property Data As Data()
        Public Property LinkedRecords As LinkedRecords

    End Class

    Public Class Data

        Public Property TOCHeading As TOCHeading
        Public Property Name As String
        Public Property Value As Value
        Public Property URL As String
        Public Property Description As String

    End Class

    Public Class TOCHeading

        Public Property type As String

        <DataMember(Name:="#TOCHeading")>
        Public Property TOCHeading As String

    End Class

    Public Class LinkedRecords

        Public Property CID As String()
        Public Property SID As String()

    End Class

End Namespace