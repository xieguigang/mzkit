Imports System.Xml.Serialization

Namespace TMIC.HMDB.Spectra

    <XmlType("ms-ms")> Public Class MSMS : Inherits SpectraFile
        Implements IPeakList(Of MSMSPeak)

        Public Property peakList As MSMSPeak() Implements IPeakList(Of MSMSPeak).peakList
    End Class

    <XmlType("ms-ms-peak")> Public Class MSMSPeak : Inherits MSPeak

        <XmlElement("ms-ms-id")>
        Public Property msmsID As String
        <XmlElement("molecule-id")>
        Public Property moleculeID As NullableValue

    End Class
End Namespace