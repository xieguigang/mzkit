Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Blender

    Public Class RGBConfigs

        Public Property R As NamedValue(Of Double)
        Public Property G As NamedValue(Of Double)
        Public Property B As NamedValue(Of Double)

        Public Function GetJSON() As String
            Dim json As New Dictionary(Of String, Dictionary(Of String, String))

            json!r = New Dictionary(Of String, String) From {{"m/z", R.Value}, {"annotation", R.Name}}
            json!g = New Dictionary(Of String, String) From {{"m/z", G.Value}, {"annotation", G.Name}}
            json!b = New Dictionary(Of String, String) From {{"m/z", B.Value}, {"annotation", B.Name}}

            Return json.GetJson
        End Function

    End Class
End Namespace