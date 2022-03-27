<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class testColorBar
    Inherits System.Windows.Forms.Form

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
        Me.ColorScaleMap1 = New ControlLibrary.ColorScaleMap()
        Me.SuspendLayout()
        '
        'ColorScaleMap1
        '
        Me.ColorScaleMap1.colorMap = Microsoft.VisualBasic.Imaging.Drawing2D.Colors.ScalerPalette.Jet
        Me.ColorScaleMap1.Location = New System.Drawing.Point(129, 96)
        Me.ColorScaleMap1.mapLevels = 30
        Me.ColorScaleMap1.Name = "ColorScaleMap1"
        Me.ColorScaleMap1.range = New Double() {56356.0R, 96589999.0R}
        Me.ColorScaleMap1.Size = New System.Drawing.Size(386, 66)
        Me.ColorScaleMap1.TabIndex = 0
        '
        'testColorBar
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.ColorScaleMap1)
        Me.Name = "testColorBar"
        Me.Text = "Form1"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ColorScaleMap1 As ControlLibrary.ColorScaleMap
End Class
