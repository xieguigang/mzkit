Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace SingleCells

    Public Module View

        <Extension>
        Public Function ResolveSingleCells(raw As mzPack) As IEnumerable(Of UMAPPoint)
            Return raw.MS.AsParallel.Select(Function(cell) LoadScanMeta(cell))
        End Function

        <Extension>
        Public Function LoadScanMeta(cell As ScanMS1) As UMAPPoint
            Dim meta As StringReader = StringReader.WrapDictionary(cell.meta)
            Dim cluster As String = meta.GetString("cluster")

            Return New UMAPPoint() With {
                .[class] = cluster,
                .label = cell.scan_id,
                .x = meta.GetDouble("umap1"),
                .y = meta.GetDouble("umap2"),
                .z = meta.GetDouble("umap3")
            }
        End Function
    End Module
End Namespace