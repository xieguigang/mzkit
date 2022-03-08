Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq

Public Class InputDataVisual

    Dim fields As Dictionary(Of String, Type)

    Public Sub SetAxis(fields As Dictionary(Of String, Type))
        For Each item In fields
            ListBox1.Items.Add(item.Key)
            ListBox2.Items.Add(item.Key)
        Next

        ListBox1.SelectedIndex = 0
        ListBox2.SelectedIndex = 0

        Me.ComboBox1.SelectedIndex = 0
        Me.fields = fields
    End Sub

    Public Function GetX() As String
        Return ListBox1.SelectedItem.ToString
    End Function

    Public Function GetY() As String
        Return ListBox2.SelectedItem.ToString
    End Function

    Public Sub DoPlot(x As Array, y As Array)
        Dim grid = MyApplication.host.mzkitTool.DataGridView1
        Dim plot As Image

        Call grid.Rows.Clear()
        Call grid.Columns.Clear()

        Call grid.Columns.Add("X", "X")
        Call grid.Columns.Add("Y", "Y")

        Select Case ComboBox1.SelectedItem.ToString
            Case "Scatter"
                plot = Scatter.Plot({Scatter.FromPoints(x.AsObjectEnumerator.Select(Function(xi, i) New PointF(xi, y(i))))}).AsGDIImage
            Case "Line
BarPlot
BoxPlot
ViolinPlot"
        End Select

        MyApplication.host.mzkitTool.PictureBox1.BackgroundImage = plot
        MyApplication.host.ShowMzkitToolkit()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub
End Class