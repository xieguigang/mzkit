Imports System.Windows.Forms
Imports WeifenLuo.WinFormsUI.Docking

Namespace DockSample
    Public Partial Class ToolWindow
        Inherits DockContent

        Public Sub New()
            InitializeComponent()
            AutoScaleMode = AutoScaleMode.Dpi
            DoubleBuffered = True
        End Sub
    End Class
End Namespace
