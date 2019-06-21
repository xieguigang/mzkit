Imports BioNovoGene.BioDeep.Chemistry.Model
Imports BioNovoGene.BioDeep.Chemistry.Model.Graph
Imports ms2_simulator
Imports SMRUCC.MassSpectrum.Math.Spectra

Module simulatorTest

    Sub Main()
        Dim boundEnergies = BoundEnergy.GetEnergyTable
        Dim boundTable As New BoundEnergyFinder

        Dim molecule = "D:\MassSpectrum-toolkits\visual\KCF\DATA\NADPH.txt".LoadKCF.CreateGraph.FillBoundEnergy(boundTable)
        Dim energy As New EnergyModel(Function(x, y)
                                          Return x
                                      End Function, 0, 1000)
        Dim result As LibraryMatrix = molecule.MolecularFragment(energy)



        Pause()
    End Sub
End Module
