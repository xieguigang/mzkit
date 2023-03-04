
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.Raman
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("RamanSpectroscopy")>
Public Module RamanSpectroscopy

    <ExportAPI("readFile")>
    Public Function readFile(<RRawVectorArgument> file As Object,
                             Optional env As Environment = Nothing) As Object

        Dim buf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If buf Like GetType(Message) Then
            Return buf.TryCast(Of Message)
        End If

        Return FileReader.ParseTextFile(New StreamReader(buf))
    End Function
End Module
