#Region "Microsoft.VisualBasic::829e095c071c5a4ac89724293d4a78ec, src\mzkit\mzkit\pages\dockWindow\frmRsharp.vb"

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

    ' Class frmRsharp
    ' 
    '     Sub: frmRsharp_Closing, frmRsharp_Load
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

Public Class frmRsharp

    Friend Routput As New TextBox

    Private Sub frmRsharp_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub frmRsharp_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(Routput)

        Routput.Multiline = True
        Routput.Dock = DockStyle.Fill
        Routput.ReadOnly = True
        Routput.Font = New Font("Consolas", 10, FontStyle.Regular)
        Routput.ScrollBars = ScrollBars.Vertical
        Routput.BackColor = Color.White
        Routput.ForeColor = Color.Black

        TabText = "R# Terminal"
        Me.Icon = My.Resources.Rscript

        Me.ShowIcon = True
        '  Me.ShowInTaskbar = True
    End Sub
End Class

