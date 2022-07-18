Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.DataMining.BinaryTree

Public Module Networking

    <Extension>
    Public Function Tree(ions As IEnumerable(Of PeakMs2),
                         Optional mzdiff As Double = 0.3,
                         Optional intocutoff As Double = 0.05,
                         Optional equals As Double = 0.85) As ClusterTree

        Dim align As New MSScore(New CosAlignment(Tolerance.DeltaMass(mzdiff), New RelativeIntensityCutoff(intocutoff)), ions, equals, equals)
        Dim clustering As New ClusterTree

        For Each ion As PeakMs2 In align.Ions
            Call ClusterTree.Add(clustering, ion.lib_guid, align, threshold:=equals)
        Next

        Return clustering
    End Function
End Module
