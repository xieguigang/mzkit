Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' a helper module for create the spectrum tree alignment
''' </summary>
Public Class NetworkingTree

    ReadOnly cosine As CosAlignment
    ReadOnly align As MSScoreGenerator
    ReadOnly equals_cutoff As Double = 0.85

    Sub New(Optional mzdiff As Double = 0.3,
            Optional intocutoff As Double = 0.05,
            Optional equals As Double = 0.85)

        cosine = New CosAlignment(
            mzwidth:=Tolerance.DeltaMass(mzdiff),
            intocutoff:=New RelativeIntensityCutoff(intocutoff)
        )
        align = New MSScoreGenerator(cosine, equals, equals)
        equals_cutoff = equals
    End Sub

    ''' <summary>
    ''' clear the spectrum cache
    ''' </summary>
    ''' <returns></returns>
    Public Function Clear() As NetworkingTree
        Call align.Clear()
        Return Me
    End Function

    ''' <summary>
    ''' do spectrum tree alignment
    ''' </summary>
    ''' <param name="ions"></param>
    ''' <returns></returns>
    Public Function Tree(ions As IEnumerable(Of PeakMs2)) As TreeCluster
        Dim ionsList As New List(Of PeakMs2)
        Dim clustering As New ClusterTree

        For Each ion As PeakMs2 In ions.SafeQuery
            Call ionsList.Add(ion)
            Call ClusterTree.Add(clustering, ion.lib_guid, align, threshold:=equals_cutoff)
        Next

        Return New TreeCluster With {
            .tree = clustering,
            .spectrum = ionsList.ToArray
        }
    End Function
End Class
