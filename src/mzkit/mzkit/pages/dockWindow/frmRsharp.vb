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
Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.Language.UnixBash
Imports mzkit.SmileWei.EmbeddedApp

Public Class frmRsharp

    Friend WithEvents AppContainer1 As New SmileWei.EmbeddedApp.AppContainer

    Private Sub frmRsharp_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub frmRsharp_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "R# Terminal"

        Me.Icon = My.Resources.Rscript

        Me.ShowIcon = True

        Me.AppContainer1.AppProcess = Nothing
        Me.AppContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AppContainer1.Location = New System.Drawing.Point(3, 3)
        Me.AppContainer1.Name = "AppContainer1"
        Me.AppContainer1.ShowEmbedResult = False
        Me.AppContainer1.Size = New System.Drawing.Size(748, 348)
        Me.AppContainer1.TabIndex = 0

        Me.Controls.Add(Me.AppContainer1)

        Call Win32API.AllocConsole()
        Call AppContainer1.EmbedProcess(Process.GetCurrentProcess, Win32API.GetConsoleWindow, AppContainer1)

        Call New Thread(Sub()
                            Call New Shell(New PS1("> "), Sub(str)
                                                              Console.Out.WriteLine(str)
                                                          End Sub) With {
                              .Quite = "!.R#::quit" & Rnd()
                          }.Run()
                        End Sub).Start()
    End Sub
End Class

