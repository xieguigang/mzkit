#Region "Microsoft.VisualBasic::6829a17f5766f8c2f8a089f937832aa3, mzkit\src\mzkit\mzkit\pages\dockWindow\tools\OutputWindow.vb"

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


    ' Code Statistics:

    '   Total Lines: 61
    '    Code Lines: 43
    ' Comment Lines: 8
    '   Blank Lines: 10
    '     File Size: 1.93 KB


    '     Class OutputWindow
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: AppendMessage, AppendRoutput, comboBox_SelectedIndexChanged, OutputWindow_Load, ToolStripButton1_Click
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

            Me.ToolStripComboBox1.SelectedIndex = 0
        End Sub

        ''' <summary>
        ''' append line
        ''' </summary>
        ''' <param name="msg"></param>
        Public Sub AppendMessage(msg As String)
            Invoke(Sub()
                       textBox1.AppendText(msg & vbCrLf)
                       textBox1.SelectionStart = textBox1.TextLength
                   End Sub)
        End Sub

        ''' <summary>
        ''' append line
        ''' </summary>
        ''' <param name="msg"></param>
        Public Sub AppendRoutput(msg As String)
            Invoke(Sub()
                       textBox2.AppendText(msg & vbCrLf)
                       textBox2.SelectionStart = textBox2.TextLength
                   End Sub)
        End Sub

        Private Sub comboBox_SelectedIndexChanged() Handles ToolStripComboBox1.SelectedIndexChanged
            If ToolStripComboBox1.SelectedIndex = 0 Then
                textBox2.Hide()
                textBox1.Show()
                textBox1.Dock = DockStyle.Fill
            Else
                textBox1.Hide()
                textBox2.Show()
                textBox2.Dock = DockStyle.Fill
            End If
        End Sub

        Private Sub OutputWindow_Load(sender As Object, e As EventArgs) Handles Me.Load
            Call ApplyVsTheme(ToolStrip1)
            Call comboBox_SelectedIndexChanged()

            Me.Icon = My.Resources.logging
        End Sub

        Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
            textBox1.Clear()
            textBox2.Clear()
        End Sub
    End Class
End Namespace
