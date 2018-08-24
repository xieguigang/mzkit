Imports System.Data.Linq.Mapping
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports sys = System.Math

''' <summary>
''' The ms1 peak
''' </summary>
Public Class Ms1Feature : Implements INamedValue

    <Column(Name:="#ID")>
    Public Property ID As String Implements IKeyedEntity(Of String).Key
    Public Property mz As Double
    Public Property rt As Double

    Public Overrides Function ToString() As String
        Return $"{sys.Round(mz, 4)}@{rt}"
    End Function
End Class

''' <summary>
''' 质谱标准品基本注释信息
''' </summary>
Public Class MetaInfo : Inherits Ms1Feature

    Public Property name As String

    ''' <summary>
    ''' 这个ms1信息所对应的物质在数据库之中的编号信息列表
    ''' </summary>
    ''' <returns></returns>
    Public Property xref As Dictionary(Of String, String)

End Class

Public Class ms1_scan

    Public Property mz As Double
    Public Property scan_time As Double
    Public Property intensity As Double

End Class