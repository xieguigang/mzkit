Imports BioNovoGene.Analytical.MassSpectrometry.Math.MoleculeNetworking
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("MoleculeNetworking")>
Module MoleculeNetworking

    <ExportAPI("tree")>
    Public Function Tree(ions As PeakMs2(),
                         Optional mzdiff As Double = 0.3,
                         Optional intocutoff As Double = 0.05,
                         Optional equals As Double = 0.85) As ClusterTree

        Return ions.Tree(mzdiff, intocutoff, equals)
    End Function
End Module
