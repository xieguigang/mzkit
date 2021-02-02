#Region "Microsoft.VisualBasic::171c1224018392aaa3264e7a49d7cd2c, MwtWinDll\MwtWin\frmTextbrowser.vb"

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

    ' Class frmTextbrowser
    ' 
    '     Properties: GetText, ReadOnlyText, SetText, TextFontSize
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: AppendText, CopyText, CutText, Dispose, InitializeComponent
    '          InitializeForm, mnuEditCopy_Click, mnuEditCut_Click, mnuEditFontSizeDecrease_Click, mnuEditFontSizeIncrease_Click
    '          mnuEditPaste_Click, mnuFileExit_Click, PasteText
    ' 
    ' /********************************************************************************/

#End Region

Option Strict On

Public Class frmTextbrowser
    Inherits System.Windows.Forms.Form

    ' -------------------------------------------------------------------------------
    ' Written by Matthew Monroe for the Department of Energy (PNNL, Richland, WA)
    ' E-mail: matthew.monroe@pnnl.gov or matt@alchemistmatt.com
    ' Website: http://ncrr.pnnl.gov/ or http://www.sysbio.org/resources/staff/
    ' -------------------------------------------------------------------------------
    ' 
    ' Licensed under the Apache License, Version 2.0; you may not use this file except
    ' in compliance with the License.  You may obtain a copy of the License at 
    ' http://www.apache.org/licenses/LICENSE-2.0
    '
    ' Notice: This computer software was prepared by Battelle Memorial Institute, 
    ' hereinafter the Contractor, under Contract No. DE-AC05-76RL0 1830 with the 
    ' Department of Energy (DOE).  All rights in the computer software are reserved 
    ' by DOE on behalf of the United States Government and the Contractor as 
    ' provided in the Contract.  NEITHER THE GOVERNMENT NOR THE CONTRACTOR MAKES ANY 
    ' WARRANTY, EXPRESS OR IMPLIED, OR ASSUMES ANY LIABILITY FOR THE USE OF THIS 
    ' SOFTWARE.  This notice including this sentence must appear on any copies of 
    ' this computer software.

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        InitializeForm()
    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents txtData As System.Windows.Forms.TextBox
    Friend WithEvents MainMenuControl As System.Windows.Forms.MainMenu
    Friend WithEvents mnuFile As System.Windows.Forms.MenuItem
    Friend WithEvents mnuFileExit As System.Windows.Forms.MenuItem
    Friend WithEvents mnuEdit As System.Windows.Forms.MenuItem
    Friend WithEvents mnuEditCut As System.Windows.Forms.MenuItem
    Friend WithEvents mnuEditCopy As System.Windows.Forms.MenuItem
    Friend WithEvents mnuEditPaste As System.Windows.Forms.MenuItem
    Friend WithEvents mnuEditSep1 As System.Windows.Forms.MenuItem
    Friend WithEvents mnuEditFontSizeDecrease As System.Windows.Forms.MenuItem
    Friend WithEvents mnuEditFontSizeIncrease As System.Windows.Forms.MenuItem
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.txtData = New System.Windows.Forms.TextBox
        Me.MainMenuControl = New System.Windows.Forms.MainMenu
        Me.mnuFile = New System.Windows.Forms.MenuItem
        Me.mnuFileExit = New System.Windows.Forms.MenuItem
        Me.mnuEdit = New System.Windows.Forms.MenuItem
        Me.mnuEditCut = New System.Windows.Forms.MenuItem
        Me.mnuEditCopy = New System.Windows.Forms.MenuItem
        Me.mnuEditPaste = New System.Windows.Forms.MenuItem
        Me.mnuEditSep1 = New System.Windows.Forms.MenuItem
        Me.mnuEditFontSizeDecrease = New System.Windows.Forms.MenuItem
        Me.mnuEditFontSizeIncrease = New System.Windows.Forms.MenuItem
        Me.SuspendLayout()
        '
        'txtData
        '
        Me.txtData.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtData.Location = New System.Drawing.Point(0, 0)
        Me.txtData.Multiline = True
        Me.txtData.Name = "txtData"
        Me.txtData.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtData.Size = New System.Drawing.Size(488, 316)
        Me.txtData.TabIndex = 0
        Me.txtData.Text = ""
        Me.txtData.WordWrap = False
        '
        'MainMenuControl
        '
        Me.MainMenuControl.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuFile, Me.mnuEdit})
        '
        'mnuFile
        '
        Me.mnuFile.Index = 0
        Me.mnuFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuFileExit})
        Me.mnuFile.Text = "&File"
        '
        'mnuFileExit
        '
        Me.mnuFileExit.Index = 0
        Me.mnuFileExit.Text = "E&xit"
        '
        'mnuEdit
        '
        Me.mnuEdit.Index = 1
        Me.mnuEdit.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuEditCut, Me.mnuEditCopy, Me.mnuEditPaste, Me.mnuEditSep1, Me.mnuEditFontSizeDecrease, Me.mnuEditFontSizeIncrease})
        Me.mnuEdit.Text = "&Edit"
        '
        'mnuEditCut
        '
        Me.mnuEditCut.Index = 0
        Me.mnuEditCut.Text = "Cu&t"
        '
        'mnuEditCopy
        '
        Me.mnuEditCopy.Index = 1
        Me.mnuEditCopy.Text = "&Copy"
        '
        'mnuEditPaste
        '
        Me.mnuEditPaste.Index = 2
        Me.mnuEditPaste.Text = "&Paste"
        '
        'mnuEditSep1
        '
        Me.mnuEditSep1.Index = 3
        Me.mnuEditSep1.Text = "-"
        '
        'mnuEditFontSizeDecrease
        '
        Me.mnuEditFontSizeDecrease.Index = 4
        Me.mnuEditFontSizeDecrease.Shortcut = System.Windows.Forms.Shortcut.F3
        Me.mnuEditFontSizeDecrease.Text = "Decrease Font Size"
        '
        'mnuEditFontSizeIncrease
        '
        Me.mnuEditFontSizeIncrease.Index = 5
        Me.mnuEditFontSizeIncrease.Shortcut = System.Windows.Forms.Shortcut.F4
        Me.mnuEditFontSizeIncrease.Text = "Increase Font Size"
        '
        'frmTextbrowser
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(488, 314)
        Me.Controls.Add(Me.txtData)
        Me.Menu = Me.MainMenuControl
        Me.Name = "frmTextbrowser"
        Me.Text = "frmTextbrowser"
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "Processing Options Interface Functions"
    Public Property ReadOnlyText() As Boolean
        Get
            Return txtData.ReadOnly
        End Get
        Set(Value As Boolean)
            txtData.ReadOnly = Value
        End Set
    End Property

    Public ReadOnly Property GetText() As String
        Get
            Return txtData.Text
        End Get
    End Property

    Public WriteOnly Property SetText() As String
        Set(Value As String)
            txtData.Text = Value
            txtData.SelectionStart = 1
            txtData.ScrollToCaret()
        End Set
    End Property

    Public Property TextFontSize() As Single
        Get
            Return txtData.Font.SizeInPoints
        End Get
        Set(Value As Single)
            If Value < 6 Then
                Value = 6
            ElseIf Value > 72 Then
                Value = 72
            End If

            Try
                txtData.Font = New System.Drawing.Font(txtData.Font.FontFamily, Value)
            Catch ex As Exception
                ' Ignore errors here
            End Try

        End Set
    End Property
#End Region

#Region "Procedures"

    Public Sub AppendText(Value As String)
        txtData.Text &= Value & ControlChars.NewLine
        txtData.SelectionStart = txtData.TextLength
        txtData.ScrollToCaret()
    End Sub

    Private Sub CopyText()
        txtData.Copy()
    End Sub

    Private Sub CutText()
        If txtData.ReadOnly Then
            CopyText()
        Else
            txtData.Cut()
        End If
    End Sub

    Private Sub InitializeForm()
        txtData.ReadOnly = True
        Me.TextFontSize = 11
    End Sub

    Private Sub PasteText()
        If txtData.ReadOnly Then Exit Sub

        txtData.Paste()
    End Sub
#End Region

#Region "Menu Handlers"

    Private Sub mnuFileExit_Click(sender As System.Object, e As System.EventArgs) Handles mnuFileExit.Click
        Me.Close()
    End Sub

    Private Sub mnuEditCut_Click(sender As System.Object, e As System.EventArgs) Handles mnuEditCut.Click
        CutText()
    End Sub
    Private Sub mnuEditCopy_Click(sender As Object, e As System.EventArgs) Handles mnuEditCopy.Click
        CopyText()
    End Sub
    Private Sub mnuEditPaste_Click(sender As Object, e As System.EventArgs) Handles mnuEditPaste.Click
        PasteText()
    End Sub

    Private Sub mnuEditFontSizeDecrease_Click(sender As System.Object, e As System.EventArgs) Handles mnuEditFontSizeDecrease.Click
        If Me.TextFontSize > 14 Then
            Me.TextFontSize = Me.TextFontSize - 2
        Else
            Me.TextFontSize = Me.TextFontSize - 1
        End If
    End Sub

    Private Sub mnuEditFontSizeIncrease_Click(sender As System.Object, e As System.EventArgs) Handles mnuEditFontSizeIncrease.Click
        If Me.TextFontSize >= 14 Then
            Me.TextFontSize = Me.TextFontSize + 2
        Else
            Me.TextFontSize = Me.TextFontSize + 1
        End If
    End Sub

#End Region

End Class
