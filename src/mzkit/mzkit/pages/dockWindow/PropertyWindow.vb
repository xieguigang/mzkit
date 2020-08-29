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
    End Class
End Namespace
