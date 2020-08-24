Public Class PageSettings

    Dim host As frmMain
    Dim status As ToolStripStatusLabel

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        host.ShowPage(host.mzkitTool)
    End Sub

    Private Sub PageSettings_Load(sender As Object, e As EventArgs) Handles Me.Load
        host = DirectCast(ParentForm, frmMain)
        status = host.ToolStripStatusLabel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        showStatusMessage("New settings value applied and saved!")
    End Sub

    Sub showStatusMessage(message As String)
        host.Invoke(Sub() status.Text = message)
    End Sub
End Class
