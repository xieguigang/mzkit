Imports BioNovoGene.mzkit_win32.My

Public Class InputRSymbol

    Private Sub InputRSymbol_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For Each symbol As String In MyApplication.REngine.globalEnvir.GetSymbolsNames
            Call ComboBox1.Items.Add(symbol)
        Next
    End Sub

    Public Iterator Function GetNames() As IEnumerable(Of String)
        For Each i As Integer In CheckedListBox1.CheckedIndices
            Yield CheckedListBox1.Items(i).ToString
        Next
    End Function

    Public Sub LoadFields(names As IEnumerable(Of String))
        Call CheckedListBox1.Items.Clear()

        For Each name As String In names
            Call CheckedListBox1.Items.Add(name)
            Call CheckedListBox1.SetItemChecked(CheckedListBox1.Items.Count - 1, True)
        Next
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Trim(ComboBox1.Text).StringEmpty Then
            MessageBox.Show("A symbol name is required!", "Create Symbol", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            DialogResult = DialogResult.OK
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel
    End Sub
End Class