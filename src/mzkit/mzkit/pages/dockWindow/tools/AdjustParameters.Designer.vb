<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AdjustParameters
    Inherits ToolWindow

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
        Me.propertyGrid = New System.Windows.Forms.PropertyGrid()
        Me.SuspendLayout()
        '
        'propertyGrid
        '
        Me.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar
        Me.propertyGrid.Location = New System.Drawing.Point(0, 0)
        Me.propertyGrid.Name = "propertyGrid"
        Me.propertyGrid.Size = New System.Drawing.Size(413, 543)
        Me.propertyGrid.TabIndex = 1
        '
        'AdjustParameters
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(413, 543)
        Me.Controls.Add(Me.propertyGrid)
        Me.Name = "AdjustParameters"
        Me.Text = "Adjust Parameters"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents propertyGrid As PropertyGrid
End Class
