Public Class Atom

    Public Property label As String
    Public Property maxKeys As Integer

    Sub New(label As String, max As Integer)
        Me.label = label
        Me.maxKeys = max
    End Sub

    Public Shared Iterator Function DefaultElements() As IEnumerable(Of Atom)
        Yield New Atom("C", 4)
        Yield New Atom("O", 2)
        Yield New Atom("N", 3)
    End Function

End Class
