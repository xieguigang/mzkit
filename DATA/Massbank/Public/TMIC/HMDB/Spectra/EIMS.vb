Imports System.Xml.Serialization

Namespace TMIC.HMDB.Spectra

    <XmlType("ei-ms")> Public Class EIMS : Inherits SpectraFile
        Implements IPeakList(Of EIMSPeak)

        <XmlArray("ei-ms-peaks")>
        Public Property peakList As EIMSPeak() Implements IPeakList(Of EIMSPeak).peakList
    End Class

    <XmlType("ei-ms-peak")> Public Class EIMSPeak : Inherits MSPeak

        <XmlElement("ei-ms-id")>
        Public Property EImsId As String
    End Class
End Namespace