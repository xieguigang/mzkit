Imports BioNovoGene.Analytical.MassSpectrometry.Lipidomics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' Lipidomics annotation based on MS-DIAL
''' </summary>
<Package("lipidomics")>
Module LCLipidomics

    ''' <summary>
    ''' meansrue lipid ions
    ''' </summary>
    ''' <param name="lipidclass">configs of the target lipid <see cref="LbmClass"/> for run spectrum peaks generation</param>
    ''' <param name="adduct"></param>
    ''' <param name="minCarbonCount"></param>
    ''' <param name="maxCarbonCount"></param>
    ''' <param name="minDoubleBond"></param>
    ''' <param name="maxDoubleBond"></param>
    ''' <param name="maxOxygen"></param>
    ''' <returns>a collection of the <see cref="LipidIon"/> data</returns>
    <ExportAPI("lipid_ions")>
    <RApiReturn(GetType(LipidIon))>
    Public Function GetLipidIons(lipidclass As LbmClass,
                                 adduct As AdductIon,
                                 minCarbonCount As Integer,
                                 maxCarbonCount As Integer,
                                 minDoubleBond As Integer,
                                 maxDoubleBond As Integer,
                                 maxOxygen As Integer) As Object

        Dim precursor_type As AdductIon

        Return LipidMassLibraryGenerator _
            .GetIons(lipidclass, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen) _
            .ToArray
    End Function
End Module
