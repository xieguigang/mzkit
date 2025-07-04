#Region "Microsoft.VisualBasic::2ebf26fe0c350ed41c0c83ad36b77da7, assembly\LoadR.NET5\Ms1Search\SearchTable.vb"

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

    '   Total Lines: 58
    '    Code Lines: 46 (79.31%)
    ' Comment Lines: 2 (3.45%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 10 (17.24%)
    '     File Size: 2.40 KB


    ' Class SearchTable
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: SearchAll, UniqueResult
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.MSEngine
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports any = Microsoft.VisualBasic.Scripting

Public Class SearchTable : Inherits ISearchOp

    Public Sub New(repo As IMzQuery, uniqueByScore As Boolean, field_mz As String, field_score As String, env As Environment)
        MyBase.New(repo, uniqueByScore, field_mz, field_score, env)
    End Sub

    Public Overrides Iterator Function SearchAll(mz As Object) As IEnumerable(Of MzSearch)
        Dim table As dataframe = DirectCast(mz, dataframe)

        For Each row As list In table.getRowList
            Dim mzi As Double = CLRVector.asNumeric(row(field_mz)).First
            Dim query As MzQuery() = repo.QueryByMz(mzi).ToArray

            If query.IsNullOrEmpty Then
                Continue For
            End If

            Dim meta As KeyValuePair(Of String, String)() = row.slots _
                .Select(Function(a)
                            Return New KeyValuePair(Of String, String)(a.Key, any.ToString(a.Value))
                        End Function) _
                .ToArray
            Dim score As Double = 1.0

            If Not field_score.StringEmpty(, True) AndAlso row.hasName(field_score) Then
                score = CLRVector.asNumeric(row.getByName(field_score)).First
            End If

            For Each hit As MzQuery In query
                Dim result As New MzSearch(hit, 0)
                result.metadata = result.metadata.AddRange(meta, replaceDuplicated:=True)
                result.score = score * result.score
                result.metadata("index") = CStr(row.getAttribute("rowname"))
                Yield result
            Next
        Next
    End Function

    Protected Overrides Iterator Function UniqueResult(all As IEnumerable(Of MzSearch)) As IEnumerable(Of MzSearch)
        Dim groups = all.GroupBy(Function(a) CStr(a!index)).ToArray

        For Each group As IGrouping(Of Integer, MzSearch) In groups
            If uniqueByScore Then
                ' get top score
                Yield group.OrderByDescending(Function(a) a.score).First
            Else
                ' get min ppm
                Yield group.OrderBy(Function(a) a.ppm).First
            End If
        Next
    End Function
End Class

