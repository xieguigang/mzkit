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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(OutputWindow))
            Me.textBox1 = New System.Windows.Forms.TextBox()
            Me.textBox2 = New System.Windows.Forms.TextBox()
            Me.comboBox = New System.Windows.Forms.ComboBox()
            Me.SuspendLayout()
            '
            'textBox1
            '

            '
            'comboBox
            '
            Me.comboBox.Dock = System.Windows.Forms.DockStyle.Top
            Me.comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.comboBox.Items.AddRange(New Object() {"Mzkit", "R#"})
            Me.comboBox.Location = New System.Drawing.Point(0, 2)
            Me.comboBox.Name = "comboBox"
            Me.comboBox.Size = New System.Drawing.Size(653, 21)
            Me.comboBox.TabIndex = 1
            '
            'DummyOutputWindow
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.ClientSize = New System.Drawing.Size(653, 374)
            Me.Controls.Add(Me.textBox1)
            Me.Controls.Add(Me.textBox2)
            Me.Controls.Add(Me.comboBox)
            Me.HideOnClose = True

            Me.Name = "Output"
            Me.Padding = New System.Windows.Forms.Padding(0, 2, 0, 2)
            Me.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide
            Me.TabText = "Output"
            Me.Text = "Output"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
#End Region
        Private textBox1 As Windows.Forms.TextBox
        Private textBox2 As Windows.Forms.TextBox
        Private WithEvents comboBox As Windows.Forms.ComboBox
    End Class
End Namespace
