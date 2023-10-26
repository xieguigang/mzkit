Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text.Parser

Friend NotInheritable Class PropertyFileReader

    Private Sub New()
    End Sub

    Public Shared Iterator Function ReadData(file As StreamReader) As IEnumerable(Of NamedValue(Of String()))
        For Each block As String() In FormattedParser.FlagSplit(file, AddressOf CheckFlag)
            Dim si As String = block.JoinBy(vbCrLf)
            si = si.TrimStart("#"c, "$"c)
            Dim split = si.GetTagValue("=", trim:=True)
            block = split.Value.Trim.LineTokens

            Yield New NamedValue(Of String()) With {
                .Name = split.Name,
                .Value = block
            }
        Next

        Call file.Close()
        Call file.Dispose()
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
