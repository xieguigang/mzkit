Imports mzkit.My
Imports RibbonLib.Controls.Events
Imports RibbonLib.Interop

Public Class frmTargetedQuantification

    Private Sub frmTargetedQuantification_Load(sender As Object, e As EventArgs) Handles Me.Load
        MyApplication.ribbon.TargetedContex.ContextAvailable = ContextAvailability.Active
        AddHandler MyApplication.ribbon.ImportsLinear.ExecuteEvent, AddressOf loadLinearRaw

        TabText = "Targeted Quantification"
    End Sub

    Private Sub frmTargetedQuantification_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        RemoveHandler MyApplication.ribbon.ImportsLinear.ExecuteEvent, AddressOf loadLinearRaw
    End Sub

    Sub loadLinearRaw(sender As Object, e As ExecuteEventArgs)
        Using importsFile As New OpenFileDialog With {
            .Filter = "LC-MSMS / GC-MS Targeted(*.mzML)|*.mzML|GC-MS Targeted(*.cdf)|*.cdf",
            .Multiselect = True,
            .Title = "Select linears"
        }

            If importsFile.ShowDialog = DialogResult.OK Then
                Dim files = importsFile.FileNames _
                    .OrderBy(Function(path)
                                 Return path.BaseName.Match("\d+").ParseInteger
                             End Function) _
                    .ToArray

                DataGridView1.Rows.Clear()
                DataGridView1.Columns.Clear()

                DataGridView1.Columns.Add(New DataGridViewLinkColumn With {.HeaderText = "Features"})

                For Each file As String In files.Select(AddressOf BaseName)
                    DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = file})
                Next


            End If
        End Using
    End Sub
End Class