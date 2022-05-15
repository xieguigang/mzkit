Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.BioDeep.MSEngine.Mummichog
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("Mummichog")>
Module Mummichog

    <ExportAPI("peakList_annotation")>
    Public Function PeakListAnnotation(background As list, candidates As MzSet(),
                                       Optional minhit As Integer = 3,
                                       Optional permutation As Integer = 100,
                                       Optional modelSize As Integer = -1,
                                       Optional env As Environment = Nothing) As Object

        Dim models As New List(Of NamedValue(Of NetworkGraph))
        Dim graph As NamedValue(Of NetworkGraph)
        Dim slot As list

        For Each name As String In background.getNames
            slot = background.getByName(name)
            graph = New NamedValue(Of NetworkGraph) With {
                .Name = name,
                .Description = slot.getValue({"desc", "description"}, env, "NA"),
                .Value = slot.getValue(Of NetworkGraph)({"model", "background", "graph"}, env)
            }
            models.Add(graph)
        Next

        Dim result = candidates.PeakListAnnotation(
            background:=models,
            minhit:=minhit,
            permutation:=permutation,
            modelSize:=modelSize
        ) _
            .ToArray

        Return result
    End Function

    <ExportAPI("queryCandidateSet")>
    <RApiReturn(GetType(MzSet))>
    Public Function queryCandidateSet(mz As Double(), msData As Object, Optional env As Environment = Nothing) As Object
        If msData Is Nothing Then
            Return Internal.debug.stop("the given ms compound annotation repository can not be nothing!", env)
        ElseIf msData.GetType.ImplementInterface(Of IMzQuery) Then
            Return DirectCast(msData, IMzQuery).GetCandidateSet(peaks:=mz).ToArray
        Else
            Return Message.InCompatibleType(GetType(IMzQuery), msData.GetType, env)
        End If
    End Function

    <ExportAPI("kegg_background")>
    Public Function CreateKEGGBackground(maps As Map(), reactions As Reaction()) As list
        Dim networkIndex = reactions _
            .GroupBy(Function(r) r.ID) _
            .ToDictionary(Function(r) r.Key,
                          Function(r)
                              Return r.First
                          End Function)
        Dim subgraphs = KEGG.CreateBackground(maps, networkIndex).ToArray
        Dim graphSet As New list With {.slots = New Dictionary(Of String, Object)}
        Dim model As list

        For Each graph As NamedValue(Of NetworkGraph) In subgraphs
            model = New list With {
                .slots = New Dictionary(Of String, Object) From {
                    {"name", graph.Name},
                    {"desc", graph.Description},
                    {"model", graph.Value}
                }
            }

            Call graphSet.add(graph.Name, model)
        Next

        Return graphSet
    End Function
End Module
