Imports Microsoft.VisualBasic.ComponentModel.Collection

Public Class InputFeatureFilter

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Public Sub AddTypes(types As Dictionary(Of String, Boolean))
        CheckedListBox1.Items.Clear()

        For Each item In types
            Call CheckedListBox1.Items.Add(item.Key)
            Call CheckedListBox1.SetItemChecked(CheckedListBox1.Items.Count - 1, item.Value)
        Next
    End Sub

    Public Function GetTypes() As Index(Of String)
        Dim types As New List(Of String)

        For Each item In CheckedListBox1.CheckedItems
            types.Add(item.ToString)
        Next

        Return types.Indexing
    End Function

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub

    Private Sub txtPPM_TextChanged(sender As Object, e As EventArgs) Handles txtPPM.TextChanged

    End Sub
End Class