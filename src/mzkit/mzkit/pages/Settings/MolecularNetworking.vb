Imports mzkit.My

Public Class MolecularNetworking : Implements ISaveSettings, IPageSettings

    Public Sub LoadSettings() Implements ISaveSettings.LoadSettings
        If Globals.Settings.network Is Nothing Then
            Globals.Settings.network = New NetworkArguments With {
                .layout_iterations = 100,
                .linkWidth = New ElementRange With {.min = 1, .max = 10},
                .nodeRadius = New ElementRange With {.min = 10, .max = 100}
            }
        End If

        NumericUpDown1.Value = Globals.Settings.network.layout_iterations

        TrackBar1.Value = Globals.Settings.network.nodeRadius.min
        TrackBar2.Value = Globals.Settings.network.nodeRadius.max

        TrackBar3.Value = Globals.Settings.network.linkWidth.min
        TrackBar4.Value = Globals.Settings.network.linkWidth.max
    End Sub

    Public Sub SaveSettings() Implements ISaveSettings.SaveSettings
        Globals.Settings.network.layout_iterations = NumericUpDown1.Value
        Globals.Settings.network.nodeRadius = New ElementRange With {
            .min = TrackBar1.Value, .max = TrackBar2.Value
        }
        Globals.Settings.network.linkWidth = New ElementRange With {
            .min = TrackBar3.Value, .max = TrackBar4.Value
        }
    End Sub

    Public Sub ShowPage() Implements IPageSettings.ShowPage
        Call MyApplication.host.ShowPage(MyApplication.host.mzkitMNtools)
        Call MyApplication.host.ShowMzkitToolkit()
    End Sub
End Class
