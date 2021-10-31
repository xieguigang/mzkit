#Region "Microsoft.VisualBasic::636f731beb3dfd4ab14de85f201ccf68, src\mzkit\Task\BioDeep\KEGGRepo.vb"

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

    ' Module KEGGRepo
    ' 
    '     Function: RequestKEGGcompounds, RequestKEGGMaps, RequestKEGGReactions, unzip
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports BioDeep
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.My
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Data.KEGG.Metabolism

Module KEGGRepo

    <Extension>
    Private Function unzip(file As Stream) As Stream
        Using zip As New ZipArchive(file, ZipArchiveMode.Read)
            Dim repoEntry As ZipArchiveEntry = zip.Entries.First

            Using repofile As Stream = repoEntry.Open
                Dim buffer As New MemoryStream

                Call repofile.CopyTo(buffer)
                Call buffer.Seek(0, SeekOrigin.Begin)

                Return buffer
            End Using
        End Using
    End Function

    Public Function RequestKEGGcompounds(println As Action(Of String)) As Compound()
        Const url As String = "http://query.biodeep.cn/kegg/repository/compounds"

        Call println("request KEGG compounds from BioDeep...")

        Return SingletonHolder(Of BioDeepSession).Instance _
            .RequestStream(url) _
            .unzip _
            .DoCall(AddressOf KEGGCompoundPack.ReadKeggDb)
    End Function

    Public Function RequestKEGGReactions(println As Action(Of String)) As ReactionClass()
        Const url As String = "http://query.biodeep.cn/kegg/repository/reactions"

        Call println("request KEGG reaction network from BioDeep...")

        Return SingletonHolder(Of BioDeepSession).Instance _
            .RequestStream(url) _
            .unzip _
            .DoCall(AddressOf ReactionClassPack.ReadKeggDb)
    End Function

    Public Function RequestKEGGMaps() As Map()
        Dim filepath As String = $"{App.HOME}/Rstudio/Repository/KEGG_maps.msgpack"

        If Not filepath.FileExists Then
            filepath = $"{App.HOME}/../../Rscript\Library\mzkit_app\data\KEGG_maps.msgpack"
        End If

        Using file As Stream = filepath.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Return KEGGMapPack.ReadKeggDb(file)
        End Using
    End Function
End Module
