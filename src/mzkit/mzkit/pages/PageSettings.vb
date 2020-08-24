#Region "Microsoft.VisualBasic::6baafc2376bb1842c0fef2636d723011, src\mzkit\mzkit\pages\PageSettings.vb"

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
    '     Sub: Button1_Click, Button2_Click, PageSettings_Load, showStatusMessage
    ' 
    ' /********************************************************************************/

#End Region

Public Class PageSettings

    Dim host As frmMain
    Dim status As ToolStripStatusLabel

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        host.ShowPage(host.mzkitTool)
    End Sub

    Private Sub PageSettings_Load(sender As Object, e As EventArgs) Handles Me.Load
        host = DirectCast(ParentForm, frmMain)
        status = host.ToolStripStatusLabel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        showStatusMessage("New settings value applied and saved!")
    End Sub

    Sub showStatusMessage(message As String)
        host.Invoke(Sub() status.Text = message)
    End Sub
End Class

