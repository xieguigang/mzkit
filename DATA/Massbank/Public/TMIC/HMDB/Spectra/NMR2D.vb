Imports System.Xml.Serialization

Namespace TMIC.HMDB.Spectra

    Public Class NMR2D : Inherits SpectraFile
        Implements IPeakList(Of Nmr2DPeak)

        Public Property peakList As Nmr2DPeak() Implements IPeakList(Of Nmr2DPeak).peakList
    End Class

    Public Class Nmr2DPeak : Inherits Peak

        <XmlElement("nmr-two-d-id")> Public Property nmr2Did As String
        <XmlElement("chemical-shift-x")> Public Property chemicalShiftX As Double
        <XmlElement("chemical-shift-y")> Public Property chemicalShiftY As Double

    End Class
End Namespace