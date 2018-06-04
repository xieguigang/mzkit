Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel

Namespace TMIC.HMDB.Spectra

    <XmlType("c-ms")> Public Class CMS : Inherits SpectraFile(Of c_ms_peak)

        Public Property predicted As Boolean

        <XmlElement("sample-concentration")> Public Property sample_concentration As NullableValue
        <XmlElement("solvent")> Public Property solvent As NullableValue
        <XmlElement("sample-mass")> Public Property sample_mass As NullableValue
        <XmlElement("sample-assessment")> Public Property sample_assessment As NullableValue
        <XmlElement("spectra-assessment")> Public Property spectra_assessment As NullableValue
        <XmlElement("sample-source")> Public Property sample_source As NullableValue
        <XmlElement("collection-date")> Public Property collection_date As NullableValue
        <XmlElement("instrument-type")> Public Property instrument_type As NullableValue
        <XmlElement("chromatography-type")> Public Property chromatography_type As NullableValue
        <XmlElement("retention-index")> Public Property retention_index As NullableValue
        <XmlElement("retention-time")> Public Property retention_time As NullableValue
        <XmlElement("sample-concentration-units")> Public Property sample_concentration_units As NullableValue
        <XmlElement("sample-mass-units")> Public Property sample_mass_units As NullableValue
        <XmlElement("ionization-mode")> Public Property ionization_mode As NullableValue
        <XmlElement("column-type")> Public Property column_type As NullableValue
        <XmlElement("base-peak")> Public Property base_peak As NullableValue
        <XmlElement("derivative-type")> Public Property derivative_type As NullableValue
        <XmlElement("ri-type")> Public Property ri_type As NullableValue
        <XmlElement("derivative-formula")> Public Property derivative_formula As NullableValue
        <XmlElement("derivative-mw")> Public Property derivative_mw As NullableValue
        <XmlElement("structure-id")> Public Property structure_id As NullableValue
        <XmlElement("splash-key")> Public Property splash_key As NullableValue
        <XmlElement("derivative-smiles")> Public Property derivative_smiles As NullableValue
        <XmlElement("derivative-exact-mass")> Public Property derivative_exact_mass As NullableValue

        <XmlElement("c-ms-peaks")>
        Public Property c_ms_peaks As c_ms_peak()

    End Class

    <XmlType("c-ms-peak")> Public Class c_ms_peak : Inherits Peak

        Public Property c_ms_id As String
        Public Property mass_charge As Double

    End Class
End Namespace