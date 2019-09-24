
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts

<Extension, HideModuleName> Public Module Extensions

    ''' <summary>
    ''' Dump the infer network as network csv table for file saved
    ''' </summary>
    ''' <param name="metaDNA"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function TranslateAsTable(metaDNA As XML, Optional applyLayout As Boolean = False) As NetworkTables
        Return metaDNA.CreateGraph.GraphTable(applyLayout)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function GraphTable(metaDNA As NetworkGraph, Optional applyLayout As Boolean = False) As NetworkTables
        If applyLayout Then
            Call metaDNA.doForceLayout(iterations:=2000)
        End If
        Return metaDNA.Tabular({"candidates", "intensity", "infer.depth", "score.forward", "score.reverse"})
    End Function
End Module
