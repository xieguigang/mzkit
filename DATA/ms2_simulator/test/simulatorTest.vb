Imports BioNovoGene.BioDeep.Chemistry.Model
Imports BioNovoGene.BioDeep.Chemistry.Model.Graph
Imports ms2_simulator

Module simulatorTest

    Sub Main()
        Dim boundEnergies = BoundEnergy.GetEnergyTable
        Dim molecule = "E:\MassSpectrum-toolkits\visual\KCF\DATA\NADPH.txt".LoadKCF.CreateGraph
        Dim energy As New EnergyModel(Function(x, y)
                                          Return x
                                      End Function, 0, 1000)
        Dim result = molecule.MolecularFragment(energy)

        Pause()
    End Sub
End Module
