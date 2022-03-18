#Region "Microsoft.VisualBasic::9adc228512d7cd8909c8650769cf9d55, mzkit\src\mzkit\mzkit\pages\dockWindow\frmSettings.vb"

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

    '   Total Lines: 82
    '    Code Lines: 65
    ' Comment Lines: 0
    '   Blank Lines: 17
    '     File Size: 3.03 KB


    ' Class frmSettings
    ' 
    '     Sub: Button1_Click, Button2_Click, frmSettings_Closing, LinkLabel1_LinkClicked, PageSettings_Load
    '          SaveSettings, showPage, TreeView1_AfterSelect
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.mzkit_win32.My
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmSettings

    Private Sub frmSettings_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.DockState = WeifenLuo.WinFormsUI.Docking.DockState.Hidden
    End Sub

    Dim elementProfile As New ElementProfile With {.Text = "Formula Search"}
    Dim presetProfile As New PresetProfile With {.Text = "Formula Search"}
    Dim appConfig As New AppConfig With {.Text = "Mzkit Settings"}
    Dim viewer As New RawFileViewer With {.Text = "Raw File Viewer"}
    Dim plotConfig As New PlotConfig With {.Text = "XIC/TIC Plot Style"}
    Dim mnSettings As New MolecularNetworking With {.Text = "Molecular Networking"}
    Dim pages As Control()
    Dim showPageLink As IPageSettings

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DockState = DockState.Hidden
        MyApplication.host.ShowPage(MyApplication.host.mzkitTool)
    End Sub

    Private Sub PageSettings_Load(sender As Object, e As EventArgs) Handles Me.Load
        pages = {elementProfile, appConfig, viewer, plotConfig, presetProfile, mnSettings}

        For Each page In pages
            Panel1.Controls.Add(page)
            page.Dock = DockStyle.Fill
            Call DirectCast(CObj(page), ISaveSettings).LoadSettings()
        Next

        showPage(appConfig)

        Me.Text = "Application Settings"
        Me.Icon = My.Resources.settings

        CopyFullPathToolStripMenuItem.Enabled = False
        OpenContainingFolderToolStripMenuItem.Enabled = False

        Me.ShowIcon = True
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Call SaveSettings()
    End Sub

    Public Sub SaveSettings()
        For Each page In pages
            Call DirectCast(CObj(page), ISaveSettings).SaveSettings()
        Next

        Call MyApplication.host.showStatusMessage("New settings value applied and saved!")
    End Sub

    Sub showPage(page As Control)
        For Each page2 In From ctl In pages Where Not ctl Is page
            page2.Hide()
        Next

        LinkLabel1.Text = page.Text
        page.Show()
        showPageLink = DirectCast(CObj(page), IPageSettings)
    End Sub

    Private Sub TreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterSelect
        Select Case e.Node.Text
            Case "Element Profile" : showPage(elementProfile)
            Case "Mzkit App" : showPage(appConfig)
            Case "Raw File Viewer" : showPage(viewer)
            Case "XIC/TIC Plot" : showPage(plotConfig)
            Case "Formula Search" : showPage(presetProfile)
            Case "Molecular Networking" : showPage(mnSettings)
        End Select
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Call showPageLink.ShowPage()
    End Sub
End Class
