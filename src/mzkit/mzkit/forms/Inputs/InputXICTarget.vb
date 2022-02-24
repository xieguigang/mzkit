Public Class InputXICTarget

    Public ReadOnly Property XICTarget As Double
        Get
            If ComboBox1.Items.Count = 0 Then
                Return 0
            Else
                Return Val(ComboBox1.SelectedItem)
            End If
        End Get
    End Property

    Public Sub SetIons(mz As IEnumerable(Of Double))
        ComboBox1.Items.Clear()

        For Each mzi As Double In mz
            Call ComboBox1.Items.Add(mzi.ToString("F4"))
        Next
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub
End Class