Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports HTSMatrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.[Object].dataframe
Imports SingleCellMath = BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute.Math
Imports SingleCellMatrix = BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute.PeakMatrix

''' <summary>
''' Single cells metabolomics data processor
''' </summary>
''' 
<Package("SingleCells")>
Module SingleCells

    Sub New()

    End Sub

    Private Function cellStatsTable(ions As SingleCellIonStat(), args As list, env As Environment) As Rdataframe
        Dim table As New Rdataframe With {
           .columns = New Dictionary(Of String, Array),
           .rownames = ions _
               .Select(Function(i) i.mz.ToString("F4")) _
               .ToArray
        }

        Call table.add(NameOf(SingleCellIonStat.mz), ions.Select(Function(i) i.mz))
        Call table.add(NameOf(SingleCellIonStat.cells), ions.Select(Function(i) i.cells))
        Call table.add(NameOf(SingleCellIonStat.maxIntensity), ions.Select(Function(i) i.maxIntensity))
        Call table.add(NameOf(SingleCellIonStat.baseCell), ions.Select(Function(i) i.baseCell))
        Call table.add(NameOf(SingleCellIonStat.Q1Intensity), ions.Select(Function(i) i.Q1Intensity))
        Call table.add(NameOf(SingleCellIonStat.Q2Intensity), ions.Select(Function(i) i.Q2Intensity))
        Call table.add(NameOf(SingleCellIonStat.Q3Intensity), ions.Select(Function(i) i.Q3Intensity))
        Call table.add(NameOf(SingleCellIonStat.RSD), ions.Select(Function(i) i.RSD))

        Return table
    End Function

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

        Return New HTSMatrix With {
            .expression = singleCells.ToArray,
            .sampleID = mzSet _
                .Select(Function(mzi) mzi.ToString("F4")) _
                .ToArray,
            .tag = raw.source
        }
    End Function

    ''' <summary>
    ''' do stats of the single cell metabolomics ions
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="da"></param>
    ''' <param name="parallel"></param>
    ''' <returns></returns>
    <ExportAPI("SCM_ionStat")>
    <RApiReturn(GetType(SingleCellIonStat))>
    Public Function singleCellsIons(raw As mzPack,
                                    Optional da As Double = 0.01,
                                    Optional parallel As Boolean = True) As Object

        Return SingleCellIonStat.DoIonStats(raw, da, parallel).ToArray
    End Function
End Module
