Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.SpringForce
Imports mzkit.My

Public Class MolecularNetworking : Implements ISaveSettings, IPageSettings

    Public Sub LoadSettings() Implements ISaveSettings.LoadSettings
        If Globals.Settings.network Is Nothing Then
            Globals.Settings.network = New NetworkArguments With {
                .layout = New ForceDirectedArgs With {.Damping = 0.4, .Iterations = 100, .Repulsion = 10000, .Stiffness = 41.76},
                .linkWidth = New ElementRange With {.min = 1, .max = 10},
                .nodeRadius = New ElementRange With {.min = 10, .max = 100}
            }
        End If
        If Globals.Settings.network.nodeRadius Is Nothing Then
            Globals.Settings.network.nodeRadius = New ElementRange With {.min = 10, .max = 100}
        End If
        If Globals.Settings.network.linkWidth Is Nothing Then
            Globals.Settings.network.linkWidth = New ElementRange With {.min = 1, .max = 10}
        End If
        If Globals.Settings.network.layout Is Nothing Then
            Globals.Settings.network.layout = New ForceDirectedArgs With {.Damping = 0.4, .Iterations = 100, .Repulsion = 10000, .Stiffness = 41.76}
        End If

        NumericUpDown1.Value = Globals.Settings.network.layout.Iterations
        stiffness.Value = Globals.Settings.network.layout.Stiffness
        damping.Value = Globals.Settings.network.layout.Damping
        repulsion.Value = Globals.Settings.network.layout.Repulsion

        TrackBar1.Value = Globals.Settings.network.nodeRadius.min
        TrackBar2.Value = Globals.Settings.network.nodeRadius.max

        TrackBar4.Value = Globals.Settings.network.linkWidth.min
        TrackBar3.Value = Globals.Settings.network.linkWidth.max
    End Sub

    Public Sub SaveSettings() Implements ISaveSettings.SaveSettings
        Globals.Settings.network.layout = New ForceDirectedArgs With {
            .Iterations = NumericUpDown1.Value,
            .Repulsion = repulsion.Value,
            .Damping = damping.Value,
            .Stiffness = stiffness.Value
        }
        Globals.Settings.network.nodeRadius = New ElementRange With {
            .min = TrackBar1.Value, .max = TrackBar2.Value
        }
        Globals.Settings.network.linkWidth = New ElementRange With {
            .min = TrackBar4.Value, .max = TrackBar3.Value
        }
    End Sub

    Public Sub ShowPage() Implements IPageSettings.ShowPage
        Call MyApplication.host.ShowPage(MyApplication.host.mzkitMNtools)
        Call MyApplication.host.ShowMzkitToolkit()
    End Sub
End Class
