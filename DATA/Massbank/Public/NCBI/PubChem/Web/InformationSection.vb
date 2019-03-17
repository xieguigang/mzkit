Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization

Namespace NCBI.PubChem

    Public MustInherit Class InformationSection

        Dim sectionTable As Dictionary(Of String, Section)

        <XmlElement("Section")>
        Public Property Sections As Section()
            Get
                Return sectionTable.Values.ToArray
            End Get
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Set(value As Section())
                If value.IsNullOrEmpty Then
                    sectionTable = New Dictionary(Of String, Section)
                Else
                    sectionTable = value _
                        .ToDictionary(Function(sec)
                                          Return sec.TOCHeading
                                      End Function)
                End If
            End Set
        End Property

        Default Public ReadOnly Property Section(name As String) As Section
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return sectionTable.TryGetValue(name)
            End Get
        End Property
    End Class

    Public Class Section : Inherits InformationSection

        Public Property TOCHeading As String
        Public Property Description As String
        Public Property HintGroupSubsectionsByReference As Boolean
        Public Property HintEmbeddedHTML As Boolean
        Public Property HintShowAtMost As String
        Public Property HintSortByLength As Boolean

        <XmlElement("Information")>
        Public Property Information As Information()

        Public Overrides Function ToString() As String
            Return $"[{TOCHeading}]  {Description}"
        End Function
    End Class
End Namespace