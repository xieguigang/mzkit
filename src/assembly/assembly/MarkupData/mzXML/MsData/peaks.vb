Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace MarkupData.mzXML

    Public Class peaks : Implements IBase64Container

        ''' <summary>
        ''' 1. zlib
        ''' 2. gzip
        ''' 3. none
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property compressionType As String
        <XmlAttribute> Public Property compressedLen As Integer
        <XmlAttribute> Public Property precision As Double
        <XmlAttribute> Public Property byteOrder As String
        <XmlAttribute> Public Property contentType As String

        <XmlText>
        Public Property value As String Implements IBase64Container.BinaryArray

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Function GetPrecision() As Integer Implements IBase64Container.GetPrecision
            Return precision
        End Function

        Public Function GetCompressionType() As CompressionMode Implements IBase64Container.GetCompressionType
            If charToModes.ContainsKey(compressionType) Then
                Return charToModes(compressionType)
            Else
                Throw New NotImplementedException(compressionType)
            End If
        End Function
    End Class
End Namespace