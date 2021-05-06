Imports Microsoft.VisualBasic.Data.GraphTheory.Network

Public Class ChemicalElement : Inherits Node

    Public Property elementName As String

    Sub New()
    End Sub

    Sub New(element As String)
        Me.label = App.GetNextUniqueName($"{element}_")
        Me.elementName = element
    End Sub

End Class