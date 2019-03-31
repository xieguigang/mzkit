#Region "Microsoft.VisualBasic::210d74b6dfe88172cd3e665e4383351f, Massbank\Public\NCBI\PubChem\Web\Query.vb"

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
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace NCBI.PubChem

    Public Module Query

        ''' <summary>
        ''' Search pubchem by CAS
        ''' </summary>
        Const queryCAS_URL As String = "https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/%s/cids/JSON"
        Const fetchPugView As String = "https://pubchem.ncbi.nlm.nih.gov/rest/pug_view/data/compound/%s/XML/"

        Public Function QueryCAS(CAS As String) As IdentifierList
            Dim url As String = sprintf(queryCAS_URL, CAS)
            Dim jsonText = url.GET

            If jsonText.StringEmpty Then
                ' 404 代码之下得到的content text是空字符串
                Return Nothing
            Else
                Dim list As IdentifierList = jsonText _
                    .LoadJSON(Of QueryResponse) _
                   ?.IdentifierList
                Return list
            End If
        End Function

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

            Dim list As IdentifierList = QueryCAS(CAS)
            Dim table As New Dictionary(Of String, PugViewRecord)

            If list Is Nothing OrElse list.CID.IsNullOrEmpty Then
                Return New Dictionary(Of String, PugViewRecord)
            Else
                For Each cid As String In list.CID
                    table(cid) = PugView(cid)
                    Call Thread.Sleep(1000)
                Next

                Call Thread.Sleep(1000)
            End If

            Call table.Values _
                .ToList _
                .GetXml _
                .SaveTo(cache)

            Return table
        End Function

        Public Function PugView(cid As String) As PugViewRecord
            Dim url As String = sprintf(fetchPugView, cid)
            Dim view As PugViewRecord = url.GET.LoadFromXml(Of PugViewRecord)

            Return view
        End Function
    End Module

    Public Class QueryResponse

        Public Property IdentifierList As IdentifierList
        ''' <summary>
        ''' 当这个属性为空值的时候说明请求成功,反之不为空的时候说明出现了错误
        ''' </summary>
        ''' <returns></returns>
        Public Property Fault As Fault

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class Fault
        Public Property Code As String
        Public Property Message As String
        Public Property Details As String()
    End Class

    Public Class IdentifierList

        Public Property CID As String()

        Public Overrides Function ToString() As String
            Return CID.GetJson
        End Function
    End Class
End Namespace
