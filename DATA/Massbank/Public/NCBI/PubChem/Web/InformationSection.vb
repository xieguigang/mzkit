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
        Public Property DisplayControls As DisplayControls

        <XmlElement("Information")>
        Public Property Information As Information()

        Public Overrides Function ToString() As String
            Return $"[{TOCHeading}]  {Description}"
        End Function
    End Class

    Public Class DisplayControls
        Public Property CreateTable As CreateTable
        Public Property ShowAtMost As String
    End Class

    Public Class CreateTable
        Public Property FromInformationIn As String
        Public Property NumberOfColumns As Integer
        <XmlElement("ColumnContents")>
        Public Property ColumnContents As String()
    End Class
End Namespace