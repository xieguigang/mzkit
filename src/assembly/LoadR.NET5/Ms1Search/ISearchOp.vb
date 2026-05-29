#Region "Microsoft.VisualBasic::1ac767188e2dcee223082b398cbce4aa, assembly\LoadR.NET5\Ms1Search\ISearchOp.vb"

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

    '   Total Lines: 72
    '    Code Lines: 44 (61.11%)
    ' Comment Lines: 11 (15.28%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 17 (23.61%)
    '     File Size: 2.10 KB


    ' Class ISearchOp
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: SearchUnique
    ' 
    ' Class MzSearch
    ' 
    '     Properties: metadata
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.MSEngine
Imports SMRUCC.Rsharp.Runtime

Public MustInherit Class ISearchOp

    Protected repo As IMzQuery

    Protected uniqueByScore As Boolean
    Protected field_mz As String
    Protected field_score As String
    Protected env As Environment

    Sub New(repo As IMzQuery,
            uniqueByScore As Boolean,
            field_mz As String,
            field_score As String,
            env As Environment)

        Me.repo = repo

        Me.uniqueByScore = uniqueByScore
        Me.field_mz = field_mz
        Me.field_score = field_score
        Me.env = env
    End Sub

    Public MustOverride Function SearchAll(mz As Object) As IEnumerable(Of MzSearch)

    Public Function SearchUnique(mz As Object) As IEnumerable(Of MzSearch)
        Return UniqueResult(SearchAll(mz))
    End Function

    Protected MustOverride Function UniqueResult(all As IEnumerable(Of MzSearch)) As IEnumerable(Of MzSearch)

End Class

Public Class MzSearch : Inherits MzQuery

    ''' <summary>
    ''' the additional metadata informartin of the search result.
    ''' this is a key-value pair dictionary that can be used to store
    ''' additional information about the search result, such as the index
    ''' of the result in the original data source, or any other metadata
    ''' that may be relevant to the search result.
    ''' </summary>
    ''' <returns></returns>
    Public Property metadata As Dictionary(Of String, String)

    Default Public Property Item(key As String) As String
        Get
            If metadata Is Nothing Then
                Return Nothing
            End If
            Return If(metadata.ContainsKey(key), metadata(key), Nothing)
        End Get
        Set
            metadata(key) = Value
        End Set
    End Property

    ''' <summary>
    ''' create a new search result object for the given m/z query.
    ''' </summary>
    Sub New()
    End Sub

    Sub New(copy As MzQuery, index As Integer)
        Call MyBase.New(copy)

        Me.metadata = New Dictionary(Of String, String) From {
            {"index", index}
        }
    End Sub

End Class
