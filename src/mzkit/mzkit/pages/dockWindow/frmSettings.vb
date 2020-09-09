#Region "Microsoft.VisualBasic::f97ca52c4dbf4fad3f0388a0a3288aa3, src\mzkit\mzkit\pages\dockWindow\frmSettings.vb"

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

    ' Class frmSettings
    ' 
    '     Sub: frmSettings_Closing, frmSettings_Load
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

Public Class frmSettings

    Friend mzkitSettings As New PageSettings With {.Text = "Settings"}

    Private Sub frmSettings_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(mzkitSettings)

        mzkitSettings.Location = New Point
        mzkitSettings.Dock = DockStyle.Fill

        Me.Text = "Application Settings"
        Me.Icon = My.Resources.settings

        Me.ShowIcon = True
        '  Me.ShowInTaskbar = True
    End Sub

    Private Sub frmSettings_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.DockState = WeifenLuo.WinFormsUI.Docking.DockState.Hidden
    End Sub
End Class
