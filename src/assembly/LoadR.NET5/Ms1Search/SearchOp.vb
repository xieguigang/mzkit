Imports BioNovoGene.BioDeep.MSEngine

Public MustInherit Class SearchOp

    Protected repo As IMzQuery

    Sub New(repo As IMzQuery)
        Me.repo = repo
    End Sub

End Class
