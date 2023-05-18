Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

Public Module MergeFakeSTImagingSliders

    ''' <summary>
    ''' merge the STImaging slide data files in linear
    ''' </summary>
    ''' <param name="samples"></param>
    ''' <param name="relativePos"></param>
    ''' <param name="padding"></param>
    ''' <param name="norm"></param>
    ''' <param name="println"></param>
    ''' <returns>
    ''' generated a new faked ms-imaging data object for 10x genomics STImaging data 
    ''' </returns>
    Public Function JoinSTImagingSamples(samples As IEnumerable(Of mzPack),
                                         Optional relativePos As Boolean = True,
                                         Optional padding As Integer = 20,
                                         Optional norm As Boolean = True,
                                         Optional println As Action(Of String) = Nothing) As mzPack

        ' load polygon shape for each imaging slider
        Dim polygons = samples.PullPolygons(println).ToArray
        Dim unionGeneIds As Index(Of String) = polygons _
            .Select(Function(m) m.ms.Annotations.Values) _
            .IteratesALL _
            .Distinct _
            .Indexing
        Dim mergeSt As New MergeSTSlides(relativePos, norm, println, unionGeneIds)
        Dim union As mzPack = MergeSliders.JoinMSISamples(samples, relativePos, padding, norm, println, mergeSt)
        Dim res As Double = 55
        Dim layer_annotations As New Dictionary(Of String, String)

        For Each map As KeyValuePair(Of String, Integer) In unionGeneIds.Map
            Call layer_annotations.Add(map.Value.ToString, map.Key)
        Next

        If Not polygons.Any(Function(m) m.ms.metadata.ContainsKey("resolution")) Then
            union.metadata("resolution") = res
        End If

        ' set the gene idset
        union.Annotations = layer_annotations

        Return union
    End Function
End Module
