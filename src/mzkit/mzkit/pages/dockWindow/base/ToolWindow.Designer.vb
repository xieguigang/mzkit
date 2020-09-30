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
            components = New ComponentModel.Container()
            contextMenuStrip1 = New Windows.Forms.ContextMenuStrip(components)
            floatToolStripMenuItem = New Windows.Forms.ToolStripMenuItem()
            dockToolStripMenuItem = New Windows.Forms.ToolStripMenuItem()
            autoHideToolStripMenuItem = New Windows.Forms.ToolStripMenuItem()
            closeToolStripMenuItem = New Windows.Forms.ToolStripMenuItem()
            contextMenuStrip1.SuspendLayout()
            SuspendLayout()
            ' 
            ' contextMenuStrip1
            ' 
            contextMenuStrip1.Items.AddRange(New Windows.Forms.ToolStripItem() {floatToolStripMenuItem, dockToolStripMenuItem, autoHideToolStripMenuItem, closeToolStripMenuItem})
            contextMenuStrip1.Name = "contextMenuStrip1"
            contextMenuStrip1.Size = New Drawing.Size(113, 70)
            ' 
            ' option1ToolStripMenuItem
            ' 
            floatToolStripMenuItem.Name = "option1ToolStripMenuItem"
            floatToolStripMenuItem.Size = New Drawing.Size(152, 22)
            floatToolStripMenuItem.Text = "Float"
            ' 
            ' option2ToolStripMenuItem
            ' 
            dockToolStripMenuItem.Name = "option2ToolStripMenuItem"
            dockToolStripMenuItem.Size = New Drawing.Size(152, 22)
            dockToolStripMenuItem.Text = "Dock"
            ' 
            ' option3ToolStripMenuItem
            ' 
            autoHideToolStripMenuItem.Name = "option3ToolStripMenuItem"
            autoHideToolStripMenuItem.Size = New Drawing.Size(152, 22)
            autoHideToolStripMenuItem.Text = "Auto Hide"

            closeToolStripMenuItem.Name = "option4ToolStripMenuItem"
            closeToolStripMenuItem.Size = New Drawing.Size(152, 22)
            closeToolStripMenuItem.Text = "Close"
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
        Private WithEvents floatToolStripMenuItem As Windows.Forms.ToolStripMenuItem
        Private WithEvents dockToolStripMenuItem As Windows.Forms.ToolStripMenuItem
        Private WithEvents autoHideToolStripMenuItem As Windows.Forms.ToolStripMenuItem
        Private WithEvents closeToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    End Class
End Namespace
