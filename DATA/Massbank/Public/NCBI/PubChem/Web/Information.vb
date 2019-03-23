Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace NCBI.PubChem

    Public Class Value
        <XmlElement("StringWithMarkup")>
        Public Property StringWithMarkup As StringWithMarkup()
        Public Property ExternalDataURL As String
        Public Property MimeType As String
        Public Property Number As String
        Public Property Unit As String
        Public Property DateISO8601 As String
        Public Property [Boolean] As Boolean
    End Class

    Public Class StringWithMarkup
        Public Property [String] As String
        <XmlElement("Markup")>
        Public Property Markups As Markup()
    End Class

    Public Class Markup
        Public Property Start As Integer
        Public Property Length As Integer
        Public Property URL As String
        Public Property Type As String
        Public Property Extra As String
    End Class

    Public Class Information

        Public Property ReferenceNumber As String
        Public Property Name As String
        Public Property Description As String

        Public Property Value As Value
        <XmlElement("StringValueList")>
        Public Property Table As Table
        Public Property URL As String
        Public Property ExternalDataURL As String
        Public Property ExternalDataMimeType As String

        Public ReadOnly Property InfoType As Type
            Get
                If Not Value.Number.StringEmpty Then
                    Return GetType(Double)
                ElseIf Not Value.StringWithMarkup.IsNullOrEmpty AndAlso Value.StringWithMarkup.Length = 1 Then
                    Return GetType(String)
                ElseIf Not Value.StringWithMarkup.IsNullOrEmpty Then
                    Return GetType(String())
                ElseIf Not Value.DateISO8601.StringEmpty Then
                    Return GetType(Date)
                Else
                    Return GetType(Boolean)
                End If
            End Get
        End Property

        Public ReadOnly Property InfoValue As Object
            Get
                Select Case InfoType
                    Case GetType(Double)
                        Return Val(Value.Number)
                    Case GetType(String)
                        Return Value.StringWithMarkup.First.String
                    Case GetType(String())
                        Return Value.StringWithMarkup.Select(Function(v) v.String).ToArray
                    Case GetType(Date)
                        Return Date.Parse(Value.DateISO8601)
                    Case Else
                        Return Value.Boolean
                End Select
            End Get
        End Property

        Public Overrides Function ToString() As String
            If InfoType Is GetType(String()) Then
                Return $"Dim {Name} As {InfoType.FullName} = {DirectCast(InfoValue, String()).GetJson}"
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

        Public Shared Function ToDictionary(table As Table) As Dictionary(Of NamedValue(Of String))
            If table Is Nothing OrElse table.ColumnNames.IsNullOrEmpty Then
                Call "Empty table data!".Warning
                ' return a empty dictionary table
                Return New Dictionary(Of NamedValue(Of String))
            ElseIf table.ColumnNames.Length > 2 Then
                Call $"Target table is not a key-value pair! (columns={table.ColumnNames.Length} > 2)".Warning
            End If

            Return table.Rows _
                .Select(Function(r)
                            Return New NamedValue(Of String) With {
                                .Name = r.Cells(0).Value.StringWithMarkup.First.String,
                                .Value = Scripting.ToString(r.Cells(1).InfoValue),
                                .Description = r.Cells(1).Value.Unit
                            }
                        End Function) _
                .ToDictionary
        End Function

        Public Overrides Function ToString() As String
            If ColumnNames.IsNullOrEmpty Then
                Return ExternalTableName
            Else
                Return ColumnNames.GetJson
            End If
        End Function

    End Class

    Public Class Row

        <XmlElement("Cell")>
        Public Property Cells As Information()

    End Class

    Public Class Reference

        Public Property ReferenceNumber As String
        Public Property SourceName As String
        Public Property SourceID As String
        Public Property Name As String
        Public Property URL As String
        Public Property Description As String

        Public Overrides Function ToString() As String
            Return $"{Name} ({URL})"
        End Function
    End Class
End Namespace