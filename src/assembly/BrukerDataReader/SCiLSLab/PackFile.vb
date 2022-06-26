Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace SCiLSLab

    Public Delegate Function LineParser(Of T)(row As String(), headers As Index(Of String)) As T

    Public Class PackFile

        Public Property comment As String
        Public Property export_time As Date
        Public Property raw As String
        Public Property fullName As String
        Public Property guid As String
        Public Property type As String
        Public Property create_time As Date

        Protected Shared Iterator Function ParseTable(Of T)(file As Stream, byrefPack As PackFile, parseLine As LineParser(Of T)) As IEnumerable(Of T)

        End Function

    End Class
End Namespace