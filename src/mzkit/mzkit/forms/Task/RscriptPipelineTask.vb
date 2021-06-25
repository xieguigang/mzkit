Imports Task.CLI

Public Class RscriptPipelineTask

    Friend Shared ReadOnly Rscript As Rscript = Rscript.FromEnvironment(App.HOME)

    Public Shared Function GetRScript(filename As String) As String
        Dim filepath As String = $"{App.HOME}/Rstudio/Pipeline/{filename}"

        ' product version
        If filepath.FileLength > 10 Then
            Return filepath
        End If

        ' development version
        filepath = $"{App.HOME}/../../src/mzkit/Pipeline/{filename}".GetFullPath

        Return filepath
    End Function

End Class
