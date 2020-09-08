Imports mzkit.My

Public Class RawFileViewer : Implements ISaveSettings, IPageSettings

    Public Sub LoadSettings() Implements ISaveSettings.LoadSettings
        If Globals.Settings.viewer Is Nothing Then
            Globals.Settings.viewer = New RawFileViewerSettings
        End If

        NumericUpDown1.Value = Globals.Settings.viewer.XIC_ppm

        If Globals.Settings.viewer.method = TrimmingMethods.RelativeIntensity Then
            NumericUpDown2.Value = Globals.Settings.viewer.intoCutoff
        Else
            NumericUpDown2.Value = Globals.Settings.viewer.quantile
        End If

        ComboBox1.SelectedIndex = Globals.Settings.viewer.method
    End Sub

    Public Sub SaveSettings() Implements ISaveSettings.SaveSettings
        Globals.Settings.viewer.XIC_ppm = Val(NumericUpDown1.Value)
        Globals.Settings.viewer.method = ComboBox1.SelectedIndex

        If Globals.Settings.viewer.method = TrimmingMethods.RelativeIntensity Then
            Globals.Settings.viewer.intoCutoff = NumericUpDown2.Value
        Else
            Globals.Settings.viewer.quantile = NumericUpDown2.Value
        End If
    End Sub

    Public Sub ShowPage() Implements IPageSettings.ShowPage
        Call MyApplication.host.ShowPage(MyApplication.host.mzkitTool)
        Call MyApplication.host.ShowMzkitToolkit()
    End Sub
End Class
