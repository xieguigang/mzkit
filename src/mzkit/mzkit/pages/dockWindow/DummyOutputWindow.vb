
Imports System.ComponentModel

Namespace DockSample
    Partial Public Class DummyOutputWindow
        Inherits ToolWindow

        Public Sub New()
            InitializeComponent()

            DoubleBuffered = True

            Me.textBox1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.textBox1.Location = New System.Drawing.Point(0, 23)
            Me.textBox1.Multiline = True
            Me.textBox1.Name = "textBox1"
            Me.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both
            Me.textBox1.Size = New System.Drawing.Size(653, 349)
            Me.textBox1.TabIndex = 2
            Me.textBox1.ReadOnly = True

            Me.textBox1.WordWrap = True


            Me.textBox2.Dock = System.Windows.Forms.DockStyle.Fill
            Me.textBox2.Location = New System.Drawing.Point(0, 23)
            Me.textBox2.Multiline = True
            Me.textBox2.Name = "textBox1"
            Me.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Both
            Me.textBox2.Size = New System.Drawing.Size(653, 349)
            Me.textBox2.TabIndex = 2
            Me.textBox2.ReadOnly = True

            Me.textBox2.WordWrap = True

            Me.comboBox.SelectedIndex = 0
        End Sub

        Public Sub AppendMessage(msg As String)
            Invoke(Sub() textBox1.AppendText(msg & vbCrLf))
        End Sub

        Private Sub comboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles comboBox.SelectedIndexChanged
            If comboBox.SelectedIndex = 0 Then
                textBox1.Show()
                textBox2.Hide()
            Else
                textBox1.Hide()
                textBox2.Show()
            End If
        End Sub

        Private Sub DummyOutputWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
            e.Cancel = True
            Call Me.Hide()
        End Sub
    End Class
End Namespace
