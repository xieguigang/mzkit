<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MolecularNetworking
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.NumericUpDown1 = New System.Windows.Forms.NumericUpDown()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TrackBar1 = New System.Windows.Forms.TrackBar()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TrackBar2 = New System.Windows.Forms.TrackBar()
        Me.TrackBar3 = New System.Windows.Forms.TrackBar()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TrackBar4 = New System.Windows.Forms.TrackBar()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.stiffness = New System.Windows.Forms.NumericUpDown()
        Me.repulsion = New System.Windows.Forms.NumericUpDown()
        Me.damping = New System.Windows.Forms.NumericUpDown()
        CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TrackBar2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TrackBar3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TrackBar4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        CType(Me.stiffness, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.repulsion, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.damping, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(36, 32)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(88, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Layout Iterations:"
        '
        'NumericUpDown1
        '
        Me.NumericUpDown1.Increment = New Decimal(New Integer() {25, 0, 0, 0})
        Me.NumericUpDown1.Location = New System.Drawing.Point(149, 32)
        Me.NumericUpDown1.Maximum = New Decimal(New Integer() {3000, 0, 0, 0})
        Me.NumericUpDown1.Minimum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.NumericUpDown1.Name = "NumericUpDown1"
        Me.NumericUpDown1.Size = New System.Drawing.Size(148, 20)
        Me.NumericUpDown1.TabIndex = 1
        Me.NumericUpDown1.Value = New Decimal(New Integer() {100, 0, 0, 0})
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.TrackBar3)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.TrackBar4)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.TrackBar2)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.TrackBar1)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Location = New System.Drawing.Point(23, 277)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(719, 178)
        Me.GroupBox1.TabIndex = 2
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Network Styling"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(26, 37)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(107, 13)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Node Radius Range:"
        '
        'TrackBar1
        '
        Me.TrackBar1.LargeChange = 10
        Me.TrackBar1.Location = New System.Drawing.Point(177, 37)
        Me.TrackBar1.Maximum = 100
        Me.TrackBar1.Minimum = 1
        Me.TrackBar1.Name = "TrackBar1"
        Me.TrackBar1.Size = New System.Drawing.Size(190, 45)
        Me.TrackBar1.TabIndex = 1
        Me.TrackBar1.TickFrequency = 10
        Me.TrackBar1.Value = 1
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(390, 50)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(20, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "To"
        '
        'TrackBar2
        '
        Me.TrackBar2.LargeChange = 10
        Me.TrackBar2.Location = New System.Drawing.Point(433, 37)
        Me.TrackBar2.Maximum = 200
        Me.TrackBar2.Minimum = 10
        Me.TrackBar2.Name = "TrackBar2"
        Me.TrackBar2.Size = New System.Drawing.Size(187, 45)
        Me.TrackBar2.SmallChange = 5
        Me.TrackBar2.TabIndex = 3
        Me.TrackBar2.TickFrequency = 10
        Me.TrackBar2.Value = 10
        '
        'TrackBar3
        '
        Me.TrackBar3.Location = New System.Drawing.Point(433, 123)
        Me.TrackBar3.Maximum = 30
        Me.TrackBar3.Minimum = 5
        Me.TrackBar3.Name = "TrackBar3"
        Me.TrackBar3.Size = New System.Drawing.Size(187, 45)
        Me.TrackBar3.SmallChange = 3
        Me.TrackBar3.TabIndex = 7
        Me.TrackBar3.TickFrequency = 3
        Me.TrackBar3.Value = 5
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(390, 136)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(20, 13)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "To"
        '
        'TrackBar4
        '
        Me.TrackBar4.LargeChange = 3
        Me.TrackBar4.Location = New System.Drawing.Point(177, 123)
        Me.TrackBar4.Maximum = 15
        Me.TrackBar4.Minimum = 1
        Me.TrackBar4.Name = "TrackBar4"
        Me.TrackBar4.Size = New System.Drawing.Size(190, 45)
        Me.TrackBar4.TabIndex = 5
        Me.TrackBar4.TickFrequency = 3
        Me.TrackBar4.Value = 1
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(26, 123)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(96, 13)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Link Width Range:"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.damping)
        Me.GroupBox2.Controls.Add(Me.repulsion)
        Me.GroupBox2.Controls.Add(Me.stiffness)
        Me.GroupBox2.Controls.Add(Me.Label8)
        Me.GroupBox2.Controls.Add(Me.Label7)
        Me.GroupBox2.Controls.Add(Me.Label6)
        Me.GroupBox2.Location = New System.Drawing.Point(23, 79)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(719, 183)
        Me.GroupBox2.TabIndex = 3
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Layouts"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(26, 46)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(50, 13)
        Me.Label6.TabIndex = 0
        Me.Label6.Text = "Stiffness:"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(26, 141)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(52, 13)
        Me.Label7.TabIndex = 1
        Me.Label7.Text = "Damping:"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(26, 91)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(57, 13)
        Me.Label8.TabIndex = 2
        Me.Label8.Text = "Repulsion:"
        '
        'stiffness
        '
        Me.stiffness.Increment = New Decimal(New Integer() {5, 0, 0, 0})
        Me.stiffness.Location = New System.Drawing.Point(126, 46)
        Me.stiffness.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.stiffness.Name = "stiffness"
        Me.stiffness.Size = New System.Drawing.Size(148, 20)
        Me.stiffness.TabIndex = 3
        Me.stiffness.Value = New Decimal(New Integer() {80, 0, 0, 0})
        '
        'repulsion
        '
        Me.repulsion.Increment = New Decimal(New Integer() {100, 0, 0, 0})
        Me.repulsion.Location = New System.Drawing.Point(126, 91)
        Me.repulsion.Maximum = New Decimal(New Integer() {20000, 0, 0, 0})
        Me.repulsion.Minimum = New Decimal(New Integer() {100, 0, 0, 0})
        Me.repulsion.Name = "repulsion"
        Me.repulsion.Size = New System.Drawing.Size(148, 20)
        Me.repulsion.TabIndex = 4
        Me.repulsion.Value = New Decimal(New Integer() {1000, 0, 0, 0})
        '
        'damping
        '
        Me.damping.DecimalPlaces = 2
        Me.damping.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.damping.Location = New System.Drawing.Point(126, 139)
        Me.damping.Maximum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.damping.Name = "damping"
        Me.damping.Size = New System.Drawing.Size(148, 20)
        Me.damping.TabIndex = 5
        Me.damping.Value = New Decimal(New Integer() {8, 0, 0, 65536})
        '
        'MolecularNetworking
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.NumericUpDown1)
        Me.Controls.Add(Me.Label1)
        Me.Name = "MolecularNetworking"
        Me.Size = New System.Drawing.Size(770, 531)
        CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TrackBar2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TrackBar3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TrackBar4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        CType(Me.stiffness, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.repulsion, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.damping, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents NumericUpDown1 As NumericUpDown
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents Label2 As Label
    Friend WithEvents TrackBar2 As TrackBar
    Friend WithEvents Label3 As Label
    Friend WithEvents TrackBar1 As TrackBar
    Friend WithEvents TrackBar3 As TrackBar
    Friend WithEvents Label4 As Label
    Friend WithEvents TrackBar4 As TrackBar
    Friend WithEvents Label5 As Label
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents Label6 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents damping As NumericUpDown
    Friend WithEvents repulsion As NumericUpDown
    Friend WithEvents stiffness As NumericUpDown
End Class
