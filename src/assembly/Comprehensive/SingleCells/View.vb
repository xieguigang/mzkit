Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace SingleCells

    Public Module View

        <Extension>
        Public Iterator Function ResolveSingleCells(raw As mzPack) As IEnumerable(Of UMAPPoint)
            For Each cell As ScanMS1 In raw.MS
                Dim meta As StringReader = StringReader.WrapDictionary(cell.meta)
                Dim cluster As String = meta.GetString("cluster")

                Yield New UMAPPoint() With {
                    .[class] = cluster,
                    .label = cell.scan_id,
                    .x = meta.GetDouble("umap1"),
                    .y = meta.GetDouble("umap2"),
                    .z = meta.GetDouble("umap3")
                }
            Next
        End Function
    End Module
End Namespace