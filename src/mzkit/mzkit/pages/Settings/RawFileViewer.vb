Imports mzkit.My

Public Class RawFileViewer : Implements ISaveSettings, IPageSettings

    Public Sub LoadSettings() Implements ISaveSettings.LoadSettings
        If Globals.Settings.viewer Is Nothing Then
            Globals.Settings.viewer = New RawFileViewerSettings
        End If

        TextBox1.Text = Globals.Settings.viewer.XIC_ppm
    End Sub

    Public Sub SaveSettings() Implements ISaveSettings.SaveSettings
        Globals.Settings.viewer.XIC_ppm = Val(TextBox1.Text)
    End Sub

    Public Sub ShowPage() Implements IPageSettings.ShowPage
        Call MyApplication.host.ShowPage(MyApplication.host.mzkitTool)
        Call MyApplication.host.ShowMzkitToolkit()
    End Sub
End Class
