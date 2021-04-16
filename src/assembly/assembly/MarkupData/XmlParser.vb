Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO

Namespace MarkupData

    Public Class XmlParser

        ReadOnly bin As BinaryDataReader

        Sub New(file As BinaryDataReader)
            bin = file
        End Sub

        Public Function ParseDataNode(Of T)(index As Long) As T

        End Function

    End Class
End Namespace