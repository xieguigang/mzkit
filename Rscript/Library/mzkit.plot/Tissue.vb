
Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("tissue")>
Module Tissue

    <ExportAPI("scan_tissue")>
    Public Function scanTissue(tissue As Image) As Cell()
        Return HistologicalImage.GridScan(target:=tissue).ToArray
    End Function
End Module
