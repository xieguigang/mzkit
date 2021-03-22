#Region "Microsoft.VisualBasic::d3deddb5b64d8b9c54e5ff997c378f6e, src\metadb\MetabolomeXchange\Provider.vb"

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

    ' Module Provider
    ' 
    '     Function: ensureStringArray, GetAllDataSet, GetAllDataSetJson, (+2 Overloads) ToTable
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON
Imports MetabolomeXchange.Json
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Public Module Provider

    Const api$ = "http://api.metabolomexchange.org"

    ''' <summary>
    ''' Get all info about a single provider, including a list of the datasets of that provider.
    ''' 
    ''' > ``/provider/{shortname}``
    ''' </summary>
    ''' <param name="provider">The provider shortname, default is using ``metabolights`` repostiory</param>
    ''' <returns></returns>
    Public Function GetAllDataSetJson(Optional provider$ = "mtbls") As String
        Dim url$ = $"{api}/provider/{provider}"
        Dim json$ = url.GET
        Return json
    End Function

    Public Function GetAllDataSet(Optional provider$ = "mtbls") As DataSet()
        Return GetAllDataSetJson(provider) _
            .LoadJSON(Of response) _
            .datasets _
            .Values _
            .ToArray
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ToTable(data As IEnumerable(Of DataSet)) As DataTable()
        Return data.Select(AddressOf ToTable).ToArray
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function ToTable(data As DataSet) As DataTable
        Return New DataTable With {
            .analysis = data.meta.analysis,
            .date = Date.Parse(data.date),           ' .description = data.description,
            .ID = data.accession,            ' .metabolites = data.meta.metabolites,
            .organism = ensureStringArray(data.meta.organism),
            .organism_parts = ensureStringArray(data.meta.organism_parts),
            .platform = data.meta.platform,
            .publications = data.publications _
                .SafeQuery _
                .Select(Function(pub) pub.ToString) _
                .ToArray,
            .submitter = data.submitter,
            .title = data.title            ' .url = data.url
        }
    End Function

    Private Function ensureStringArray(data) As String()
        Return If(TypeOf data Is String, {CStr(data)}, DirectCast(data, IEnumerable(Of String)).ToArray)
    End Function
End Module
