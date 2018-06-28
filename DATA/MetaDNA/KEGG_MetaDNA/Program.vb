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

        Call BuildNetwork(("D:\Resources\GCModeller-CAD-blueprint\KGML\br08201", "D:\smartnucl_integrative\DATA\2017-12-22.MetaReference\KEGG_cpd"), "D:\MassSpectrum-toolkits\DATA\MetaDNA\MetaDNA\data\metaDNA_kegg.rda")

    End Sub

    Sub BuildNetwork(repository As (reaction$, compound$), rda$)
        Dim reactions = (ls - l - r - "*.Xml" <= repository.reaction) _
            .Select(Function(path)
                        Return path.LoadXml(Of Reaction)(stripInvalidsCharacter:=True)
                    End Function)
        Dim compounds = ScanLoad(repository.compound) _
            .GroupBy(Function(c) c.Entry) _
            .ToDictionary(Function(c) c.Key,
                          Function(g)
                              Return g.First
                          End Function)

        ' network <- list(rxnID, define, reactants, products);
        ' reactants和products都是KEGG的代谢物编号
        Dim network = Rbase.list()

        SyncLock R_server.R
            With R_server.R

                With New VisualBasic

                    network = Rbase.lapply(
                        x:=reactions,
                        FUN:=Function(reaction)
                                 Dim model = reaction.ReactionModel

                                 ' 生成一个列表之中的反应过程的摘要模型
                                 ' 相当于网络之中的一个节点
                                 Return Rbase.list(
                                    !rxnID = reaction.ID,
                                    !define = reaction.Definition,
                                    !reactants = Rbase.c(model.Reactants.Keys, stringVector:=True),
                                    !products = Rbase.c(model.Products.Keys, stringVector:=True)
                                 )
                             End Function)
                End With

                !network = network

            End With
        End SyncLock

        Call Rbase.save({"network"}, rda)
    End Sub
End Module
