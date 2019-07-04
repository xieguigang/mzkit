Imports ProteoWizard.d

Module batchConvertor

    Sub Main()
        Dim cli As New ProteoWizardCLI(App.GetVariable("bin"))

        For Each dir As String In App.CommandLine.Name.ListDirectory
            For Each file As String In dir.EnumerateFiles("*.raw")
                If Not file.ChangeSuffix("mzXML").FileLength > 1024 Then
                    Call cli.Convert2mzML(file, dir, ProteoWizardCLI.OutFileTypes.mzXML)
                End If
            Next
        Next
    End Sub
End Module
