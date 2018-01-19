Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace DATA.SpectrumJSON

    ''' <summary>
    ''' ``m/z -> into``
    ''' </summary>
    Public Class IntensityCoordinate

        ''' <summary>
        ''' m/z
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property x As Double
        ''' <summary>
        ''' 相对强度
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property y As Double
        <XmlAttribute> Public Property fragment As Boolean

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace