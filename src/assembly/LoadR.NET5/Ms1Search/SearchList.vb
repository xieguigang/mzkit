#Region "Microsoft.VisualBasic::df93f0309010b85f0cbd7d8b35985686, assembly\LoadR.NET5\Ms1Search\SearchList.vb"

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

    '   Total Lines: 82
    '    Code Lines: 66 (80.49%)
    ' Comment Lines: 2 (2.44%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 14 (17.07%)
    '     File Size: 3.31 KB


    ' Class SearchList
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: QueryItem, SearchAll, UniqueResult
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports any = Microsoft.VisualBasic.Scripting

Public Class SearchList : Inherits ISearchOp

    Public Sub New(repo As IMzQuery, uniqueByScore As Boolean, field_mz As String, field_score As String, env As Environment)
        MyBase.New(repo, uniqueByScore, field_mz, field_score, env)
    End Sub

    Public Overrides Iterator Function SearchAll(mz As Object) As IEnumerable(Of MzSearch)
        Dim list As list = DirectCast(mz, list)

        For Each item As KeyValuePair(Of String, Object) In list.slots
            For Each result As MzSearch In QueryItem(item)
                result.metadata("index") = item.Key
                Yield result
            Next
        Next
    End Function

    Private Function QueryItem(item As KeyValuePair(Of String, Object)) As IEnumerable(Of MzSearch)
        If TypeOf item.Value Is list Then
            Dim sublist As list = DirectCast(item.Value, list)
            Dim mz As Double() = CLRVector.asNumeric(sublist.getByName(field_mz))
            Dim scores As Double() = 1.0.Repeats(mz.Length)

            If Not field_score.StringEmpty(, True) AndAlso sublist.hasName(field_score) Then
                scores = CLRVector.asNumeric(sublist.getByName(field_score))
            End If

            Dim all As New List(Of MzSearch)
            Dim slots = sublist.slots _
                .Select(Function(a)
                            Return New KeyValuePair(Of String, String)(a.Key, any.ToString(a.Value))
                        End Function) _
                .ToArray

            For i As Integer = 0 To mz.Length - 1
                Dim result = repo.QueryByMz(mz(i)).ToArray
                Dim offset As Integer = i
                Dim score As Double = scores(i)
                Dim searchResult As MzSearch() = result _
                    .Select(Function(r)
                                Dim o As New MzSearch(r, offset + 1)
                                o.score = score * o.score
                                o.metadata = o.metadata.AddRange(slots, replaceDuplicated:=True)

                                Return o
                            End Function) _
                    .ToArray

                Call all.AddRange(searchResult)
            Next

            Return all
        Else
            Dim mz As Double() = CLRVector.asNumeric(item.Value)

            Return mz _
                .Select(Function(mzi) repo.QueryByMz(mzi)) _
                .IteratesALL
        End If
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

