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

Imports System.Threading

Namespace NCBI.PubChem

    Public Module Query

        Public Function QueryPugViews(CAS As String, Optional cacheFolder$ = "./pubchem_cache", Optional ByRef hitCache As Boolean = False) As Dictionary(Of String, PugViewRecord)
            Dim cache = $"{cacheFolder}/{CAS.NormalizePathString(False)}.Xml"

            If cache.FileLength > 0 Then
                hitCache = True
                Return cache _
                    .LoadXml(Of List(Of PugViewRecord)) _
                    .ToDictionary(Function(info) info.RecordNumber)
            Else
                Call Thread.Sleep(1000)
            End If

            Dim cidQuery As New CIDQuery($"{cacheFolder}/cid/")
            Dim list As IdentifierList = cidQuery.Query(Of IdentifierList)(CAS, ".json")
            Dim table As New Dictionary(Of String, PugViewRecord)
            Dim api As New WebQuery($"{cacheFolder}/pugViews/")

            If list Is Nothing OrElse list.CID.IsNullOrEmpty Then
                Return New Dictionary(Of String, PugViewRecord)
            Else
                For Each cid As String In list.CID
                    table(cid) = api.Query(Of PugViewRecord)(cid)
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
