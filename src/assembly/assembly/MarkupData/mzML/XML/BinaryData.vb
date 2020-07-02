
Imports System.Xml.Serialization

Namespace MarkupData.mzML

    Public Class BinaryData : Inherits Params

        <XmlAttribute> Public Property index As Integer
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property defaultArrayLength As String

        Public Property binaryDataArrayList As binaryDataArrayList

    End Class

    Public Class binaryDataArrayList : Inherits List

        <XmlElement(NameOf(binaryDataArray))>
        Public Property list As binaryDataArray()

    End Class

    Public Class binaryDataArray : Implements IBase64Container

        Public Property encodedLength As Integer

        <XmlElement(NameOf(cvParam))>
        Public Property cvParams As cvParam()
        Public Property binary As String Implements IBase64Container.BinaryArray

        Public Overrides Function ToString() As String
            Return binary
        End Function

        Public Function GetPrecision() As Integer Implements IBase64Container.GetPrecision
            If Not cvParams.KeyItem("64-bit float") Is Nothing Then
                Return 64
            ElseIf Not cvParams.KeyItem("32-bit float") Is Nothing Then
                Return 32
            Else
                Throw New NotImplementedException
            End If
        End Function

        Public Function GetCompressionType() As String Implements IBase64Container.GetCompressionType
            If Not cvParams.KeyItem("zlib compression") Is Nothing Then
                Return "zlib"
            Else
                Throw New NotImplementedException
            End If
        End Function
    End Class
End Namespace