Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

''' <summary>
''' Storage.mcf_idx
''' </summary>
Public Module Storage

    Public Function GetMetaData(mcf_file As String) As NamedValue(Of String)()
        Dim parser As New mcfParser(mcf_file)
    End Function
End Module
