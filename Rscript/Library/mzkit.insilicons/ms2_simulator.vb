Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Insilicon
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry.Model
Imports BioNovoGene.BioDeep.Chemistry.Model.Graph
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("mzkit.simulator")>
Module ms2_simulator

    <ExportAPI("read.kcf")>
    Public Function loadKCF(file As String) As KCF
        Return file.LoadKCF(False)
    End Function

    <ExportAPI("molecular.graph")>
    Public Function MolecularGraph(mol As KCF, Optional verbose As Boolean = False) As NetworkGraph
        Dim g As NetworkGraph = mol _
           .CreateGraph _
           .FillBoundEnergy(New BoundEnergyFinder)

        If verbose Then
            Call Console.WriteLine($"energy range: {energyRange(g).ToString}")
        End If

        Return g
    End Function

    <ExportAPI("energy.range")>
    Public Function energyRange(mol As NetworkGraph) As DoubleRange
        Dim energies As Double() = mol.graphEdges _
            .Select(Function(e) e.data.weight) _
            .ToArray

        Return energies
    End Function

    <ExportAPI("fragmentation")>
    Public Function MolecularFragmentation(mol As NetworkGraph, energy As EnergyModel,
                                           Optional nIntervals% = 10,
                                           Optional precision% = 4) As LibraryMatrix
        Return mol.MolecularFragment(energy, nIntervals, precision)
    End Function

    <ExportAPI("energy.normal")>
    Public Function energyModel_normalDist(mu#, delta#, Optional max# = 1000) As EnergyModel
        Return New EnergyModel(Function(x)
                                   Return pnorm.ProbabilityDensity(x, mu, delta)
                               End Function, 0, max)
    End Function

    <ExportAPI("write.mgf")>
    Public Function writeMgf(fragments As LibraryMatrix, file$) As Boolean
        Using mgf As StreamWriter = file.OpenWriter
            Call fragments.MgfIon.WriteAsciiMgf(mgf)
        End Using

        Return True
    End Function
End Module
