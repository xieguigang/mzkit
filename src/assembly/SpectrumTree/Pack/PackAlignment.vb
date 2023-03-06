Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Query
Imports stdNum = System.Math

Public Class PackAlignment : Inherits Ms2Search

    ReadOnly spectrum As SpectrumReader

    ''' <summary>
    ''' cutoff of the cos similarity
    ''' </summary>
    Dim dotcutoff As Double = 0.6

    Sub New(pack As SpectrumReader, Optional dotcutoff As Double = 0.6)
        Call MyBase.New

        ' set the reference spectrum library
        Me.spectrum = pack
        Me.dotcutoff = dotcutoff
    End Sub

    Public Overrides Iterator Function Search(centroid() As ms2, mz1 As Double) As IEnumerable(Of ClusterHit)
        Dim candidates = spectrum.QueryByMz(mz1).ToArray
        Dim hits As New List(Of (BlockNode, align As SSM2MatrixFragment(), cos_forward As Double, cos_reverse As Double))

        For Each hit As BlockNode In candidates
            Dim align = GlobalAlignment.CreateAlignment(centroid, hit.centroid, da).ToArray
            Dim score = CosAlignment.GetCosScore(align)
            Dim min = stdNum.Min(score.forward, score.reverse)

            If min > dotcutoff Then
                Call hits.Add((hit, align, score.forward, score.reverse))
            End If
        Next

        ' hits may contains multiple metabolite reference data
        ' multiple cluster object should be populates from
        ' this function?
        If hits.Count > 0 Then
            Yield reportClusterHit(centroid, hits:=hits)
        End If
    End Function

    Private Function reportClusterHit(centroid() As ms2, hits As List(Of (BlockNode, align As SSM2MatrixFragment(), cos_forward As Double, cos_reverse As Double))) As ClusterHit
        Dim max = hits.OrderByDescending(Function(n) stdNum.Min(n.cos_forward, n.cos_reverse)).First


    End Function
End Class
