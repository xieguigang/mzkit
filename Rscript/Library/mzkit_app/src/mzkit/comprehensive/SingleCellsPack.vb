Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive.SingleCells
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("cellsPack")>
Module SingleCellsPack

    <ExportAPI("pack_cells")>
    Public Function PackSingleCells(<RRawVectorArgument>
                                    rawdata As Object,
                                    Optional source_tag As String = Nothing,
                                    Optional env As Environment = Nothing) As Object

        Dim cell_packs As pipeline = pipeline.TryCreatePipeline(Of mzPack)(rawdata, env)

        If cell_packs.isError Then
            Return cell_packs.getError
        End If

        Return cell_packs _
            .populates(Of mzPack)(env) _
            .PackRawData(source_tag)
    End Function

End Module
