Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml

Namespace PoolData

    ''' <summary>
    ''' a spectrum cluster node collection proxy
    ''' </summary>
    Public MustInherit Class MetadataProxy

        Default Public MustOverride ReadOnly Property GetById(id As String) As Metadata
        Public MustOverride ReadOnly Property AllClusterMembers As IEnumerable(Of Metadata)
        Public MustOverride ReadOnly Property Depth As Integer

        ''' <summary>
        ''' the root spectrum id in current cluster object
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride ReadOnly Property RootId As String

        Public MustOverride Sub Add(id As String, metadata As Metadata)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="score"></param>
        ''' <param name="align">
        ''' the root spectrum to current cluster node is nothing, due 
        ''' to the reason of no spectrum compares to it
        ''' </param>
        ''' <param name="pval"></param>
        Public MustOverride Sub Add(id As String, score As Double, align As AlignmentOutput, pval As Double)

        Public MustOverride Function HasGuid(id As String) As Boolean
        Public MustOverride Sub SetRootId(hashcode As String)

    End Class

End Namespace