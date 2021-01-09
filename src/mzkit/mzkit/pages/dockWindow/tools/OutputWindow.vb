#Region "Microsoft.VisualBasic::4d04897dc5a1ed2d605791d99b7b8f64, src\mzkit\mzkit\pages\dockWindow\DummyOutputWindow.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    '     Class DummyOutputWindow
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: AppendMessage, comboBox_SelectedIndexChanged, DummyOutputWindow_Closing
    ' 
    ' 
    ' /********************************************************************************/

#End Region


Imports System.ComponentModel

Namespace DockSample
    Partial Public Class OutputWindow
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

            Me.ToolStripComboBox1.SelectedIndex = 0
        End Sub

        Public Sub AppendMessage(msg As String)
            Invoke(Sub() textBox1.AppendText(msg & vbCrLf))
        End Sub

        Private Sub comboBox_SelectedIndexChanged(sender As Object, e As EventArgs)
            If ToolStripComboBox1.SelectedIndex = 0 Then
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

        Private Sub OutputWindow_Load(sender As Object, e As EventArgs) Handles Me.Load
            Call ApplyVsTheme(ToolStrip1)
        End Sub
    End Class
End Namespace

