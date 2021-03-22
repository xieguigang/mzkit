Namespace DockSample
    Partial Class OutputWindow
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As ComponentModel.IContainer = Nothing

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If

            MyBase.Dispose(disposing)
        End Sub

#Region "Windows Form Designer generated code"
        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(OutputWindow))
            Me.textBox1 = New System.Windows.Forms.TextBox()
            Me.textBox2 = New System.Windows.Forms.TextBox()
            Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
            Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
            Me.ToolStripComboBox1 = New System.Windows.Forms.ToolStripComboBox()
            Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
            Me.Panel1 = New System.Windows.Forms.Panel()
            Me.ToolStrip1.SuspendLayout()
            Me.Panel1.SuspendLayout()
            Me.SuspendLayout()
            '
            'textBox1
            '
            Me.textBox1.Location = New System.Drawing.Point(27, 17)
            Me.textBox1.Multiline = True
            Me.textBox1.Name = "textBox1"
            Me.textBox1.ReadOnly = True
            Me.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            Me.textBox1.Size = New System.Drawing.Size(277, 158)
            Me.textBox1.TabIndex = 0
            '
            'textBox2
            '
            Me.textBox2.Location = New System.Drawing.Point(136, 74)
            Me.textBox2.Multiline = True
            Me.textBox2.Name = "textBox2"
            Me.textBox2.ReadOnly = True
            Me.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            Me.textBox2.Size = New System.Drawing.Size(242, 115)
            Me.textBox2.TabIndex = 1
            '
            'ToolStrip1
            '
            Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel1, Me.ToolStripComboBox1, Me.ToolStripButton1})
            Me.ToolStrip1.Location = New System.Drawing.Point(0, 2)
            Me.ToolStrip1.Name = "ToolStrip1"
            Me.ToolStrip1.Size = New System.Drawing.Size(653, 25)
            Me.ToolStrip1.TabIndex = 2
            Me.ToolStrip1.Text = "ToolStrip1"
            '
            'ToolStripLabel1
            '
            Me.ToolStripLabel1.Name = "ToolStripLabel1"
            Me.ToolStripLabel1.Size = New System.Drawing.Size(111, 22)
            Me.ToolStripLabel1.Text = "Show Output From:"
            '
            'ToolStripComboBox1
            '
            Me.ToolStripComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ToolStripComboBox1.Items.AddRange(New Object() {"Mzkit", "R#"})
            Me.ToolStripComboBox1.Name = "ToolStripComboBox1"
            Me.ToolStripComboBox1.Size = New System.Drawing.Size(300, 25)
            '
            'ToolStripButton1
            '
            Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.ToolStripButton1.Image = CType(resources.GetObject("ToolStripButton1.Image"), System.Drawing.Image)
            Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.ToolStripButton1.Name = "ToolStripButton1"
            Me.ToolStripButton1.Size = New System.Drawing.Size(23, 22)
            Me.ToolStripButton1.Text = "Clear All"
            '
            'Panel1
            '
            Me.Panel1.Controls.Add(Me.textBox1)
            Me.Panel1.Controls.Add(Me.textBox2)
            Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Panel1.Location = New System.Drawing.Point(0, 27)
            Me.Panel1.Name = "Panel1"
            Me.Panel1.Size = New System.Drawing.Size(653, 345)
            Me.Panel1.TabIndex = 3
            '
            'OutputWindow
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.ClientSize = New System.Drawing.Size(653, 374)
            Me.Controls.Add(Me.Panel1)
            Me.Controls.Add(Me.ToolStrip1)
            Me.DoubleBuffered = True
            Me.HideOnClose = True
            Me.Name = "OutputWindow"
            Me.Padding = New System.Windows.Forms.Padding(0, 2, 0, 2)
            Me.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
            Me.TabText = "Output"
            Me.Text = "Output"
            Me.ToolStrip1.ResumeLayout(False)
            Me.ToolStrip1.PerformLayout()
            Me.Panel1.ResumeLayout(False)
            Me.Panel1.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
#End Region
        Private textBox1 As Windows.Forms.TextBox
        Private textBox2 As Windows.Forms.TextBox
        Friend WithEvents ToolStrip1 As ToolStrip
        Friend WithEvents ToolStripLabel1 As ToolStripLabel
        Friend WithEvents ToolStripButton1 As ToolStripButton
        Friend WithEvents ToolStripComboBox1 As ToolStripComboBox
        Friend WithEvents Panel1 As Panel
    End Class
End Namespace
