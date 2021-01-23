Public Class WorkStudio

    Public Shared Sub RunTaskScript(file As String, args As String)
        Call CommandLine.Call($"{App.HOME}/R#.exe", args)
    End Sub
End Class
