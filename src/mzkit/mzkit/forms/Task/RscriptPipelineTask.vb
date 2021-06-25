Public Class RscriptPipelineTask

    Public Shared Function GetRScript(filename As String) As String
        Dim filepath As String = $"{App.HOME}/Rstudio/Pipeline/{filename}"

        If filepath.FileLength > 10 Then
            Return filepath
        End If

        filepath = $"{App.HOME}/../../src/mzkit/Pipeline/{filename}".GetFullPath

        Return filepath
    End Function

End Class
