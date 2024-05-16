#Region "Microsoft.VisualBasic::bc4f7d7d4fe4170576db3bd6824a4745, mzmath\MSEngine\DBPool.vb"

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

    '   Total Lines: 53
    '    Code Lines: 40
    ' Comment Lines: 3
    '   Blank Lines: 10
    '     File Size: 1.80 KB


    ' Class DBPool
    ' 
    '     Function: getAnnotation, MSetAnnotation, QueryByMz
    ' 
    '     Sub: Register
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

''' <summary>
''' database pool of the metabolites database
''' </summary>
Public Class DBPool

    Protected ReadOnly metadb As New Dictionary(Of String, IMzQuery)

    Public Sub Register(name As String, database As IMzQuery)
        If Not database Is Nothing Then
            metadb(name) = database
        End If
    End Sub

    Public Function getAnnotation(uniqueId As String) As (name As String, formula As String)
        For Each db In metadb.Values
            Dim result = db.GetAnnotation(uniqueId)

            If Not (result.formula.StringEmpty AndAlso result.name.StringEmpty) Then
                Return result
            End If
        Next

        Return Nothing
    End Function

    Public Iterator Function QueryByMz(mz As Double) As IEnumerable(Of NamedCollection(Of MzQuery))
        For Each xrefDb In metadb
            Yield New NamedCollection(Of MzQuery) With {
                .name = xrefDb.Key,
                .value = xrefDb.Value.QueryByMz(mz).ToArray
            }
        Next
    End Function

    Public Iterator Function MSetAnnotation(mzlist As IEnumerable(Of Double), Optional println As Action(Of String) = Nothing) As IEnumerable(Of NamedCollection(Of MzQuery))
        Dim allMz As Double() = mzlist.ToArray

        For Each xrefDb In metadb
            If Not println Is Nothing Then
                Call println($"Do m/z set annotation of {xrefDb.Key}...")
            End If

            Yield New NamedCollection(Of MzQuery) With {
                .name = xrefDb.Key,
                .value = xrefDb.Value _
                    .MSetAnnotation(allMz) _
                    .ToArray
            }
        Next
    End Function
End Class
