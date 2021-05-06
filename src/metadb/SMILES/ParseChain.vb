Imports Microsoft.VisualBasic.Text.Parser

Public Class ParseChain

    ReadOnly graph As New ChemicalFormula
    ReadOnly chainStack As New Stack(Of ChemicalElement)
    ReadOnly SMILES As String

    Sub New(SMILES As String)
        Me.SMILES = SMILES
    End Sub

    Public Function ParseGraph() As ChemicalFormula
        Dim tokens As Token() = New Scanner(SMILES).GetTokens().ToArray
    End Function

End Class
