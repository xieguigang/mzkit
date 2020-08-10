<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.Ribbon1 = New System.Windows.Forms.Ribbon()
        Me.RibbonContext1 = New System.Windows.Forms.RibbonContext()
        Me.RibbonButton1 = New System.Windows.Forms.RibbonButton()
        Me.RibbonTab1 = New System.Windows.Forms.RibbonTab()
        Me.RibbonOrbMenuItem1 = New System.Windows.Forms.RibbonOrbMenuItem()
        Me.RibbonOrbMenuItem2 = New System.Windows.Forms.RibbonOrbMenuItem()
        Me.SuspendLayout()
        '
        'Ribbon1
        '
        Me.Ribbon1.Contexts.Add(Me.RibbonContext1)
        Me.Ribbon1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.Ribbon1.Location = New System.Drawing.Point(0, 0)
        Me.Ribbon1.Minimized = False
        Me.Ribbon1.Name = "Ribbon1"
        '
        '
        '
        Me.Ribbon1.OrbDropDown.BorderRoundness = 8
        Me.Ribbon1.OrbDropDown.Location = New System.Drawing.Point(0, 0)
        Me.Ribbon1.OrbDropDown.Name = ""
        Me.Ribbon1.OrbDropDown.Size = New System.Drawing.Size(527, 447)
        Me.Ribbon1.OrbDropDown.TabIndex = 0
        Me.Ribbon1.OrbStyle = System.Windows.Forms.RibbonOrbStyle.Office_2010_Extended
        '
        '
        '
        Me.Ribbon1.QuickAccessToolbar.Items.Add(Me.RibbonButton1)
        Me.Ribbon1.RibbonTabFont = New System.Drawing.Font("Trebuchet MS", 9.0!)
        Me.Ribbon1.Size = New System.Drawing.Size(1219, 168)
        Me.Ribbon1.TabIndex = 0
        Me.Ribbon1.Tabs.Add(Me.RibbonTab1)
        Me.Ribbon1.TabSpacing = 3
        Me.Ribbon1.Text = "Ribbon1"
        '
        'RibbonContext1
        '
        Me.RibbonContext1.GlowColor = System.Drawing.Color.FromArgb(CType(CType(230, Byte), Integer), CType(CType(193, Byte), Integer), CType(CType(182, Byte), Integer))
        Me.RibbonContext1.Text = "RibbonContext1"
        '
        'RibbonButton1
        '
        Me.RibbonButton1.Image = CType(resources.GetObject("RibbonButton1.Image"), System.Drawing.Image)
        Me.RibbonButton1.LargeImage = CType(resources.GetObject("RibbonButton1.LargeImage"), System.Drawing.Image)
        Me.RibbonButton1.MaxSizeMode = System.Windows.Forms.RibbonElementSizeMode.Compact
        Me.RibbonButton1.Name = "RibbonButton1"
        Me.RibbonButton1.SmallImage = CType(resources.GetObject("RibbonButton1.SmallImage"), System.Drawing.Image)
        Me.RibbonButton1.Text = "RibbonButton1"
        '
        'RibbonTab1
        '
        Me.RibbonTab1.Name = "RibbonTab1"
        Me.RibbonTab1.Text = "RibbonTab1"
        '
        'RibbonOrbMenuItem1
        '
        Me.RibbonOrbMenuItem1.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left
        Me.RibbonOrbMenuItem1.Image = CType(resources.GetObject("RibbonOrbMenuItem1.Image"), System.Drawing.Image)
        Me.RibbonOrbMenuItem1.LargeImage = CType(resources.GetObject("RibbonOrbMenuItem1.LargeImage"), System.Drawing.Image)
        Me.RibbonOrbMenuItem1.Name = "RibbonOrbMenuItem1"
        Me.RibbonOrbMenuItem1.SmallImage = CType(resources.GetObject("RibbonOrbMenuItem1.SmallImage"), System.Drawing.Image)
        Me.RibbonOrbMenuItem1.Text = "File"
        '
        'RibbonOrbMenuItem2
        '
        Me.RibbonOrbMenuItem2.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left
        Me.RibbonOrbMenuItem2.Image = CType(resources.GetObject("RibbonOrbMenuItem2.Image"), System.Drawing.Image)
        Me.RibbonOrbMenuItem2.LargeImage = CType(resources.GetObject("RibbonOrbMenuItem2.LargeImage"), System.Drawing.Image)
        Me.RibbonOrbMenuItem2.Name = "RibbonOrbMenuItem2"
        Me.RibbonOrbMenuItem2.SmallImage = CType(resources.GetObject("RibbonOrbMenuItem2.SmallImage"), System.Drawing.Image)
        Me.RibbonOrbMenuItem2.Text = "Exit"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1219, 747)
        Me.Controls.Add(Me.Ribbon1)
        Me.KeyPreview = True
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Ribbon1 As Ribbon
    Friend WithEvents RibbonContext1 As RibbonContext
    Friend WithEvents RibbonButton1 As RibbonButton
    Friend WithEvents RibbonTab1 As RibbonTab
    Friend WithEvents RibbonOrbMenuItem1 As RibbonOrbMenuItem
    Friend WithEvents RibbonOrbMenuItem2 As RibbonOrbMenuItem
End Class
