
Imports System.Xml.Serialization

Namespace Spectra

    Public Class MzAnnotation : Implements IMzAnnotation

        <XmlAttribute> Public Property productMz As Double Implements IMzAnnotation.mz
        <XmlAttribute> Public Property annotation As String Implements IMzAnnotation.annotation

        Public Overrides Function ToString() As String
            Return $"{productMz.ToString("F4")} [{annotation}]"
        End Function

    End Class

    Public Interface IMzAnnotation

        Property mz As Double
        Property annotation As String

    End Interface
End Namespace