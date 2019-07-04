Imports ProteoWizard.d

Module batchConvertor

    Sub Main()
        Dim cli As New ProteoWizardCLI(App.GetVariable("bin"))

        For Each file As String In App.CommandLine.Name.EnumerateFiles("*.raw")
            Call cli.Convert2mzML(file, App.CommandLine.Name, ProteoWizardCLI.OutFileTypes.mzXML)
        Next
    End Sub
End Module
