Imports System.Windows.Forms.Design

<ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip Or ToolStripItemDesignerAvailability.StatusStrip)>
Public Class ToolStripTraceBarItem : Inherits ToolStripControlHost

    Public Event AdjustValue(value As Integer)

    Public ReadOnly Property TrackBar As TrackBar
        Get
            Return Control
        End Get
    End Property

    Sub New()
        Call MyBase.New(New TrackBar)
    End Sub

    Public Sub SetValueRange(min As Integer, max As Integer)
        With DirectCast(Control, TrackBar)
            .Minimum = min
            .Maximum = max
            .Value = min
        End With

        AddHandler TrackBar.ValueChanged,
            Sub()
                RaiseEvent AdjustValue(TrackBar.Value)
            End Sub
    End Sub
End Class
