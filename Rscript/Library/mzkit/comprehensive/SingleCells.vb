Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.Rsharp.Runtime
Imports SingleCellMath = BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute.Math
Imports SingleCellMatrix = BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute.PeakMatrix

''' <summary>
''' Single cells metabolomics data processor
''' </summary>
''' 
<Package("SingleCells")>
Module SingleCells

    ''' <summary>
    ''' export single cell expression matrix from the raw data scans
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="mzdiff"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("cell_matrix")>
    Public Function cellMatrix(raw As mzPack,
                               Optional mzdiff As Double = 0.005,
                               Optional freq As Double = 0.001,
                               Optional env As Environment = Nothing) As Object

        Dim singleCells As New List(Of DataFrameRow)
        Dim mzSet As Double() = SingleCellMath.GetMzIndex(raw:=raw, mzdiff:=mzdiff, freq:=freq)

        For Each cell_scan As DataFrameRow In SingleCellMatrix.ExportScans(Of DataFrameRow)(raw, mzSet)
            cell_scan.geneID = cell_scan.geneID _
                .Replace("[MS1]", "") _
                .Trim
            singleCells.Add(cell_scan)
        Next

        Return New Matrix With {
            .expression = singleCells.ToArray,
            .sampleID = mzSet _
                .Select(Function(mzi) mzi.ToString("F4")) _
                .ToArray,
            .tag = raw.source
        }
    End Function

    <ExportAPI("SCMionStat")>
    Public Function singleCellsIons(raw As mzPack) As Object

    End Function
End Module
