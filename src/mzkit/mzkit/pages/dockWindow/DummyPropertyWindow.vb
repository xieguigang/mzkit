Imports System.Windows.Forms

Namespace DockSample
    Public Partial Class DummyPropertyWindow
        Inherits ToolWindow

        Public Sub New()
            InitializeComponent()
            ' comboBox.SelectedIndex = 0
            propertyGrid.SelectedObject = propertyGrid
        End Sub
    End Class
End Namespace
