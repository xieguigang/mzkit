Imports System.Xml.Serialization

Namespace TMIC.HMDB.Spectra

    Public Class NMR1D : Inherits SpectraFile
        Implements IPeakList(Of Nmr1DPeak)

        Public Property peakList As Nmr1DPeak() Implements IPeakList(Of Nmr1DPeak).peakList
    End Class

    Public Class Nmr1DPeak : Inherits Peak

        <XmlElement("nmr-one-d-id")> Public Property Nmr1Did As String
        <XmlElement("chemical-shift")>
        Public Property chemicalShift As Double

    End Class
End Namespace