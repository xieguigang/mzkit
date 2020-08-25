#Region "Microsoft.VisualBasic::e64a898492f4da23848ca21514503802, src\metadb\Massbank\Public\NCBI\PubChem\Web\Query\Query.vb"

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

    '     Module Query
    ' 
    '         Function: FetchPugViewByCID, getQueryHandler, QueryCID, QueryPugViews
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http

Namespace NCBI.PubChem

    Public Module Query

        ReadOnly cache As New Dictionary(Of String, Object)

        ''' <summary>
        ''' 根据类型获取数据请求查询的接口对象
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="handle"></param>
        ''' <returns></returns>
        <Extension>
        Private Function getQueryHandler(Of T)(handle As String, offline As Boolean) As T
            If GetType(T) Is GetType(CIDQuery) Then
                If Not cache.ContainsKey(handle) Then
                    cache(handle) = New CIDQuery(cache:=handle, offline:=offline)
                End If
            Else
                If Not cache.ContainsKey(handle) Then
                    cache(handle) = New WebQuery(cache:=handle, offline:=offline)
                End If
            End If

            ' 因为在这里采用的是缓存,所以还需要额外的修改执行模式
            Return DirectCast(cache(handle), WebQuery(Of String)) _
                .With(Sub(q) q.offlineMode = offline) _
                .DoCall(Function(q) CObj(q))
        End Function

        ''' <summary>
        ''' Query pubchem compound id by a given name
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="cacheFolder$"></param>
        ''' <returns></returns>
        Public Function QueryCID(name As String, Optional cacheFolder$ = "./pubchem_cache", Optional offlineMode As Boolean = False, Optional ByRef hitCache As Boolean = False) As String()
            Dim cidQuery As CIDQuery = $"{cacheFolder}/cid/".getQueryHandler(Of CIDQuery)(offline:=offlineMode)
            Dim list As IdentifierList = cidQuery.Query(Of IdentifierList)(name, ".json", hitCache:=hitCache)
            Dim CID As String() = Nothing

            If list Is Nothing OrElse list.CID.IsNullOrEmpty Then
                ' Dim exportQuery As New CIDExport($"{cacheFolder}/cid/")
                ' Dim exportList = exportQuery.Query(Of QueryTableExport())(CAS, ".txt")

                ' CID = exportList.Select(Function(row) row.cid).ToArray
            Else
                CID = list.CID
            End If

            Return CID
        End Function

        ''' <summary>
        ''' Query compound annotation information by a given name
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="cacheFolder">
        ''' The folder for save cache json/xml data, which are downloads from pubchem web server.
        ''' </param>
        ''' <returns></returns>
        Public Function QueryPugViews(name As String, Optional cacheFolder$ = "./pubchem_cache", Optional offline As Boolean = False) As Dictionary(Of String, PugViewRecord)
            Dim hitCache As Boolean = False
            Dim CID As String() = Query.QueryCID(name, cacheFolder, offlineMode:=offline, hitCache:=hitCache)
            Dim table As New Dictionary(Of String, PugViewRecord)
            Dim api As WebQuery = $"{cacheFolder}/pugViews/".getQueryHandler(Of WebQuery)(offline)
            Dim cache = $"{cacheFolder}/{name.NormalizePathString(False)}.Xml"

            If CID.IsNullOrEmpty Then
                Return New Dictionary(Of String, PugViewRecord)
            Else
                Dim hitsCache As New List(Of Boolean)

                For Each id As String In CID
                    table(id) = api.Query(Of PugViewRecord)(id, hitCache:=hitCache)
                    hitsCache.Add(hitCache)
                Next

                If Not hitsCache.Any(Function(t) True = t) Then
                    Call Thread.Sleep(1000)
                End If
            End If

            Call table.Values _
                .ToList _
                .GetXml _
                .SaveTo(cache)

            Return table
        End Function

        Public Function FetchPugViewByCID(cid As String, Optional cacheFolder$ = "./pubchem_cache", Optional offline As Boolean = False) As PugViewRecord
            Return $"{cacheFolder}/pugViews/".getQueryHandler(Of WebQuery)(offline).Query(Of PugViewRecord)(cid)
        End Function
    End Module
End Namespace
