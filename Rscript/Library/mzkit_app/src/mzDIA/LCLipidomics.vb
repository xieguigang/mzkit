Imports BioNovoGene.Analytical.MassSpectrometry.Lipidomics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' Lipidomics annotation based on MS-DIAL
''' </summary>
<Package("lipidomics")>
Module LCLipidomics

    Public Function GetLipidIons(lipidclass As LbmClass,
                                adduct As AdductIon,
                                minCarbonCount As Integer,
                                maxCarbonCount As Integer,
                                minDoubleBond As Integer,
                                maxDoubleBond As Integer,
                                maxOxygen As Integer) As LipidIon()

        Dim precursor_type As AdductIon

        Return LipidMassLibraryGenerator.GetIons(lipidclass, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen).ToArray
    End Function
End Module
