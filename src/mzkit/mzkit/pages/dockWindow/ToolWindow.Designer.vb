Namespace DockSample
    Partial Class ToolWindow
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As ComponentModel.IContainer = Nothing

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
            components = New ComponentModel.Container()
            contextMenuStrip1 = New Windows.Forms.ContextMenuStrip(components)
            option1ToolStripMenuItem = New Windows.Forms.ToolStripMenuItem()
            option2ToolStripMenuItem = New Windows.Forms.ToolStripMenuItem()
            option3ToolStripMenuItem = New Windows.Forms.ToolStripMenuItem()
            contextMenuStrip1.SuspendLayout()
            SuspendLayout()
            ' 
            ' contextMenuStrip1
            ' 
            contextMenuStrip1.Items.AddRange(New Windows.Forms.ToolStripItem() {option1ToolStripMenuItem, option2ToolStripMenuItem, option3ToolStripMenuItem})
            contextMenuStrip1.Name = "contextMenuStrip1"
            contextMenuStrip1.Size = New Drawing.Size(113, 70)
            ' 
            ' option1ToolStripMenuItem
            ' 
            option1ToolStripMenuItem.Name = "option1ToolStripMenuItem"
            option1ToolStripMenuItem.Size = New Drawing.Size(152, 22)
            option1ToolStripMenuItem.Text = "Option&1"
            ' 
            ' option2ToolStripMenuItem
            ' 
            option2ToolStripMenuItem.Name = "option2ToolStripMenuItem"
            option2ToolStripMenuItem.Size = New Drawing.Size(152, 22)
            option2ToolStripMenuItem.Text = "Option&2"
            ' 
            ' option3ToolStripMenuItem
            ' 
            option3ToolStripMenuItem.Name = "option3ToolStripMenuItem"
            option3ToolStripMenuItem.Size = New Drawing.Size(152, 22)
            option3ToolStripMenuItem.Text = "Option&3"
            ' 
            ' ToolWindow
            ' 
            ClientSize = New Drawing.Size(292, 266)
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Float Or WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft Or WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight Or WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop Or WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom
            Name = "ToolWindow"
            TabPageContextMenuStrip = contextMenuStrip1
            TabText = "ToolWindow"
            Text = "ToolWindow"
            contextMenuStrip1.ResumeLayout(False)
            ResumeLayout(False)
        End Sub

#End Region

        Private contextMenuStrip1 As Windows.Forms.ContextMenuStrip
        Private option1ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
        Private option2ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
        Private option3ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    End Class
End Namespace
