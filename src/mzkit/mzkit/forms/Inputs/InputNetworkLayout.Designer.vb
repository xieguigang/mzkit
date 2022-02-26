<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class InputNetworkLayout
    Inherits InputDialog

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
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.damping = New System.Windows.Forms.NumericUpDown()
        Me.repulsion = New System.Windows.Forms.NumericUpDown()
        Me.stiffness = New System.Windows.Forms.NumericUpDown()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        CType(Me.damping, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.repulsion, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.stiffness, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.damping)
        Me.GroupBox1.Controls.Add(Me.repulsion)
        Me.GroupBox1.Controls.Add(Me.stiffness)
        Me.GroupBox1.Controls.Add(Me.Label8)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 24)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(479, 164)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Force Directed"
        '
        'damping
        '
        Me.damping.DecimalPlaces = 2
        Me.damping.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.damping.Location = New System.Drawing.Point(125, 117)
        Me.damping.Maximum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.damping.Name = "damping"
        Me.damping.Size = New System.Drawing.Size(148, 20)
        Me.damping.TabIndex = 11
        Me.damping.Value = New Decimal(New Integer() {8, 0, 0, 65536})
        '
        'repulsion
        '
        Me.repulsion.Increment = New Decimal(New Integer() {100, 0, 0, 0})
        Me.repulsion.Location = New System.Drawing.Point(125, 75)
        Me.repulsion.Maximum = New Decimal(New Integer() {20000, 0, 0, 0})
        Me.repulsion.Minimum = New Decimal(New Integer() {100, 0, 0, 0})
        Me.repulsion.Name = "repulsion"
        Me.repulsion.Size = New System.Drawing.Size(148, 20)
        Me.repulsion.TabIndex = 10
        Me.repulsion.Value = New Decimal(New Integer() {1000, 0, 0, 0})
        '
        'stiffness
        '
        Me.stiffness.DecimalPlaces = 2
        Me.stiffness.Increment = New Decimal(New Integer() {5, 0, 0, 0})
        Me.stiffness.Location = New System.Drawing.Point(125, 38)
        Me.stiffness.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.stiffness.Name = "stiffness"
        Me.stiffness.Size = New System.Drawing.Size(148, 20)
        Me.stiffness.TabIndex = 9
        Me.stiffness.Value = New Decimal(New Integer() {80, 0, 0, 0})
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(25, 75)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(57, 13)
        Me.Label8.TabIndex = 8
        Me.Label8.Text = "Repulsion:"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(25, 119)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(52, 13)
        Me.Label7.TabIndex = 7
        Me.Label7.Text = "Damping:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(25, 38)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(50, 13)
        Me.Label6.TabIndex = 6
        Me.Label6.Text = "Stiffness:"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(397, 204)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 1
        Me.Button1.Text = "Apply"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(280, 204)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 2
        Me.Button2.Text = "Cancel"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'InputNetworkLayout
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(502, 241)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "InputNetworkLayout"
        Me.Text = "Config Network Layout"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.damping, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.repulsion, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.stiffness, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents damping As NumericUpDown
    Friend WithEvents repulsion As NumericUpDown
    Friend WithEvents stiffness As NumericUpDown
    Friend WithEvents Label8 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label6 As Label
End Class
