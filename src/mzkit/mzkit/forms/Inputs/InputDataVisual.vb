Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Class InputDataVisual

    Dim fields As Dictionary(Of String, Type)

    Public Sub SetAxis(fields As Dictionary(Of String, Type))
        For Each item In fields
            ListBox1.Items.Add(item.Key)
            CheckedListBox1.Items.Add(item.Key)
        Next

        ListBox1.SelectedIndex = 0
        CheckedListBox1.SelectedIndex = 0

        Me.ComboBox1.SelectedIndex = 0
        Me.fields = fields
    End Sub

    Public Function GetX() As String
        Return ListBox1.SelectedItem.ToString
    End Function

    Public Iterator Function GetY() As IEnumerable(Of String)
        For Each check In CheckedListBox1.CheckedItems
            Yield check.ToString
        Next
    End Function

    Private Iterator Function getSerials(x As Array, getVector As Func(Of String, Array)) As IEnumerable(Of SerialData)
        Dim colors As String() = Globals.Settings.viewer.colorSet
        Dim idx As i32 = Scan0

        If colors.IsNullOrEmpty Then
            colors = Designer.GetColors("paper", 12).Select(Function(c) c.ToHtmlColor).ToArray
        End If

        For Each name As String In GetY()
            Dim y As Array = getVector(name)
            Dim points = x.AsObjectEnumerator.Select(Function(xi, i) New PointF(xi, y(i))).OrderByDescending(Function(p) p.X).ToArray
            Dim s = Scatter.FromPoints(points, lineColor:=colors(++idx))

            Yield s
        Next
    End Function

    Public Sub DoPlot(x As Array, getVector As Func(Of String, Array))
        Dim grid = MyApplication.host.mzkitTool.DataGridView1
        Dim plot As Image

        Call grid.Rows.Clear()
        Call grid.Columns.Clear()

        Call grid.Columns.Add("X", "X")
        Call grid.Columns.Add("Y", "Y")

        Select Case ComboBox1.SelectedItem.ToString
            Case "Scatter"
                plot = Scatter.Plot(getSerials(x, getVector), size:="2100,1800", drawLine:=False).AsGDIImage
            Case "Line"
                plot = Scatter.Plot(getSerials(x, getVector), size:="2100,1800", drawLine:=True).AsGDIImage
            Case "BarPlot
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