Imports System.ComponentModel

Public Class frmRScriptEdit

    Public Property scriptFile As String

    Private Sub frmRScriptEdit_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If scriptFile.StringEmpty Then
            Dim result = MessageBox.Show("Save current script file?", "File Not Saved", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
            ElseIf result = DialogResult.Cancel Then
                e.Cancel = True
            End If
        End If
    End Sub
End Class