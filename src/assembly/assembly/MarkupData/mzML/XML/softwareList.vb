Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Namespace MarkupData.mzML

    Public Class softwareList : Inherits List

        <XmlElement("software")>
        Public Property softwares As software()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ToArray() As NamedValue(Of String)()
            Return softwares _
                .SafeQuery _
                .Select(Function(si)
                            Return New NamedValue(Of String)(si.cvParams.First.name, si.version, si.id)
                        End Function) _
                .ToArray
        End Function

        Public Overrides Function ToString() As String
            Return softwares.JoinBy("; ")
        End Function

    End Class

    Public Class software : Inherits Params

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property version As String

        Public Overrides Function ToString() As String
            Return $"{cvParams.First.name}({version})"
        End Function

    End Class
End Namespace