Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel

Namespace TMIC.HMDB.Spectra

    Public Class NullableValue

        <XmlAttribute>
        Public Property nil As Boolean
        <XmlText>
        Public Property value As String

        Public Overrides Function ToString() As String
            If nil Then
                Return "null"
            Else
                Return value
            End If
        End Function

        Public Shared Narrowing Operator CType(value As NullableValue) As String
            Return value.value
        End Operator
    End Class

    Public Class reference
        Public Property id As String

        <XmlElement("spectra-id")> Public Property spectra_id As String
        <XmlElement("spectra-type")> Public Property spectra_type As String
        <XmlElement("pubmed-id")> Public Property pubmed_id As String
        <XmlElement("ref-text")> Public Property ref_text As String
        <XmlElement("database")> Public Property database As String
        <XmlElement("database-id")> Public Property database_id As String
    End Class

    Public MustInherit Class SpectraFile : Inherits XmlDataModel

        Public Property id As String
        Public Property notes As NullableValue

        <XmlElement("database-id")> Public Property database_id As NullableValue
        <XmlElement("peak-counter")> Public Property peak_counter As NullableValue

        <XmlElement("created-at")> Public Property created_at As NullableValue
        <XmlElement("updated-at")> Public Property updated_at As NullableValue

        Public Property references As reference()

    End Class

    Public Interface IPeakList(Of T As Peak)
        Property peakList As T()
    End Interface

    Public MustInherit Class Peak

        Public Property id As String
        Public Property intensity As Double

        Public Overrides Function ToString() As String
            Return $"[{id} => {intensity}]"
        End Function
    End Class

    Public Class MSPeak : Inherits Peak
        <XmlElement("mass-charge")>
        Public Property mass_charge As Double
    End Class
End Namespace