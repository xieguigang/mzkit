Imports ControlLibrary

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmUntargettedViewer
    Inherits DocumentWindow

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
        Me.components = New System.ComponentModel.Container()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.MsSelector1 = New MSSelector()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PictureBox1
        '
        Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBox1.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(904, 392)
        Me.PictureBox1.TabIndex = 1
        Me.PictureBox1.TabStop = False
        '
        'MsSelector1
        '
        Me.MsSelector1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.MsSelector1.FillColor = System.Drawing.Color.Blue
        Me.MsSelector1.Location = New System.Drawing.Point(0, 392)
        Me.MsSelector1.Name = "MsSelector1"
        Me.MsSelector1.rtmax = 0R
        Me.MsSelector1.rtmin = 0R
        Me.MsSelector1.SelectedColor = System.Drawing.Color.Green
        Me.MsSelector1.Size = New System.Drawing.Size(904, 143)
        Me.MsSelector1.TabIndex = 2
        '
        'frmUntargettedViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(904, 535)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.MsSelector1)
        Me.DoubleBuffered = True
        Me.Name = "frmUntargettedViewer"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents MsSelector1 As MSSelector
End Class
