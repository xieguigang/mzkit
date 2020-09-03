Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports mzkit.My

Public Class PlotConfig : Implements ISaveSettings, IPageSettings

    Dim WithEvents colorPicker As New ThemeColorPicker

    Public Sub LoadSettings() Implements ISaveSettings.LoadSettings
        If Globals.Settings.viewer Is Nothing Then
            Globals.Settings.viewer = New RawFileViewerSettings
        End If
        If Globals.Settings.viewer.colorSet.IsNullOrEmpty Then
            Globals.Settings.viewer.colorSet = Designer.GetColors("scibasic.category31()").Select(Function(a) a.ToHtmlColor).ToArray
        End If

        ListBox1.Items.Clear()

        For Each color As String In Globals.Settings.viewer.colorSet
            ListBox1.Items.Add(color)
        Next
    End Sub

    Public Sub SaveSettings() Implements ISaveSettings.SaveSettings
        Dim colorSet As New List(Of String)

        For i As Integer = 0 To ListBox1.Items.Count - 1
            colorSet.Add(ListBox1.Items(i).ToString)
        Next

        Globals.Settings.viewer.colorSet = colorSet.ToArray
    End Sub

    Public Sub ShowPage() Implements IPageSettings.ShowPage
        Call MyApplication.host.ShowPage(MyApplication.host.mzkitTool)
        Call MyApplication.host.ShowMzkitToolkit()
    End Sub

    Private Sub PlotConfig_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(colorPicker)

        colorPicker.Location = New Point(50, 50)
        ' PictureBox1.BorderStyle = BorderStyle.FixedSingle

        AddHandler colorPicker.ColorSelected, AddressOf selectColor
    End Sub

    Private Sub selectColor(sender As Object, e As ColorSelectedArg) Handles colorPicker.ColorSelected
        ' Dim color As Integer() = {e.R, e.G, e.B}

        PictureBox1.BackColor = e.Color

        'If PictureBox1.BorderStyle = BorderStyle.FixedSingle Then
        '    Globals.Settings.ui.background = color
        '    PictureBox1.BackColor = e.Color
        'ElseIf PictureBox2.BorderStyle = BorderStyle.FixedSingle Then
        '    Globals.Settings.ui.highlight = color
        '    PictureBox2.BackColor = e.Color
        'ElseIf PictureBox3.BorderStyle = BorderStyle.FixedSingle Then
        '    Globals.Settings.ui.text = color
        '    PictureBox3.BackColor = e.Color
        'End If

        'Globals.Settings.ui.setColors(MyApplication.host.Ribbon1)
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged

    End Sub

    Private Sub ListBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles ListBox1.MouseDown
        If Not (ListBox1.SelectedItem Is Nothing) Then
            ListBox1.DoDragDrop(ListBox1.SelectedItem, DragDropEffects.Move)
        End If
    End Sub

    Private Sub ListBox1_DragEnter(sender As Object, e As DragEventArgs) Handles ListBox1.DragEnter
        e.Effect = DragDropEffects.Move
    End Sub

    Private Sub ListBox1_DragDrop(sender As Object, e As DragEventArgs) Handles ListBox1.DragDrop
        Dim Point = ListBox1.PointToClient(New Point(e.X, e.Y))
        Dim Index = ListBox1.IndexFromPoint(Point)
        If (Index < 0) Then Index = ListBox1.Items.Count - 1
        Dim Data = e.Data.GetData(GetType(String))
        ListBox1.Items.Remove(Data)
        ListBox1.Items.Insert(Index, Data)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not PictureBox1.BackColor.IsEmpty Then
            ListBox1.Items.Add(PictureBox1.BackColor.ToHtmlColor)
        End If
    End Sub
End Class
