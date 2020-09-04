Imports mzkit.My

Public Class MolecularNetworking : Implements ISaveSettings, IPageSettings

    Public Sub LoadSettings() Implements ISaveSettings.LoadSettings
        If Globals.Settings.network Is Nothing Then
            Globals.Settings.network = New NetworkArguments With {.layout_iterations = 100}
        End If

        NumericUpDown1.Value = Globals.Settings.network.layout_iterations
    End Sub

    Public Sub SaveSettings() Implements ISaveSettings.SaveSettings
        Globals.Settings.network.layout_iterations = NumericUpDown1.Value
    End Sub

    Public Sub ShowPage() Implements IPageSettings.ShowPage
        Call MyApplication.host.ShowPage(MyApplication.host.mzkitMNtools)
        Call MyApplication.host.ShowMzkitToolkit()
    End Sub
End Class
