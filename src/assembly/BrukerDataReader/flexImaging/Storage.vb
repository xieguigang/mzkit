Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.My.JavaScript
Imports any = Microsoft.VisualBasic.Scripting


''' <summary>
''' Storage.mcf_idx
''' </summary>
Public Module Storage

    Public Iterator Function GetMetaData(mcf_file As String) As IEnumerable(Of NamedValue(Of String))
        Dim parser As New IndexParser(mcf_file)
        Dim metadataId = parser.LoadTable("MetadataId").ToArray
        Dim strs = parser.LoadTable("MetaDataString").CreateValueIndex
        Dim ints = parser.LoadTable("MetaDataInt").CreateValueIndex
        Dim dbls = parser.LoadTable("MetaDataDouble").CreateValueIndex
        Dim bytes = parser.LoadTable("MetaDataBlob").CreateValueIndex

        For Each meta As JavaScriptObject In metadataId
            Dim id As String = meta("metadataId").ToString
            Dim name As String = meta("permanentName").ToString
            Dim displayName As String = meta("displayName").ToString
            Dim type As String = "any"
            Dim value As String = ""

            If strs.ContainsKey(id) Then
                type = "chr"
                value = strs(id)
            ElseIf ints.ContainsKey(id) Then
                type = "int"
                value = ints(id)
            ElseIf dbls.ContainsKey(id) Then
                type = "num"
                value = dbls(id)
            ElseIf bytes.ContainsKey(id) Then
                type = "raw"
                value = bytes(id)
            End If

            Yield New NamedValue(Of String) With {
                .Name = name,
                .Value = value,
                .Description = $"{type}|{displayName}"
            }
        Next
    End Function

    <Extension>
    Private Function CreateValueIndex(data As IEnumerable(Of JavaScriptObject)) As Dictionary(Of String, String)
        Return data _
            .GroupBy(Function(a) a("MetaDataId").ToString) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a _
                                  .Select(Function(i) any.ToString(i("Value"))) _
                                  .JoinBy("; ")
                          End Function)
    End Function
End Module
