Imports ProteoWizard.d

Module batchConvertor

    Sub Main()
        Dim cli As New ProteoWizardCLI(App.GetVariable("bin"))

        For Each file As String In App.Command.EnumerateFiles("*.raw")
            Call cli.Convert2mzML(file, App.Command, ProteoWizardCLI.OutFileTypes.mzXML)
        Next
    End Sub
End Module
