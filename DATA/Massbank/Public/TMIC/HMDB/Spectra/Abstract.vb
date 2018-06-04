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
        Public Property predicted As Boolean

        ''' <summary>
        ''' HMDB ID
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("database-id")> Public Property database_id As NullableValue
        <XmlElement("peak-counter")> Public Property peak_counter As NullableValue

        <XmlElement("sample-concentration")> Public Property sample_concentration As NullableValue
        <XmlElement("sample-concentration-units")> Public Property sample_concentration_units As NullableValue

        <XmlElement("solvent")> Public Property solvent As NullableValue
        <XmlElement("sample-mass")> Public Property sample_mass As NullableValue
        <XmlElement("sample-mass-units")> Public Property sample_mass_units As NullableValue
        <XmlElement("sample-assessment")> Public Property sample_assessment As NullableValue
        <XmlElement("spectra-assessment")> Public Property spectra_assessment As NullableValue
        <XmlElement("sample-source")> Public Property sample_source As NullableValue
        <XmlElement("collection-date")> Public Property collection_date As NullableValue
        <XmlElement("instrument-type")> Public Property instrument_type As NullableValue
        <XmlElement("chromatography-type")> Public Property chromatography_type As NullableValue
        <XmlElement("retention-index")> Public Property retention_index As NullableValue
        <XmlElement("retention-time")> Public Property retention_time As NullableValue
        <XmlElement("ionization-mode")> Public Property ionization_mode As NullableValue
        <XmlElement("column-type")> Public Property column_type As NullableValue
        <XmlElement("base-peak")> Public Property base_peak As NullableValue
        <XmlElement("ri-type")> Public Property ri_type As NullableValue
        <XmlElement("structure-id")> Public Property structure_id As NullableValue
        <XmlElement("splash-key")> Public Property splash_key As NullableValue

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