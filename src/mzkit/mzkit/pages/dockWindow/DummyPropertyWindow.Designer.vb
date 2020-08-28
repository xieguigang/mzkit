Imports System

Namespace DockSample
    Partial Class DummyPropertyWindow
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As ComponentModel.IContainer = Nothing

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <paramname="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DummyPropertyWindow))
            Me.propertyGrid = New System.Windows.Forms.PropertyGrid()
            Me.SuspendLayout()
            '
            'propertyGrid
            '
            Me.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill
            Me.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar
            Me.propertyGrid.Location = New System.Drawing.Point(0, 3)
            Me.propertyGrid.Name = "propertyGrid"
            Me.propertyGrid.Size = New System.Drawing.Size(635, 472)
            Me.propertyGrid.TabIndex = 0
            '
            'DummyPropertyWindow
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.ClientSize = New System.Drawing.Size(635, 478)
            Me.Controls.Add(Me.propertyGrid)
            Me.HideOnClose = True

            Me.Name = "DummyPropertyWindow"
            Me.Padding = New System.Windows.Forms.Padding(0, 3, 0, 3)
            Me.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight
            Me.TabText = "M/Z Properties"
            Me.Text = "Properties"
            Me.ResumeLayout(False)

        End Sub
#End Region

        Friend propertyGrid As Windows.Forms.PropertyGrid
    End Class
End Namespace
