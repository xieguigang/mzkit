Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Module KEGGRepo

    Public Function RequestKEGGcompounds() As Compound()
        Const url As String = "http://query.biodeep.cn/kegg/repository/compounds"
    End Function

    Public Function RequestKEGGReactions() As ReactionClass()
        Const url As String = "http://query.biodeep.cn/kegg/repository/reactions"
    End Function
End Module
