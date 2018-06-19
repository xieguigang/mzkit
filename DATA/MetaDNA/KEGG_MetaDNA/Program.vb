Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports R_server = RDotNet.Extensions.VisualBasic.RSystem
Imports Rbase = RDotNet.Extensions.VisualBasic.API.base
Imports VisualBasic = Microsoft.VisualBasic.Language.Runtime

Module Program

    ''' <summary>
    ''' 生成MetaDNA进行计算所需要的数据包
    ''' </summary>
    Sub Main()

    End Sub

    Sub BuildNetwork(repository As (reaction$, compound$), rda$)
        Dim reactions = (ls - l - r - "*.Xml" <= repository.reaction).Select(AddressOf LoadXml(Of Reaction))
        Dim compounds = ScanLoad(repository.compound) _
            .GroupBy(Function(c) c.Entry) _
            .ToDictionary(Function(c) c.Key,
                          Function(g)
                              Return g.First
                          End Function)

        ' network <- list(rxnID, reactants, products);
        Dim network = Rbase.list()

        SyncLock R_server.R
            With R_server.R

                With New VisualBasic

                    network = Rbase.lapply(
                        x:=reactions,
                        FUN:=Function(reaction)
                                 Dim model = reaction.ReactionModel

                                 Return Rbase.list(
                                    !rxnID = reaction.ID,
                                    !define = reaction.Definition,
                                    !reactants = Rbase.c(model.Reactants.Keys, stringVector:=True),
                                    !products = Rbase.c(model.Products.Keys, stringVector:=True)
                                 )
                             End Function)

                End With

            End With
        End SyncLock
    End Sub
End Module
