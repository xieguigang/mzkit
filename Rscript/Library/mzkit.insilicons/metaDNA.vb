
Imports MetaDNA.visual
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime.Internal

<Package("mzkit.metadna")>
Module metaDNA

    <ExportAPI("read.metadna.infer")>
    <RApiReturn(GetType(NetworkGraph))>
    Public Function loadMetaDNAInferNetwork(debugOutput As Object, Optional env As Environment = Nothing) As Object
        If debugOutput Is Nothing Then
            Return Nothing
        ElseIf debugOutput.GetType Is GetType(String) Then
            debugOutput = DirectCast(debugOutput, String).LoadXml(Of Global.MetaDNA.visual.XML)
        End If

        If Not TypeOf debugOutput Is Global.MetaDNA.visual.XML Then
            Return REnv.debug.stop(New InvalidCastException, env)
        End If

        Return DirectCast(debugOutput, Global.MetaDNA.visual.XML).CreateGraph
    End Function
End Module
