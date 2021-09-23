Imports System.Windows.Forms.Design

<ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip Or ToolStripItemDesignerAvailability.StatusStrip)>
Public Class ToolStripTraceBarItem : Inherits ToolStripControlHost

    Sub New()
        Call MyBase.New(New TrackBar)
    End Sub
End Class
