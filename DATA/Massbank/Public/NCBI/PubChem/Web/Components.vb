Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace NCBI.PubChem

    Public Class Reference

        Public Property ReferenceNumber As String
        Public Property SourceName As String
        Public Property SourceID As String
        Public Property Name As String
        Public Property URL As String

        Public Overrides Function ToString() As String
            Return $"{Name} ({URL})"
        End Function
    End Class

    Public Class Section

        Public Property TOCHeading As String
        Public Property Description As String
        Public Property HintGroupSubsectionsByReference As Boolean
        Public Property HintEmbeddedHTML As Boolean
        <XmlElement("Information")>
        Public Property Information As Information()
        <XmlElement("Section")>
        Public Property Sections As Section()

        Public Overrides Function ToString() As String
            Return $"[{TOCHeading}]  {Description}"
        End Function
    End Class

    Public Class Information

        Public Property ReferenceNumber As String
        Public Property Name As String
        Public Property Description As String

        Public Property BoolValue As Boolean
        Public Property NumValue As String
        Public Property StringValue As String
        Public Property ValueUnit As String
        <XmlElement("StringValueList")>
        Public Property StringValueList As String()
        Public Property DateValue As String

        Public Property URL As String
        Public Property ExternalDataURL As String
        Public Property ExternalDataMimeType As String

        Public ReadOnly Property InfoType As Type
            Get
                If Not NumValue.StringEmpty Then
                    Return GetType(Double)
                ElseIf Not StringValue.StringEmpty Then
                    Return GetType(String)
                ElseIf Not StringValueList.IsNullOrEmpty Then
                    Return GetType(String())
                ElseIf Not DateValue.StringEmpty Then
                    Return GetType(Date)
                Else
                    Return GetType(Boolean)
                End If
            End Get
        End Property

        Public ReadOnly Property InfoValue As Object
            Get
                If Not NumValue.StringEmpty Then
                    Return Val(NumValue)
                ElseIf Not StringValue.StringEmpty Then
                    Return StringValue
                ElseIf Not StringValueList.IsNullOrEmpty Then
                    Return StringValueList
                ElseIf Not DateValue.StringEmpty Then
                    Return Date.Parse(DateValue)
                Else
                    Return BoolValue
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            If InfoType Is GetType(String()) Then
                Return $"Dim {Name} As {InfoType.FullName} = {StringValueList.GetJson}"
            Else
                Return $"Dim {Name} As {InfoType.FullName} = {InfoValue}"
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Narrowing Operator CType(info As Information) As NamedValue(Of Object)
            Return New NamedValue(Of Object)(info.Name, info.InfoValue, info.Description)
        End Operator
    End Class

    Public Class Table

        Public Property ExternalTableName As String

        <XmlElement("ColumnName")>
        Public Property ColumnNames As String()
        <XmlElement("Row")>
        Public Property Rows As Row()

    End Class

    Public Class Row

        <XmlElement("Cell")>
        Public Property Cells As Information()

    End Class
End Namespace