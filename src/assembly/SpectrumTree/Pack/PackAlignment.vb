Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Query
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Tree
Imports std = System.Math

Namespace PackLib

    ''' <summary>
    ''' alignment the query spectrum to the reference spectrum
    ''' </summary>
    ''' <remarks>
    ''' the data <see cref="spectrum"/> is the file stream connection
    ''' to the reference database file, spectrum could be get via id
    ''' based on the id elements inside <see cref="libnames"/> list.
    ''' </remarks>
    Public Class PackAlignment : Inherits Ms2Search

        ''' <summary>
        ''' a stream data reader for the reference spectrum
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property spectrum As SpectrumReader
        ''' <summary>
        ''' cutoff of the cos similarity
        ''' </summary>
        Public ReadOnly Property dotcutoff As Double = 0.6
        Public ReadOnly Property libnames As String()
            Get
                Return spectrum.GetAllLibNames.ToArray
            End Get
        End Property

        Sub New(pack As SpectrumReader, Optional dotcutoff As Double = 0.6)
            Call MyBase.New

            ' set the reference spectrum library
            Me.spectrum = pack
            Me.dotcutoff = dotcutoff
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="centroid"></param>
        ''' <param name="mz1"></param>
        ''' <returns>
        ''' populate output all of the possible candidates hits via the
        ''' spectrum alignment operation. All of the hit result that 
        ''' populate out from this function matched the <see cref="dotcutoff"/>
        ''' threshold condition.
        ''' </returns>
        Public Overrides Iterator Function Search(centroid() As ms2, mz1 As Double) As IEnumerable(Of ClusterHit)
            Dim candidates As BlockNode() = spectrum.QueryByMz(mz1).ToArray
            Dim hits As New List(Of ___tmp)

            ' do spectrum alignment for all matched
            ' spectrum candidates
            For Each hit As BlockNode In candidates
                Dim align = GlobalAlignment.CreateAlignment(centroid, hit.centroid, da).ToArray
                Dim score = CosAlignment.GetCosScore(align)
                Dim min = std.Min(score.forward, score.reverse)

                ' if the spectrum cos dotcutoff
                ' matches the cutoff threshold
                ' then we have a candidate hit
                If min > dotcutoff AndAlso spectrum.HasMapName(hit.Id) Then
                    Call hits.Add(New ___tmp With {
                       .id = spectrum(libname:=hit.Id),
                       .hit = hit,
                       .align = align,
                       .forward = score.forward,
                       .reverse = score.reverse
                    })
                End If
            Next

            ' hits may contains multiple metabolite reference data
            ' multiple cluster object should be populates from
            ' this function?
            If hits.Count > 0 Then
                For Each metabolite In hits.GroupBy(Function(i) i.id)
                    Yield reportClusterHit(centroid, hit_group:=metabolite)
                Next
            End If
        End Function

        ''' <summary>
        ''' the temp result data tuple object
        ''' </summary>
        Private Structure ___tmp

            Dim id As String
            Dim hit As BlockNode
            Dim align As SSM2MatrixFragment()
            Dim forward As Double
            Dim reverse As Double

        End Structure

        ''' <summary>
        ''' report the cluster alignment result
        ''' </summary>
        ''' <param name="centroid"></param>
        ''' <param name="hit_group"></param>
        ''' <returns></returns>
        Private Function reportClusterHit(centroid() As ms2, hit_group As IGrouping(Of String, ___tmp)) As ClusterHit
            Dim desc = hit_group.OrderByDescending(Function(n) std.Min(n.forward, n.reverse)).ToArray
            Dim max = desc.First
            Dim forward = desc.Select(Function(n) n.forward).ToArray
            Dim reverse = desc.Select(Function(n) n.reverse).ToArray
            Dim jaccard = desc.Select(Function(n) JaccardAlignment.GetJaccardScore(n.align)).ToArray
            Dim entropy = desc _
                .Select(Function(c)
                            Return SpectralEntropy.calculate_entropy_similarity(centroid, c.hit.centroid, da)
                        End Function) _
                .ToArray

            Return New ClusterHit With {
                .Id = hit_group.Key,
                .forward = forward.Average,
                .reverse = reverse.Average,
                .ClusterEntropy = entropy,
                .entropy = entropy.Average,
                .ClusterForward = forward,
                .ClusterReverse = reverse,
                .ClusterJaccard = jaccard,
                .jaccard = jaccard.Average,
                .ClusterId = desc.Select(Function(i) i.hit.Id).ToArray,
                .ClusterRt = desc.Select(Function(i) i.hit.rt).ToArray,
                .representive = max.align
            }
        End Function
    End Class
End Namespace