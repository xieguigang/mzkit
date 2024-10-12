Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' Spectrum binary tree clustering helper
''' </summary>
Public Class BinaryClustering

    Friend ReadOnly align As MSScoreGenerator
    Friend ReadOnly equals_cutoff As Double = 0.85

    Sub New(Optional mzdiff As Double = 0.3,
            Optional intocutoff As Double = 0.05,
            Optional equals As Double = 0.85,
            Optional interval As Double = 0.1)

        align = NetworkingTree.CreateAlignment(mzdiff, intocutoff, equals)
        equals_cutoff = equals
    End Sub

    ''' <summary>
    ''' clear the spectrum cache
    ''' </summary>
    ''' <returns></returns>
    Public Function Clear() As BinaryClustering
        Call align.Clear()
        Return Me
    End Function

    ''' <summary>
    ''' Just add the spectrum data to memory cache
    ''' </summary>
    ''' <param name="spec"></param>
    Public Sub Add(spec As PeakMs2)
        Call align.Add(spec)
    End Sub

    ''' <summary>
    ''' do spectrum tree alignment
    ''' </summary>
    ''' <param name="ions"></param>
    ''' <returns></returns>
    Public Function Tree(ions As IEnumerable(Of PeakMs2)) As BinaryClustering
        Dim uniqueIds As New List(Of String)

        For Each spectrum As PeakMs2 In ions.SafeQuery
            Call align.Add(spectrum)
            Call uniqueIds.Add(spectrum.lib_guid)
        Next

        Dim bin As BTreeCluster = BuildTree.BTreeCluster(uniqueIds, align)

    End Function
End Class
