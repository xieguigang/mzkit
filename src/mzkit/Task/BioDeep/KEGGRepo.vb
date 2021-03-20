Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.My
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Data.KEGG.Metabolism

Module KEGGRepo

    Public Function RequestKEGGcompounds() As Compound()
        Const url As String = "http://query.biodeep.cn/kegg/repository/compounds"

        Return SingletonHolder(Of BioDeepSession).Instance _
            .RequestStream(url) _
            .DoCall(AddressOf KEGGCompoundPack.ReadKeggDb)
    End Function

    Public Function RequestKEGGReactions() As ReactionClass()
        Const url As String = "http://query.biodeep.cn/kegg/repository/reactions"

        Return SingletonHolder(Of BioDeepSession).Instance _
            .RequestStream(url) _
            .DoCall(AddressOf ReactionClassPack.ReadKeggDb)
    End Function
End Module
