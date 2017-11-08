Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.Chemistry
Imports SMRUCC.Chemistry.Model.Graph
Imports SMRUCC.MassSpectrum.Assembly

''' <summary>
''' Generate insilicon MS/MS data based on the GA and graph theory.
''' </summary>
Public Module Emulator

    <Extension>
    Public Function MolecularFragment(molecule As NetworkGraph, energy As EnergyModel, Optional step% = 100) As LibraryMatrix
        Dim de# = (energy.MaxEnergy - energy.MinEnergy) / [step]
        Dim quantity As New Dictionary(Of Double, Double) ' {mz, quantity}

        For e As Double = energy.MinEnergy To energy.MaxEnergy Step de
            ' 将所有能量值低于e的化学键都打断
            ' 则完整的分子图会分裂为多个子图碎片
            Dim percentage# = energy.PercentageLess(e)


        Next

        Dim matrix As New LibraryMatrix With {
            .ms2 = quantity _
                .Select(Function(frag)
                            Return New ms2 With {
                                .mz = frag.Key,
                                .quantity = frag.Value
                            }
                        End Function) _
                .ToArray
        }

        Return (matrix / Max(matrix)) * 100
    End Function

    <Extension>
    Public Function MolecularFragment(molecule As Model.KCF, energy As EnergyModel, Optional step% = 100) As LibraryMatrix
        Return molecule.Graph.MolecularFragment(energy, [step])
    End Function
End Module
