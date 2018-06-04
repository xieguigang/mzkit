Imports System.Xml.Serialization

Namespace TMIC.HMDB.Spectra

    Public Class EIMS : Inherits SpectraFile
        Implements IPeakList(Of EIPeak)

        Public Property peakList As EIPeak() Implements IPeakList(Of EIPeak).peakList
    End Class

    Public Class EIPeak : Inherits MSPeak

        <XmlElement("ei-ms-id")>
        Public Property EImsId As String
    End Class
End Namespace