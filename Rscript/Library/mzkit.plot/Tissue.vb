
Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("tissue")>
Module Tissue

    <ExportAPI("scan_tissue")>
    Public Function scanTissue(tissue As Image, Optional colors As String() = Nothing) As Cell()
        Return HistologicalImage.GridScan(target:=tissue, colors:=colors).ToArray
    End Function

    <ExportAPI("heatmap_layer")>
    Public Function getTargets(tissue As Cell(),
                               Optional heatmap As Layers = Layers.Density,
                               Optional target As String = "black") As PixelData()

        Return tissue.GetHeatMapLayer(heatmap, target)
    End Function
End Module
