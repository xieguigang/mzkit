#Region "Microsoft.VisualBasic::5e600a629355546513ef5d546ba96fb3, mzkit\Rscript\Library\mzkit\assembly\MzPackAccess.vb"

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

'   Total Lines: 55
'    Code Lines: 47
' Comment Lines: 0
'   Blank Lines: 8
'     File Size: 2.02 KB


' Module MzPackAccess
' 
'     Function: GetMetaData, index, open_mzpack, scanInfo
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports stdNum = System.Math

<Package("mzPack")>
Module MzPackAccess

    ''' <summary>
    ''' open mzwork file and then populate all of the mzpack raw data file
    ''' </summary>
    ''' <param name="mzwork"></param>
    ''' <returns>
    ''' a collection of mzpack raw data objects
    ''' </returns>
    <ExportAPI("open.mzwork")>
    <RApiReturn(GetType(mzPack))>
    Public Function populateMzPacks(mzwork As String, Optional env As Environment = Nothing) As pipeline
        Dim stdout = env.WriteLineHandler
        Dim println As Action(Of String) =
            Sub(text)
                Call stdout(text)
            End Sub
        Dim verbose As Boolean = env.globalEnvironment.options.verbose
        Dim print2 As Action(Of String, String) =
            Sub(text1, text2)
                If verbose Then Call stdout($"[{text1}] {text2}")
            End Sub
        Dim stream As IEnumerable(Of mzPack) =
            Iterator Function() As IEnumerable(Of mzPack)
                Using pack As New ZipArchive(mzwork.Open(FileMode.Open, doClear:=False, [readOnly]:=True), ZipArchiveMode.Read)
                    For Each group In ParseArchive.LoadRawGroups(zip:=pack, msg:=println)
                        For Each raw As Raw In group
                            Dim mzpack As mzPack = raw.LoadMzpack(print2, verbose).GetLoadedMzpack
                            mzpack.source = group.name
                            Yield mzpack
                        Next
                    Next
                End Using
            End Function()

        Return pipeline.CreateFromPopulator(stream)
    End Function

    <ExportAPI("mzwork")>
    <RApiReturn(GetType(WorkspaceAccess))>
    Public Function open_mzwork(file As Object, Optional env As Environment = Nothing) As Object
        Dim buffer = ApiArgumentHelpers.GetFileStream(file, FileAccess.Read, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        End If

        Dim println As Action(Of Object) = env.WriteLineHandler

        Return New WorkspaceAccess(
            file:=buffer.TryCast(Of Stream),
            msg:=Sub(line)
                     Call println(line)
                 End Sub)
    End Function

    <ExportAPI("readFileCache")>
    <RApiReturn(GetType(mzPack))>
    Public Function readFileCache(mzwork As WorkspaceAccess, fileName As String) As Object
        Return mzwork.GetByFileName(fileName).ToArray
    End Function

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
    <RApiReturn(GetType(String))>
    Public Function index(mzpack As Object, Optional env As Environment = Nothing) As Object
        If mzpack Is Nothing Then
            Return Nothing
        ElseIf TypeOf mzpack Is mzPackReader Then
            Return DirectCast(mzpack, mzPackReader) _
                .EnumerateIndex _
                .ToArray
        ElseIf TypeOf mzpack Is WorkspaceAccess Then
            Return DirectCast(mzpack, WorkspaceAccess).ListAllFileNames
        Else
            Return Message.InCompatibleType(GetType(mzPackReader), mzpack.GetType, env)
        End If
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

    <ExportAPI("convertTo_mzXML")>
    <RApiReturn(GetType(Boolean))>
    Public Function convertTo_mzXML(mzpack As mzPack, file As Object, Optional env As Environment = Nothing) As Object
        Dim buffer = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        End If

        Using mzXML As New mzXMLWriter({}, {}, {}, buffer.TryCast(Of Stream))
            Call mzXML.WriteData(mzpack.MS)
        End Using

        Return True
    End Function

    <ExportAPI("packData")>
    <RApiReturn(GetType(mzPack))>
    Public Function packData(<RRawVectorArgument>
                             data As Object,
                             Optional timeWindow As Double = 1,
                             Optional env As Environment = Nothing) As Object
        Dim peaks As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(data, env)

        If peaks.isError Then
            Return peaks.getError
        End If

        Dim groupScans = peaks _
            .populates(Of PeakMs2)(env) _
            .GroupBy(Function(t) t.rt,
                     Function(t1, t2)
                         Return stdNum.Abs(t1 - t2) <= timeWindow
                     End Function) _
            .ToArray
        Dim groupMs1 = (From list As NamedCollection(Of PeakMs2)
                        In groupScans
                        Select list.scan1).ToArray

        Return New mzPack With {
            .Application = FileApplicationClass.LCMS,
            .MS = groupMs1,
            .source = "<assembly>"
        }
    End Function

    <Extension>
    Private Function scan1(list As NamedCollection(Of PeakMs2)) As ScanMS1
        Dim scan2 As ScanMS2() = list _
            .Select(Function(i)
                        Return New ScanMS2 With {
                            .centroided = True,
                            .mz = i.mzInto.Select(Function(mzi) mzi.mz).ToArray,
                            .into = i.mzInto.Select(Function(mzi) mzi.intensity).ToArray,
                            .parentMz = i.mz,
                            .intensity = i.intensity,
                            .rt = i.rt,
                            .scan_id = $"{i.file}#{i.lib_guid}",
                            .collisionEnergy = i.collisionEnergy
                        }
                    End Function) _
            .ToArray

        Return New ScanMS1 With {
           .into = scan2.Select(Function(i) i.intensity).ToArray,
           .mz = scan2.Select(Function(i) i.parentMz).ToArray,
           .products = scan2,
           .rt = Val(list.name),
           .scan_id = list.name,
           .TIC = .into.Sum,
           .BPC = .into.Max
        }
    End Function
End Module
