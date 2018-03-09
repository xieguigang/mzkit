Imports System.Data.Linq.Mapping
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports sys = System.Math

''' <summary>
''' The ms1 peak
''' </summary>
Public Class ms1 : Implements INamedValue

    <Column(Name:="#ID")>
    Public Property ID As String Implements IKeyedEntity(Of String).Key
    Public Property mz As Double
    Public Property rt As Double

    Public Overrides Function ToString() As String
        Return $"{sys.Round(mz, 4)}@{rt}"
    End Function
End Class

Public Class ms1_scan

    Public Property mz As Double
    Public Property scan_time As Double
    Public Property intensity As Double

End Class