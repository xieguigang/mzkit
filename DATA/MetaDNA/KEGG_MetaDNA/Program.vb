Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

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
    End Sub
End Module
