Imports Microsoft.VisualBasic.Text.Parser

Public Class ParseChain

    ReadOnly SMILES As CharPtr

    Sub New(SMILES As String)
        Me.SMILES = SMILES
    End Sub

    Public Function ParseGraph() As networkgraph

    End Function
End Class
