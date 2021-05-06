Imports Microsoft.VisualBasic.Text.Parser

Public Class ParseChain

    ReadOnly SMILES As CharPtr
    ReadOnly graph As New ChemicalFormula
    ReadOnly chainStack As New Stack(Of ChemicalElement)

    Sub New(SMILES As String)
        Me.SMILES = SMILES
    End Sub

    Public Function ParseGraph() As ChemicalFormula

    End Function
End Class
