#Region "Microsoft.VisualBasic::e803e9dd617b2070538e6c27836b8b4c, MetaDNA\KEGG_MetaDNA\Program.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    ' Module Program
    ' 
    '     Sub: BuildNetwork, Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language.Default
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
        Dim NA As New DefaultValue(Of String)("NA", Function(exp) CStr(exp) = "NULL")

        SyncLock R_server.R
            With R_server.R

                ' network <- list(rxnID, define, reactants, products);
                ' reactants和products都是KEGG的代谢物编号
                Dim network = Rbase.list()

                With New VisualBasic

                    network = Rbase.lapply(
                        x:=reactions,
                        FUN:=Function(reaction)
                                 Dim model = reaction.ReactionModel
                                 Dim name$ = Rbase.c(reaction.CommonNames, stringVector:=True) Or NA

                                 ' 生成一个列表之中的反应过程的摘要模型
                                 ' 相当于网络之中的一个节点
                                 Return Rbase.list(
                                    !rxnID = reaction.ID,
                                    !name = (name),
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
