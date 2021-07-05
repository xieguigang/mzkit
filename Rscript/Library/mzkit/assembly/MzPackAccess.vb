Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("mzPack")>
Module MzPackAccess

    <ExportAPI("mzpack")>
    <RApiReturn(GetType(mzPackReader))>
    Public Function open_mzpack(file As Object, Optional env As Environment = Nothing) As Object
        Dim buffer = ApiArgumentHelpers.GetFileStream(file, FileAccess.Read, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        End If

        Return New mzPackReader(buffer.TryCast(Of Stream))
    End Function

    <ExportAPI("ls")>
    Public Function index(mzpack As mzPackReader) As String()
        Return mzpack.EnumerateIndex.ToArray
    End Function

    <ExportAPI("metadata")>
    Public Function GetMetaData(mzpack As mzPackReader, index As String) As list
        Return New list(mzpack.GetMetadata(index))
    End Function

    <ExportAPI("scaninfo")>
    Public Function scanInfo(mzpack As mzPackReader, index As String) As list
        Dim scan As ScanMS1 = mzpack.ReadScan(index, skipProducts:=True)
        Dim info As New list With {
            .slots = New Dictionary(Of String, Object) From {
                {NameOf(scan.scan_id), index},
                {NameOf(scan.BPC), scan.BPC},
                {NameOf(scan.into), scan.into},
                {NameOf(scan.meta), scan.meta},
                {NameOf(scan.mz), scan.mz},
                {NameOf(scan.products), scan.products.TryCount},
                {NameOf(scan.rt), scan.rt},
                {NameOf(scan.TIC), scan.TIC}
            }
        }

        Return info
    End Function
End Module
