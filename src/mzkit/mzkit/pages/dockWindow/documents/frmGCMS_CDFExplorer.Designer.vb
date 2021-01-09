Imports mzkit.DockSample
Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmGCMS_CDFExplorer
    Inherits DockContent

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.RtRangeSelector1 = New mzkit.RtRangeSelector()
        Me.SuspendLayout()
        '
        'RtRangeSelector1
        '
        Me.RtRangeSelector1.Location = New System.Drawing.Point(1, 483)
        Me.RtRangeSelector1.Name = "RtRangeSelector1"
        Me.RtRangeSelector1.Size = New System.Drawing.Size(982, 97)
        Me.RtRangeSelector1.TabIndex = 0
        '
        'frmGCMS_CDFExplorer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(982, 577)
        Me.Controls.Add(Me.RtRangeSelector1)
        Me.Name = "frmGCMS_CDFExplorer"
        Me.Text = "Form1"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents RtRangeSelector1 As RtRangeSelector
End Class
