Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary

Namespace MarkupData.mzML

    <XmlType(NameOf(mzML), [Namespace]:=indexedmzML.xmlns)>
    Public Class mzML

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property version As String

        Public Property cvList As cvList
        Public Property run As run
        Public Property fileDescription As fileDescription

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadChromatogramList(path As String) As IEnumerable(Of chromatogram)
            Return MarkupData.mzML.LoadChromatogramList(path)
        End Function

        Public Overrides Function ToString() As String
            Return id
        End Function

    End Class
End Namespace