Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

''' <summary>
''' The ms1 peak
''' </summary>
Public Class ms1 : Implements INamedValue

    <Column("#ID")>
    Public Property ID As String Implements IKeyedEntity(Of String).Key
    Public Property mz As Double
    Public Property rt As Double

    Public Overrides Function ToString() As String
        Return $"{mz}@{rt}"
    End Function
End Class