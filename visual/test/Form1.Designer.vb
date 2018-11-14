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
        Me.xText = New System.Windows.Forms.TextBox()
        Me.yText = New System.Windows.Forms.TextBox()
        Me.zText = New System.Windows.Forms.TextBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.viewText = New System.Windows.Forms.TextBox()
        Me.fovText = New System.Windows.Forms.TextBox()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'xText
        '
        Me.xText.Location = New System.Drawing.Point(31, 24)
        Me.xText.Name = "xText"
        Me.xText.Size = New System.Drawing.Size(100, 21)
        Me.xText.TabIndex = 0
        '
        'yText
        '
        Me.yText.Location = New System.Drawing.Point(31, 71)
        Me.yText.Name = "yText"
        Me.yText.Size = New System.Drawing.Size(100, 21)
        Me.yText.TabIndex = 1
        '
        'zText
        '
        Me.zText.Location = New System.Drawing.Point(31, 116)
        Me.zText.Name = "zText"
        Me.zText.Size = New System.Drawing.Size(100, 21)
        Me.zText.TabIndex = 2
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
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.fovText)
        Me.Controls.Add(Me.viewText)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.zText)
        Me.Controls.Add(Me.yText)
        Me.Controls.Add(Me.xText)
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents xText As System.Windows.Forms.TextBox
    Friend WithEvents yText As System.Windows.Forms.TextBox
    Friend WithEvents zText As System.Windows.Forms.TextBox
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents viewText As System.Windows.Forms.TextBox
    Friend WithEvents fovText As System.Windows.Forms.TextBox
End Class
