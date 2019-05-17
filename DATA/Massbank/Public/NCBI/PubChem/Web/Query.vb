#Region "Microsoft.VisualBasic::7a802f3c9bc95df5022f2a290b6fb85a, Massbank\Public\NCBI\PubChem\Web\Query.vb"

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
'         Function: PugView, QueryCAS, QueryPugViews
' 
'     Class QueryResponse
' 
'         Properties: Fault, IdentifierList
' 
'         Function: ToString
' 
'     Class Fault
' 
'         Properties: Code, Details, Message
' 
'     Class IdentifierList
' 
'         Properties: CID
' 
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace NCBI.PubChem

    Public Module Query

        ReadOnly cache As New Dictionary(Of String, Object)

        <Extension>
        Private Function getQueryHandler(Of T)(handle As String) As T
            If GetType(T) Is GetType(CIDQuery) Then
                If Not cache.ContainsKey(handle) Then
                    cache(handle) = New CIDQuery(cache:=handle)
                End If
            Else
                If Not cache.ContainsKey(handle) Then
                    cache(handle) = New WebQuery(cache:=handle)
                End If
            End If

            Return cache(handle)
        End Function

        ''' <summary>
        ''' Query pubchem compound id by a given name
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="cacheFolder$"></param>
        ''' <returns></returns>
        Public Function QueryCID(name As String, Optional cacheFolder$ = "./pubchem_cache") As String()
            Dim cidQuery As CIDQuery = $"{cacheFolder}/cid/".getQueryHandler(Of CIDQuery)
            Dim list As IdentifierList = cidQuery.Query(Of IdentifierList)(name, ".json")
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
        Public Function QueryPugViews(name As String, Optional cacheFolder$ = "./pubchem_cache") As Dictionary(Of String, PugViewRecord)
            Dim CID As String() = Query.QueryCID(name, cacheFolder)
            Dim table As New Dictionary(Of String, PugViewRecord)
            Dim api As WebQuery = $"{cacheFolder}/pugViews/".getQueryHandler(Of WebQuery)
            Dim cache = $"{cacheFolder}/{name.NormalizePathString(False)}.Xml"

            If CID.IsNullOrEmpty Then
                Return New Dictionary(Of String, PugViewRecord)
            Else
                For Each id As String In CID
                    table(id) = api.Query(Of PugViewRecord)(id)
                Next

                Call Thread.Sleep(1000)
            End If

            Call table.Values _
                .ToList _
                .GetXml _
                .SaveTo(cache)

            Return table
        End Function
    End Module
End Namespace
