#Region "Microsoft.VisualBasic::16109d318ff749fcca17f2048d085da7, assembly\LoadR.NET5\Ms1Search\SearchVector.vb"

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

    '   Total Lines: 43
    '    Code Lines: 32 (74.42%)
    ' Comment Lines: 2 (4.65%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (20.93%)
    '     File Size: 1.54 KB


    ' Class SearchVector
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: QueryMz, SearchAll, UniqueResult
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.MSEngine
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Vectorization

Public Class SearchVector : Inherits ISearchOp

    Public Sub New(repo As IMzQuery, uniqueByScore As Boolean, field_mz As String, field_score As String, env As Environment)
        MyBase.New(repo, uniqueByScore, field_mz, field_score, env)
    End Sub

    Public Overrides Function SearchAll(mz As Object) As IEnumerable(Of MzSearch)
        Return QueryMz(CLRVector.asNumeric(mz))
    End Function

    Private Iterator Function QueryMz(mz As Double()) As IEnumerable(Of MzSearch())
        Dim index As Integer = 1

        For Each mzi As Double In mz
            Dim all As MzQuery() = repo.QueryByMz(mzi).ToArray
            Dim pops As MzSearch() = all _
                .Select(Function(i) New MzSearch(i, index)) _
                .ToArray

            index += 1

            Yield pops
        Next
    End Function

    Protected Overrides Iterator Function UniqueResult(all As IEnumerable(Of MzSearch)) As IEnumerable(Of MzSearch)
        Dim groups = all.GroupBy(Function(a) CInt(a!index)).ToArray

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

