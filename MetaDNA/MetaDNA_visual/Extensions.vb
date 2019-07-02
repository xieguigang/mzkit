
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream

<Extension, HideModuleName> Public Module Extensions

    ''' <summary>
    ''' Dump the infer network as network csv table for file saved
    ''' </summary>
    ''' <param name="metaDNA"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function TranslateAsTable(metaDNA As XML) As NetworkTables
        Return metaDNA.CreateGraph.Tabular
    End Function
End Module
