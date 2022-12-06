Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
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
    Public Function cellMatrix(raw As mzPack, Optional mzdiff As Double = 0.005, Optional env As Environment = Nothing) As Object


    End Function
End Module
