Imports System.Xml.Serialization

Namespace TMIC.HMDB.Spectra

    <XmlType("c-ms")> Public Class CMS : Inherits SpectraFile
        Implements IPeakList(Of c_ms_peak)

        <XmlElement("derivative-smiles")> Public Property derivative_smiles As NullableValue
        <XmlElement("derivative-exact-mass")> Public Property derivative_exact_mass As NullableValue
        <XmlElement("derivative-formula")> Public Property derivative_formula As NullableValue
        <XmlElement("derivative-mw")> Public Property derivative_mw As NullableValue
        <XmlElement("derivative-type")> Public Property derivative_type As NullableValue

        <XmlArray("c-ms-peaks")>
        Public Property peakList As c_ms_peak() Implements IPeakList(Of c_ms_peak).peakList

    End Class

    <XmlType("c-ms-peak")> Public Class c_ms_peak : Inherits MSPeak

        <XmlElement("c-ms-id")>
        Public Property c_ms_id As String

    End Class
End Namespace