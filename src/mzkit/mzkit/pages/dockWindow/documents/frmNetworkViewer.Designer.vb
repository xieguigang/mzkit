<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmNetworkViewer
    Inherits DocumentWindow

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
        Me.components = New System.ComponentModel.Container()
        Me.MsClusterNetworkViewer1 = New ControlLibrary.MSClusterNetworkViewer()
        Me.SuspendLayout()
        '
        'MsClusterNetworkViewer1
        '
        Me.MsClusterNetworkViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MsClusterNetworkViewer1.Location = New System.Drawing.Point(0, 0)
        Me.MsClusterNetworkViewer1.Name = "MsClusterNetworkViewer1"
        Me.MsClusterNetworkViewer1.Size = New System.Drawing.Size(888, 520)
        Me.MsClusterNetworkViewer1.TabIndex = 0
        '
        'frmNetworkViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(888, 520)
        Me.Controls.Add(Me.MsClusterNetworkViewer1)
        Me.DoubleBuffered = True
        Me.Name = "frmNetworkViewer"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents MsClusterNetworkViewer1 As ControlLibrary.MSClusterNetworkViewer
End Class
