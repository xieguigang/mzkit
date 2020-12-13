Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace LipidMaps

    Public Module MetaTable

        Friend ReadOnly properties As Dictionary(Of String, PropertyInfo) = DataFramework.Schema(Of MetaData)(PropertyAccess.Writeable, True)

        <Extension>
        Public Iterator Function ProjectVectors(lipidmaps As MetaData()) As IEnumerable(Of NamedValue(Of Array))
            For Each [property] As PropertyInfo In properties.Values
                Dim values As Array = Array.CreateInstance([property].PropertyType, lipidmaps.Length)

                For i As Integer = 0 To lipidmaps.Length - 1
                    values.SetValue([property].GetValue(lipidmaps(i)), i)
                Next

                Yield New NamedValue(Of Array) With {
                    .Name = [property].Name,
                    .Value = values
                }
            Next
        End Function

    End Module
End Namespace