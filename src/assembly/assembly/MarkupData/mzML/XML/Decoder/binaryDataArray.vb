Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace MarkupData.mzML

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

        Public Function GetCompressionType() As CompressionMode Implements IBase64Container.GetCompressionType
            If Not cvParams.KeyItem("zlib compression") Is Nothing Then
                Return CompressionMode.zlib
            ElseIf Not cvParams.KeyItem("no compression") Is Nothing Then
                Return CompressionMode.none
            ElseIf Not cvParams.KeyItem("gzip compression") Is Nothing Then
                Return CompressionMode.gzip
            Else
                Throw New NotImplementedException
            End If
        End Function
    End Class
End Namespace