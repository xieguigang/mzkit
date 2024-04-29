#Region "Microsoft.VisualBasic::a8b1a5f16b57b1f44bc95feb9a31ac52, E:/mzkit/src/metadna/metaDNA//MSJointConnection.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 258
    '    Code Lines: 207
    ' Comment Lines: 26
    '   Blank Lines: 25
    '     File Size: 10.53 KB


    ' Class MSJointConnection
    ' 
    '     Properties: allClusters
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetAnnotation, GetCompound, GetDbXref, getEnrichedMzSet, (+2 Overloads) GetEnrichment
    '               GetGSEABackground, GetMetadata, (+2 Overloads) ImportsBackground, QueryByMz, SetAnnotation
    '               (+2 Overloads) toClusters
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML

''' <summary>
''' the kegg database annotation search helper
''' </summary>
Public Class MSJointConnection : Implements IMzQuery

    ReadOnly kegg As CompoundSolver

    ''' <summary>
    ''' the GSEA background
    ''' </summary>
    ReadOnly jointSet As Background

    Public ReadOnly Property allClusters As String()
        Get
            Return jointSet.clusters.Select(Function(c) c.ID).ToArray
        End Get
    End Property

    Sub New(kegg As CompoundSolver, peakSet As Background)
        Me.kegg = kegg
        Me.jointSet = peakSet
    End Sub

    Public Function GetGSEABackground() As Background
        Return jointSet
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetCompound(kegg_id As String) As Compound
        Return DirectCast(kegg.GetCompound(kegg_id), KEGGCompound).KEGG
    End Function

    Public Function GetEnrichment(id As IEnumerable(Of String)) As EnrichmentResult()
        Return jointSet _
           .Enrichment(id, showProgress:=False) _
           .OrderBy(Function(d) d.pvalue) _
           .ToArray
    End Function

    Public Function GetEnrichment(mz As Double(), Optional ByRef allId As Dictionary(Of String, MzQuery()) = Nothing) As EnrichmentResult()
        Dim allIdList As Dictionary(Of String, MzQuery()) = mz _
            .Select(AddressOf kegg.QueryByMz) _
            .IteratesALL _
            .GroupBy(Function(cid) cid.unique_id) _
            .ToDictionary(Function(cid) cid.Key,
                          Function(cid)
                              Return cid.ToArray
                          End Function)
        Dim enrichment As EnrichmentResult() = GetEnrichment(allIdList.Keys)

        allId = allIdList

        Return enrichment
    End Function

    Private Function getEnrichedMzSet(mz As Double(), topN As Integer) As IGrouping(Of String, MzQuery)()
        Dim allId As Dictionary(Of String, MzQuery()) = Nothing
        Dim enrichment As EnrichmentResult() = GetEnrichment(mz, allId).Take(topN).ToArray
        Dim mzSet As IGrouping(Of String, MzQuery)() = enrichment _
            .Select(Function(list)
                        Dim score As Double = -Math.Log10(list.pvalue)
                        Dim result = list.geneIDs _
                            .Select(Function(id) allId(id)) _
                            .IteratesALL _
                            .ToArray
                        Dim copy = result _
                            .Select(Function(q)
                                        Return New MzQuery With {
                                            .score = score,
                                            .precursorType = q.precursorType,
                                            .unique_id = q.unique_id,
                                            .mz = q.mz,
                                            .ppm = q.ppm,
                                            .mz_ref = q.mz_ref,
                                            .name = q.name
                                        }
                                    End Function) _
                            .ToArray

                        Return copy
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(c)
                         Return c.mz.ToString("F4")
                     End Function) _
            .ToArray

        Return mzSet
    End Function

    Public Function QueryByMz(mz As Double) As IEnumerable(Of MzQuery) Implements IMzQuery.QueryByMz
        Return kegg.QueryByMz(mz)
    End Function

    ''' <summary>
    ''' MS1 peak list annotation
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="topN"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' combine of the enriched result and un-enriched results.
    ''' 
    ''' + enriched result: high score
    ''' + un-enriched result: zero score
    ''' 
    ''' </remarks>
    Public Function SetAnnotation(mz As IEnumerable(Of Double), Optional topN As Integer = 3) As IEnumerable(Of MzQuery) Implements IMzQuery.MSetAnnotation
        Dim allMz As Double() = mz.ToArray
        Dim mzSet = getEnrichedMzSet(allMz, topN)
        Dim annotation As MzQuery() = mzSet _
            .Select(Function(mzi)
                        Return mzi _
                            .GroupBy(Function(i) i.unique_id) _
                            .Select(Function(m)
                                        Return New MzQuery With {
                                            .unique_id = m.Key,
                                            .mz = Double.Parse(mzi.Key),
                                            .ppm = m.First.ppm,
                                            .precursorType = m.First.precursorType,
                                            .name = m.First.name,
                                            .mz_ref = m.First.mz_ref,
                                            .score = Aggregate hit As MzQuery
                                                     In m
                                                     Into Sum(hit.score)
                                        }
                                    End Function) _
                            .OrderByDescending(Function(d) d.score) _
                            .First
                    End Function) _
            .ToArray
        Dim allIdList As MzQuery() = allMz _
            .Select(AddressOf kegg.QueryByMz) _
            .IteratesALL _
            .ToArray
        Dim unique = annotation _
            .JoinIterates(allIdList) _
            .GroupBy(Function(a) a.unique_id) _
            .Select(Function(cid)
                        Return cid _
                            .OrderByDescending(Function(d) d.score) _
                            .First
                    End Function) _
            .ToArray

        Return unique
    End Function

    Public Shared Function ImportsBackground(maps As IEnumerable(Of Pathway)) As Background
        Return New Background With {
            .clusters = toClusters(maps).ToArray,
            .size = .clusters _
                .Select(Function(c) c.members) _
                .IteratesALL _
                .Select(Function(c) c.accessionID) _
                .Distinct _
                .Count
        }
    End Function

    Public Shared Function ImportsBackground(maps As IEnumerable(Of Map)) As Background
        Return New Background With {
            .clusters = toClusters(maps).ToArray,
            .size = .clusters _
                .Select(Function(c) c.members) _
                .IteratesALL _
                .Select(Function(c) c.accessionID) _
                .Distinct _
                .Count
        }
    End Function

    Private Shared Iterator Function toClusters(maps As IEnumerable(Of Pathway)) As IEnumerable(Of Cluster)
        For Each map As Pathway In maps
            Yield New Cluster With {
                .description = map.description,
                .ID = map.EntryId,
                .names = map.name,
                .members = map.compound _
                    .Select(Function(c)
                                Return New BackgroundGene With {
                                    .accessionID = c.name,
                                    .name = c.text,
                                    .[alias] = {c.name},
                                    .locus_tag = c,
                                    .term_id = BackgroundGene.UnknownTerms(c.name).ToArray
                                }
                            End Function) _
                    .ToArray
            }
        Next
    End Function

    Private Shared Iterator Function toClusters(maps As IEnumerable(Of Map)) As IEnumerable(Of Cluster)
        For Each map As Map In From pwy In maps.SafeQuery Where Not pwy Is Nothing
            Yield New Cluster With {
                .description = map.description,
                .ID = map.EntryId,
                .names = map.name,
                .members = map _
                    .GetMembers _
                    .Where(Function(id)
                               Return id.IsPattern("C\d+")
                           End Function) _
                    .Distinct _
                    .Select(Function(id)
                                Return New BackgroundGene With {
                                    .accessionID = id,
                                    .[alias] = {id},
                                    .locus_tag = New NamedValue With {
                                        .name = id,
                                        .text = id
                                    },
                                    .name = id,
                                    .term_id = BackgroundGene.UnknownTerms(id).ToArray
                                }
                            End Function) _
                    .ToArray
            }
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetAnnotation(uniqueId As String) As (name As String, formula As String) Implements IMzQuery.GetAnnotation
        Return kegg.GetAnnotation(uniqueId)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="uniqueId"></param>
    ''' <returns>
    ''' <see cref="Compound"/>
    ''' </returns>
    Public Function GetMetadata(uniqueId As String) As Object Implements IMzQuery.GetMetadata
        Return DirectCast(kegg.GetCompound(uniqueId), KEGGCompound).KEGG
    End Function

    Public Function GetDbXref(uniqueId As String) As Dictionary(Of String, String) Implements IMzQuery.GetDbXref
        Dim compound As Compound = GetMetadata(uniqueId)

        If compound Is Nothing Then
            Return New Dictionary(Of String, String)
        Else
            Return New Dictionary(Of String, String) From {
                {"kegg", compound.entry}
            }
        End If
    End Function
End Class
