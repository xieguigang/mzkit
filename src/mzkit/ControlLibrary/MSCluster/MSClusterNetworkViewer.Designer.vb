<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MSClusterNetworkViewer
    Inherits System.Windows.Forms.UserControl

    'UserControl 重写释放以清理组件列表。
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
        Me.Canvas1 = New Microsoft.VisualBasic.Data.visualize.Network.Canvas.Canvas()
        Me.SuspendLayout()
        '
        'Canvas1
        '
        Me.Canvas1.AutoRotate = True
        Me.Canvas1.BackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(243, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.Canvas1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Canvas1.DynamicsRadius = False
        Me.Canvas1.Font = New System.Drawing.Font("Microsoft YaHei UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Canvas1.Location = New System.Drawing.Point(0, 0)
        Me.Canvas1.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Canvas1.Name = "Canvas1"
        Me.Canvas1.ShowLabel = False
        Me.Canvas1.Size = New System.Drawing.Size(917, 595)
        Me.Canvas1.TabIndex = 0
        Me.Canvas1.ViewDistance = 0R
        '
        'MSClusterNetworkViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Canvas1)
        Me.Name = "MSClusterNetworkViewer"
        Me.Size = New System.Drawing.Size(917, 595)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Canvas1 As Microsoft.VisualBasic.Data.visualize.Network.Canvas.Canvas
End Class
