Imports System.Xml.Serialization

Namespace Spectra.Xml

    Public Class Meta : Inherits ms1_scan

        <XmlAttribute>
        Public Property id As String

        Public Overrides Function ToString() As String
            Return id
        End Function

    End Class
End Namespace