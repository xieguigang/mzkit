<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
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

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.viewText = New System.Windows.Forms.TextBox()
        Me.fovText = New System.Windows.Forms.TextBox()
        Me.xTrack = New System.Windows.Forms.TrackBar()
        Me.yTrack = New System.Windows.Forms.TrackBar()
        Me.zTrack = New System.Windows.Forms.TrackBar()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.xTrack, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.yTrack, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.zTrack, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PictureBox1
        '
        Me.PictureBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.PictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.PictureBox1.Location = New System.Drawing.Point(149, 12)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(629, 426)
        Me.PictureBox1.TabIndex = 3
        Me.PictureBox1.TabStop = False
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(31, 300)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(87, 32)
        Me.Button1.TabIndex = 4
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'viewText
        '
        Me.viewText.Location = New System.Drawing.Point(31, 174)
        Me.viewText.Name = "viewText"
        Me.viewText.Size = New System.Drawing.Size(100, 21)
        Me.viewText.TabIndex = 5
        '
        'fovText
        '
        Me.fovText.Location = New System.Drawing.Point(31, 229)
        Me.fovText.Name = "fovText"
        Me.fovText.Size = New System.Drawing.Size(100, 21)
        Me.fovText.TabIndex = 6
        '
        'xTrack
        '
        Me.xTrack.Location = New System.Drawing.Point(14, 12)
        Me.xTrack.Maximum = 360
        Me.xTrack.Minimum = -360
        Me.xTrack.Name = "xTrack"
        Me.xTrack.Size = New System.Drawing.Size(117, 45)
        Me.xTrack.TabIndex = 7
        '
        'yTrack
        '
        Me.yTrack.Location = New System.Drawing.Point(14, 63)
        Me.yTrack.Maximum = 360
        Me.yTrack.Minimum = -360
        Me.yTrack.Name = "yTrack"
        Me.yTrack.Size = New System.Drawing.Size(117, 45)
        Me.yTrack.TabIndex = 8
        '
        'zTrack
        '
        Me.zTrack.Location = New System.Drawing.Point(14, 114)
        Me.zTrack.Maximum = 360
        Me.zTrack.Minimum = -360
        Me.zTrack.Name = "zTrack"
        Me.zTrack.Size = New System.Drawing.Size(117, 45)
        Me.zTrack.TabIndex = 9
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.zTrack)
        Me.Controls.Add(Me.yTrack)
        Me.Controls.Add(Me.xTrack)
        Me.Controls.Add(Me.fovText)
        Me.Controls.Add(Me.viewText)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.PictureBox1)
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.xTrack, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.yTrack, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.zTrack, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents viewText As System.Windows.Forms.TextBox
    Friend WithEvents fovText As System.Windows.Forms.TextBox
    Friend WithEvents xTrack As System.Windows.Forms.TrackBar
    Friend WithEvents yTrack As System.Windows.Forms.TrackBar
    Friend WithEvents zTrack As System.Windows.Forms.TrackBar
End Class
