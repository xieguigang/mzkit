Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.IonTargeted

Namespace MarkupData.mzML

    Public Class spectrumList : Inherits DataList

        <XmlElement("spectrum")>
        Public Property spectrums As spectrum()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetAllMs1() As IEnumerable(Of spectrum)
            Return spectrums.GetAllMs1
        End Function
    End Class

    Public Class precursorList : Inherits List

        <XmlElement>
        Public Property precursor As precursor()

    End Class

    Public Class scanList : Inherits List

        <XmlElement("cvParam")>
        Public Property cvParams As cvParam()
        <XmlElement("scan")>
        Public Property scans As scan()

    End Class

    Public Class scan : Inherits Params

        <XmlAttribute>
        Public Property instrumentConfigurationRef As String

    End Class

    Public Class scanWindowList : Inherits List

        <XmlElement(NameOf(scanWindow))>
        Public Property scanWindows As scanWindow()

    End Class

    Public Class scanWindow : Inherits Params

    End Class
End Namespace