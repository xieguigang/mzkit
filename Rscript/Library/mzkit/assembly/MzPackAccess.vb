#Region "Microsoft.VisualBasic::5e600a629355546513ef5d546ba96fb3, Rscript\Library\mzkit\assembly\MzPackAccess.vb"

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

    ' Module MzPackAccess
    ' 
    '     Function: GetMetaData, index, open_mzpack, scanInfo
    ' 
    ' /********************************************************************************/

#End Region

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
