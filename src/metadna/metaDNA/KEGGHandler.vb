Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Public Class KEGGHandler

    ReadOnly precursorTypes As MzCalculator()
    ReadOnly tolerance As Tolerance
    ReadOnly keggCompounds As Compound()

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <returns>
    ''' 函数返回符合条件的kegg代谢物编号
    ''' </returns>
    Public Iterator Function QueryByMz(mz As Double) As IEnumerable(Of String)

    End Function

End Class
