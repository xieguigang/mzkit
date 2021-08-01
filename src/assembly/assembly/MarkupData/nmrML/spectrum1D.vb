Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace MarkupData.nmrML

    Public Class spectrum1D

        Public Property spectrumDataArray As fidData
        Public Property xAxis As xAxis

        Public Function ParseMatrix() As LibraryMatrix

        End Function

    End Class

    Public Class xAxis
        <XmlAttribute> Public Property unitAccession As String
        <XmlAttribute> Public Property unitName As String
        <XmlAttribute> Public Property unitCvRef As String
        <XmlAttribute> Public Property startValue As Double
        <XmlAttribute> Public Property endValue As Double
    End Class

    Public Class spectrumList

        <XmlElement> Public Property spectrum1D As spectrum1D()

    End Class
End Namespace