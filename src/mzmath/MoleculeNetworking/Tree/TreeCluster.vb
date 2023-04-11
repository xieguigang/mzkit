Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.DataMining.BinaryTree

''' <summary>
''' A tuple object that wrap the <see cref="ClusterTree"/> and
''' spectrum data <see cref="PeakMs2"/>.
''' </summary>
Public Class TreeCluster

    Public Property tree As ClusterTree
    Public Property spectrum As PeakMs2()

End Class
