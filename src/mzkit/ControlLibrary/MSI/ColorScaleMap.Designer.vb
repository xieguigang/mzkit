#Region "Microsoft.VisualBasic::be310e15468a83268917a51c97d72e64, mzkit\src\mzkit\ControlLibrary\MSI\ColorScaleMap.Designer.vb"

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

    '   Total Lines: 65
    '    Code Lines: 46
    ' Comment Lines: 14
    '   Blank Lines: 5
    '     File Size: 2.65 KB


    ' Class ColorScaleMap
    ' 
    '     Sub: Dispose, InitializeComponent
    ' 
    ' /********************************************************************************/

#End Region

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ColorScaleMap
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
        Me.colorMapDisplay = New System.Windows.Forms.PictureBox()
        Me.SlideBar = New System.Windows.Forms.PictureBox()
        CType(Me.colorMapDisplay, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SlideBar, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'colorMapDisplay
        '
        Me.colorMapDisplay.Dock = System.Windows.Forms.DockStyle.Fill
        Me.colorMapDisplay.Location = New System.Drawing.Point(0, 0)
        Me.colorMapDisplay.Name = "colorMapDisplay"
        Me.colorMapDisplay.Size = New System.Drawing.Size(386, 79)
        Me.colorMapDisplay.TabIndex = 0
        Me.colorMapDisplay.TabStop = False
        '
        'SlideBar
        '
        Me.SlideBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.SlideBar.Location = New System.Drawing.Point(0, 79)
        Me.SlideBar.Name = "SlideBar"
        Me.SlideBar.Size = New System.Drawing.Size(386, 28)
        Me.SlideBar.TabIndex = 1
        Me.SlideBar.TabStop = False
        '
        'ColorScaleMap
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.colorMapDisplay)
        Me.Controls.Add(Me.SlideBar)
        Me.Name = "ColorScaleMap"
        Me.Size = New System.Drawing.Size(386, 107)
        CType(Me.colorMapDisplay, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SlideBar, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents colorMapDisplay As PictureBox
    Friend WithEvents SlideBar As PictureBox
End Class
