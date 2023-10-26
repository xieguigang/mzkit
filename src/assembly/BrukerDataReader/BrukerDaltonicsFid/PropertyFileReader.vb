Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text.Parser

Public Class PropertyFileReader

    Public Function ReadData(file As StreamReader) As NamedValue(Of String())
        For Each block As String() In FormattedParser.FlagSplit(file, AddressOf CheckFlag)

        Next
    End Function

    ''' <summary>
    ''' ##$var=value
    ''' </summary>
    ''' <param name="si"></param>
    ''' <returns></returns>
    Private Shared Function CheckFlag(si As String) As Boolean
        If si Is Nothing OrElse si.Length = 0 Then
            Return False
        Else
            Return si.StartsWith("##")
        End If
    End Function
End Class
