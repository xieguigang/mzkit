Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmRScriptEdit
    Inherits DockContent

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Editor1 = New RDev.Editor()
        Me.SuspendLayout()
        '
        'Editor1
        '
        Me.Editor1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Editor1.FilePath = Nothing
        Me.Editor1.Location = New System.Drawing.Point(0, 0)
        Me.Editor1.Name = "Editor1"
        Me.Editor1.Size = New System.Drawing.Size(999, 613)
        Me.Editor1.TabIndex = 0
        '
        'frmRScriptEdit
        '
        Me.ClientSize = New System.Drawing.Size(999, 613)
        Me.Controls.Add(Me.Editor1)
        Me.Name = "frmRScriptEdit"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Editor1 As RDev.Editor
End Class
