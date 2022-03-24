#Region "Microsoft.VisualBasic::8b7ae3ac59cc160fa471b44b514c5d28, mzkit\src\mzkit\mzkit\forms\Inputs\InputNetworkLayout.Designer.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 161
    '    Code Lines: 121
    ' Comment Lines: 35
    '   Blank Lines: 5
    '     File Size: 6.67 KB


    ' Class InputNetworkLayout
    ' 
    '     Sub: Dispose, InitializeComponent
    ' 
    ' /********************************************************************************/

#End Region

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(InputNetworkLayout))
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
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
        Me.GroupBox1.Controls.Add(Me.damping)
        Me.GroupBox1.Controls.Add(Me.repulsion)
        Me.GroupBox1.Controls.Add(Me.stiffness)
        Me.GroupBox1.Controls.Add(Me.Label8)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        '
        'damping
        '
        resources.ApplyResources(Me.damping, "damping")
        Me.damping.DecimalPlaces = 2
        Me.damping.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.damping.Maximum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.damping.Name = "damping"
        Me.damping.Value = New Decimal(New Integer() {8, 0, 0, 65536})
        '
        'repulsion
        '
        resources.ApplyResources(Me.repulsion, "repulsion")
        Me.repulsion.Increment = New Decimal(New Integer() {100, 0, 0, 0})
        Me.repulsion.Maximum = New Decimal(New Integer() {20000, 0, 0, 0})
        Me.repulsion.Minimum = New Decimal(New Integer() {100, 0, 0, 0})
        Me.repulsion.Name = "repulsion"
        Me.repulsion.Value = New Decimal(New Integer() {1000, 0, 0, 0})
        '
        'stiffness
        '
        resources.ApplyResources(Me.stiffness, "stiffness")
        Me.stiffness.DecimalPlaces = 2
        Me.stiffness.Increment = New Decimal(New Integer() {5, 0, 0, 0})
        Me.stiffness.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.stiffness.Name = "stiffness"
        Me.stiffness.Value = New Decimal(New Integer() {80, 0, 0, 0})
        '
        'Label8
        '
        resources.ApplyResources(Me.Label8, "Label8")
        Me.Label8.Name = "Label8"
        '
        'Label7
        '
        resources.ApplyResources(Me.Label7, "Label7")
        Me.Label7.Name = "Label7"
        '
        'Label6
        '
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Name = "Label6"
        '
        'Button1
        '
        resources.ApplyResources(Me.Button1, "Button1")
        Me.Button1.Name = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        resources.ApplyResources(Me.Button2, "Button2")
        Me.Button2.Name = "Button2"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'InputNetworkLayout
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "InputNetworkLayout"
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
