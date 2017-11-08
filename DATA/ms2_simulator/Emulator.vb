Imports System.Runtime.CompilerServices
Imports KCF.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.MassSpectrum.Assembly

''' <summary>
''' Generate insilicon MS/MS data based on the GA and graph theory.
''' </summary>
Public Module Emulator

    <Extension>
    Public Function MolecularFragment(molecule As NetworkGraph, energy As EnergyModel, Optional step% = 100) As LibraryMatrix
        Dim de# = (energy.MaxEnergy - energy.MinEnergy) / [step]
        Dim matrix As LibraryMatrix

        Return (matrix / Max(matrix)) * 100
    End Function

    Public Function MolecularFragment(molecule As KCF, energy As EnergyModel, Optional step% = 100) As LibraryMatrix

    End Function
End Module
