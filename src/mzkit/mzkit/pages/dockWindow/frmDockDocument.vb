#Region "Microsoft.VisualBasic::2ac417d9c7924d86aa8a5fcdf5d61315, mzkit\src\mzkit\mzkit\pages\dockWindow\frmDockDocument.vb"

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

    '   Total Lines: 34
    '    Code Lines: 26
    ' Comment Lines: 1
    '   Blank Lines: 7
    '     File Size: 1.12 KB


    ' Class frmDockDocument
    ' 
    '     Sub: addPage, frmDockDocument_Closing, frmDockDocument_Load
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmDockDocument

    Friend pages As New List(Of Control)

    Public Sub addPage(ParamArray pageList As Control())
        For Each page As Control In pageList
            Call Globals.sharedProgressUpdater("Load [" & page.Text & "]")

            Controls.Add(page)
            pages.Add(page)
            page.Dock = DockStyle.Fill
        Next
    End Sub

    Private Sub frmDockDocument_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.DockState = DockState.Hidden
    End Sub

    Private Sub frmDockDocument_Load(sender As Object, e As EventArgs) Handles Me.Load
        Text = "BioNovoGene M/Z Data Toolkit"
        Me.Icon = My.Resources.toolkit
        Me.ShowIcon = True
        '  Me.ShowInTaskbar = True

        OpenContainingFolderToolStripMenuItem.Enabled = False
        SaveDocumentToolStripMenuItem.Enabled = False
        CopyFullPathToolStripMenuItem.Enabled = False
        CloseToolStripMenuItem.Enabled = False
    End Sub
End Class
