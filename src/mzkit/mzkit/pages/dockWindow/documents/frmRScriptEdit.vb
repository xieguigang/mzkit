#Region "Microsoft.VisualBasic::32832fe9ba8e9a859e01cc6a1de3375d, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmRScriptEdit.vb"

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

    '   Total Lines: 108
    '    Code Lines: 86
    ' Comment Lines: 1
    '   Blank Lines: 21
    '     File Size: 3.88 KB


    ' Class frmRScriptEdit
    ' 
    '     Properties: IsUnsaved, MimeType, scriptFile, ScriptText
    ' 
    '     Function: (+2 Overloads) Save
    ' 
    '     Sub: CopyFullPath, Editor1_EditCode, Editor1_OnFocus, frmRScriptEdit_Closing, frmRScriptEdit_Load
    '          LoadScript, OpenContainingFolder, SaveDocument
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text
Imports BioNovoGene.mzkit_win32.My
Imports RibbonLib.Interop

Public Class frmRScriptEdit
    Implements ISaveHandle
    Implements IFileReference

    Public Property scriptFile As String Implements IFileReference.FilePath

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return Editor1.MimeType
        End Get
    End Property

    Public ReadOnly Property IsUnsaved As Boolean = False

    Public ReadOnly Property ScriptText As String
        Get
            Return Editor1.ScriptText
        End Get
    End Property

    Public Sub LoadScript(script As String)
        Editor1.LoadScript(script)
        scriptFile = script
        TabText = script.FileName
    End Sub

    Private Sub frmRScriptEdit_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        ' 只保存新文件？
        If scriptFile.StringEmpty Then
            Dim result = MessageBox.Show("Save current script file?", "File Not Saved", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                MyApplication.host.SaveScript(Me)
            ElseIf result = DialogResult.Cancel Then
                e.Cancel = True
            End If
        End If

        If Not e.Cancel Then
            RibbonEvents.scriptFiles.Remove(Me)

            If Not MyApplication.host.dockPanel.Documents.Where(Function(d) TypeOf d Is frmRScriptEdit).Any Then
                MyApplication.host.ribbonItems.TabGroupRscriptTools.ContextAvailable = ContextAvailability.NotAvailable
            End If
        End If
    End Sub

    Private Sub frmRScriptEdit_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Icon = My.Resources.vs
        Me.ShowIcon = True
    End Sub

    Private Sub Editor1_OnFocus() Handles Editor1.OnFocus
        MyApplication.host.ribbonItems.TabGroupRscriptTools.ContextAvailable = ContextAvailability.Active
    End Sub

    Private Sub Editor1_EditCode() Handles Editor1.EditCode
        _IsUnsaved = False
    End Sub

    Protected Overrides Sub SaveDocument()
        If scriptFile.StringEmpty Then
            Dim result = MessageBox.Show("Save current script file?", "File Not Saved", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                MyApplication.host.SaveScript(Me)
            End If
        Else
            Call Save(scriptFile)
        End If
    End Sub

    Protected Overrides Sub CopyFullPath()
        If Not scriptFile.StringEmpty Then
            Call Clipboard.SetText(scriptFile)
        Else
            Call MyApplication.host.showStatusMessage("please save script file before this operation...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        End If
    End Sub

    Protected Overrides Sub OpenContainingFolder()
        If Not scriptFile.StringEmpty Then
            Call Process.Start(scriptFile.ParentPath)
        Else
            Call MyApplication.host.showStatusMessage("please save script file before this operation...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        End If
    End Sub

    Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        _IsUnsaved = False

        Return Editor1.Save(path, encoding)
    End Function

    Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        _IsUnsaved = False

        Return Save(path, encoding.CodePage)
    End Function
End Class
