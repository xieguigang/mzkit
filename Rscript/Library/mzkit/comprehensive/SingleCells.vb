Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.IndexedCache
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.Rsharp.Runtime

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

        Dim mzSet As Double() = MzMatrix.GetMzIndex(raw:=raw, mzdiff:=mzdiff, freq:=freq)
        Dim mzIndex As New BlockSearchFunction(Of (mz As Double, Integer))(
            data:=mzSet.Select(Function(mzi, i) (mzi, i)),
            eval:=Function(i) i.mz,
            tolerance:=1,
            fuzzy:=True
        )
        Dim singleCells As New List(Of DataFrameRow)
        Dim len As Integer = mzSet.Length

        For Each scan As ScanMS1 In raw.MS
            Dim cellId As String = scan.scan_id
            Dim v As Double() = MzMatrix.DeconvoluteScan(scan.mz, scan.into, len, mzIndex)
            Dim cell_scan As New DataFrameRow With {
                .experiments = v,
                .geneID = cellId
            }

            Call singleCells.Add(cell_scan)
        Next

        Return New Matrix With {
            .expression = singleCells.ToArray,
            .sampleID = mzSet _
                .Select(Function(mzi) mzi.ToString("F4")) _
                .ToArray,
            .tag = raw.source
        }
    End Function
End Module
