Imports System.ComponentModel
Imports System.Windows.Forms
Imports Task

Namespace DockSample
    Partial Public Class DummyPropertyWindow
        Inherits ToolWindow

        Public Sub New()
            InitializeComponent()
            ' comboBox.SelectedIndex = 0
            propertyGrid.SelectedObject = New SpectrumProperty("n/a", {})

            DoubleBuffered = True
        End Sub

        Private Sub DummyPropertyWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
            e.Cancel = True
            Me.Hide()
        End Sub
    End Class
End Namespace
