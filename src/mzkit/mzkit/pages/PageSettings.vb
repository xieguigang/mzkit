#Region "Microsoft.VisualBasic::0b5ab9f4231293aa3c6505c3b706cb7a, src\mzkit\mzkit\pages\PageSettings.vb"

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

' Class PageSettings
' 
'     Sub: Button1_Click, Button2_Click, LinkLabel1_LinkClicked, PageSettings_Load, showPage
'          showStatusMessage, TreeView1_AfterSelect
' 
' /********************************************************************************/

#End Region

Imports mzkit.My

Public Class PageSettings

    Dim status As ToolStripStatusLabel

    Dim elementProfile As New ElementProfile With {.Text = "Formula Search"}
    Dim appConfig As New AppConfig With {.Text = "Mzkit Settings"}
    Dim viewer As New RawFileViewer With {.Text = "Raw File Viewer"}
    Dim pages As Control()
    Dim showPageLink As IPageSettings

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        MyApplication.host.ShowPage(MyApplication.host.mzkitTool)
    End Sub

    Private Sub PageSettings_Load(sender As Object, e As EventArgs) Handles Me.Load
        status = MyApplication.host.ToolStripStatusLabel1
        pages = {elementProfile, appConfig, viewer}

        For Each page In pages
            Panel1.Controls.Add(page)
            page.Dock = DockStyle.Fill
            Call DirectCast(CObj(page), ISaveSettings).LoadSettings()
        Next

        showPage(appConfig)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        For Each page In pages
            Call DirectCast(CObj(page), ISaveSettings).SaveSettings()
        Next

        showStatusMessage("New settings value applied and saved!")
    End Sub

    Sub showStatusMessage(message As String)
        MyApplication.host.Invoke(Sub() status.Text = message)
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
        End Select
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Call showPageLink.ShowPage()
    End Sub
End Class
