Imports System.IO

Namespace IndexedCache

    Public Class XICIndex

        Public Property mz As Double()
        Public Property offset As Long()
        Public Property width As Integer
        Public Property height As Integer
        ''' <summary>
        ''' the file name of the upstream source file
        ''' </summary>
        ''' <returns></returns>
        Public Property source As String
        Public Property tolerance As String

        Public Shared Sub WriteIndexFile(cache As XICWriter, file As Stream)

        End Sub

    End Class
End Namespace