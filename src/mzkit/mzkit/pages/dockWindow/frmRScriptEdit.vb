Imports System.ComponentModel
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text
Imports mzkit.My
Imports RibbonLib.Interop

Public Class frmRScriptEdit
    Implements ISaveHandle
    Implements IFileReference

    Public Property scriptFile As String Implements IFileReference.FilePath

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return {
                New ContentType With {.Details = "http://r_lang.dev.smrucc.org/", .FileExt = ".R", .MIMEType = "text/r_sharp", .Name = "R# script"}
            }
        End Get
    End Property

    Friend WithEvents script As New PageRscriptEditor

    Public Sub LoadScript(script As String)
        Me.script.FastColoredTextBox1.Text = script
    End Sub

    Private Sub frmRScriptEdit_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If scriptFile.StringEmpty Then
            Dim result = MessageBox.Show("Save current script file?", "File Not Saved", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
            ElseIf result = DialogResult.Cancel Then
                e.Cancel = True
            End If
        End If
    End Sub

    Private Sub frmRScriptEdit_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(script)
        script.Dock = DockStyle.Fill
        Me.Icon = My.Resources.vs

        Me.ShowIcon = True
        '   Me.ShowInTaskbar = True
    End Sub

    Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Return script.FastColoredTextBox1.Text.SaveTo(path, encoding)
    End Function

    Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        Return Save(path, encoding.CodePage)
    End Function
End Class