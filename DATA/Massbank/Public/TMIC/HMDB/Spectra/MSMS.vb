Imports System.Xml.Serialization

Namespace TMIC.HMDB.Spectra

    <XmlType("ms-ms")> Public Class MSMS : Inherits SpectraFile
        Implements IPeakList(Of MSMSPeak)

        <XmlElement("energy-field")> Public Property energyField As NullableValue
        <XmlElement("collision-energy-level")> Public Property collisionEnergyLevel As NullableValue
        <XmlElement("collision-energy-voltage")> Public Property collisionEnergyVoltage As NullableValue

        <XmlArray("ms-ms-peaks")>
        Public Property peakList As MSMSPeak() Implements IPeakList(Of MSMSPeak).peakList

    End Class

    <XmlType("ms-ms-peak")> Public Class MSMSPeak : Inherits MSPeak

        <XmlElement("ms-ms-id")>
        Public Property msmsID As String
        <XmlElement("molecule-id")>
        Public Property moleculeID As NullableValue

    End Class
End Namespace