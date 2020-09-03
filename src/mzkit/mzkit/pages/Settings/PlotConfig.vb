Public Class PlotConfig : Implements ISaveSettings, IPageSettings

    Dim WithEvents colorPicker As New ThemeColorPicker

    Public Sub LoadSettings() Implements ISaveSettings.LoadSettings

    End Sub

    Public Sub SaveSettings() Implements ISaveSettings.SaveSettings

    End Sub

    Public Sub ShowPage() Implements IPageSettings.ShowPage

    End Sub

    Private Sub PlotConfig_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(colorPicker)

        colorPicker.Location = New Point(50, 50)
        ' PictureBox1.BorderStyle = BorderStyle.FixedSingle

        AddHandler colorPicker.ColorSelected, AddressOf selectColor
    End Sub

    Private Sub selectColor(sender As Object, e As ColorSelectedArg) Handles colorPicker.ColorSelected
        Dim color As Integer() = {e.R, e.G, e.B}

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
        If ListBox1.Items.Count = 0 Then Return

        Dim index = ListBox1.IndexFromPoint(e.X, e.Y)
        Dim s = ListBox1.Items(index).ToString()
        Dim dde1 As DragDropEffects = DoDragDrop(s, DragDropEffects.Move)

        If (dde1 = DragDropEffects.Move) Then
            ListBox1.Items.RemoveAt(ListBox1.IndexFromPoint(e.X, e.Y))
        End If
    End Sub

    Private Sub ListBox1_DragEnter(sender As Object, e As DragEventArgs) Handles ListBox1.DragEnter
        If (e.Data.GetDataPresent(DataFormats.Text)) Then
            e.Effect = DragDropEffects.Move
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub ListBox1_DragDrop(sender As Object, e As DragEventArgs) Handles ListBox1.DragDrop
        If (e.Data.GetDataPresent(DataFormats.StringFormat)) Then
            Dim str = e.Data.GetData(DataFormats.StringFormat)
            Dim indexPos = ListBox1.IndexFromPoint(ListBox1.PointToClient(New Point(e.X, e.Y)))
            If (indexPos > -1) Then
                ListBox1.Items.Insert(indexPos, str)
            Else
                ListBox1.Items.Add(str)

            End If
        End If
    End Sub
End Class
