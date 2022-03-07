Public Class InputAction

    Public Sub SetFields(names As IEnumerable(Of String))
        For Each name As String In names
            Call ComboBox1.Items.Add(name)
        Next

        For Each name As String In Actions.allActions
            Call ComboBox2.Items.Add(name)
        Next
    End Sub

    Public ReadOnly Property getTargetName As String
        Get
            Return ComboBox1.Text
        End Get
    End Property
    Public ReadOnly Property getActionName As String
        Get
            Return ComboBox2.Text
        End Get
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ComboBox1.SelectedIndex = -1 Then
            MessageBox.Show("no data target is selected!", "Run Action", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        ElseIf ComboBox2.SelectedIndex = -1 Then
            MessageBox.Show("no data action is selected!", "Run Action", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Me.DialogResult = DialogResult.OK
    End Sub
End Class