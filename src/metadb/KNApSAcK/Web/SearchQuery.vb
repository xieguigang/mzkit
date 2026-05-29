#Region "Microsoft.VisualBasic::afdaa7decf09a53352b879f6038a6401, metadb\KNApSAcK\Web\SearchQuery.vb"

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

    '   Total Lines: 60
    '    Code Lines: 51 (85.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (15.00%)
    '     File Size: 2.11 KB


    ' Class SearchQuery
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: guid, parseList, parser, url
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry.Massbank.KNApSAcK.Data
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Public Class SearchQuery : Inherits WebQuery(Of QueryInput)

    Public Sub New(<CallerMemberName>
                   Optional cache As String = Nothing,
                   Optional interval As Integer = -1,
                   Optional offline As Boolean = False)

        MyBase.New(
            url:=AddressOf url,
            contextGuid:=AddressOf guid,
            parser:=AddressOf parser,
            prefix:=Function(str) str.MD5.First,
            cache:=cache,
            interval:=interval,
            offline:=offline
        )
    End Sub

    Private Shared Function guid(q As QueryInput) As String
        Return q.ToString
    End Function

    Private Shared Function parser(html As String, type As Type) As Object
        Return parseList(html).ToArray
    End Function

    Private Shared Iterator Function parseList(html As String) As IEnumerable(Of ResultEntry)
        Dim content As String() = html _
            .GetTablesHTML _
            .FirstOrDefault _
            .GetRowsHTML

        For Each rowText As String In content
            Dim cells As String() = rowText.GetColumnsHTML
            Dim names As String() = cells(2) _
                .HtmlLines _
                .Select(Function(str) str.StripHTMLTags) _
                .ToArray
            Dim entry As New ResultEntry With {
                .C_ID = cells(Scan0).StripHTMLTags,
                .CAS_ID = cells(1).StripHTMLTags,
                .Metabolite = names,
                .formula = cells(3).StripHTMLTags,
                .Mw = cells(4).StripHTMLTags.ParseDouble
            }

            Yield entry
        Next
    End Function

    Private Shared Function url(q As QueryInput) As String
        Return sprintf(My.Resources.knapsack_search, q.type.Description, q.word.UrlEncode)
    End Function
End Class
