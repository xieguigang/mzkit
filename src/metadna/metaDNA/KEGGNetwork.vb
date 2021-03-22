#Region "Microsoft.VisualBasic::84b07f9ad7a1665f36d61e31c0ea883a, src\metadna\metaDNA\KEGGNetwork.vb"

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

    ' Class KEGGNetwork
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: CreateNetwork, FindPartners, FindReactions
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Public Class KEGGNetwork

    Dim kegg_id As Dictionary(Of String, String())
    Dim reactionList As Dictionary(Of String, NamedValue(Of String)())

    Private Sub New()
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function FindPartners(kegg_id As String) As IEnumerable(Of String)
        If Me.kegg_id.ContainsKey(kegg_id) Then
            Return Me.kegg_id(kegg_id)
        Else
            Return {}
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function FindReactions(a As String, b As String) As NamedValue(Of String)()
        Return New String() {a, b} _
            .OrderBy(Function(str) str) _
            .JoinBy("+") _
            .DoCall(AddressOf reactionList.TryGetValue)
    End Function

    Public Shared Function CreateNetwork(network As IEnumerable(Of ReactionClass)) As KEGGNetwork
        Dim index As New Dictionary(Of String, List(Of String))
        Dim reactions As New Dictionary(Of String, List(Of NamedValue(Of String)))

        For Each reaction As ReactionClass In network
            For Each link As ReactionCompoundTransform In reaction.reactantPairs
                If Not index.ContainsKey(link.from) Then
                    index(link.from) = New List(Of String)
                End If
                If Not index.ContainsKey(link.to) Then
                    index(link.to) = New List(Of String)
                End If

                index(link.from).Add(link.to)
                index(link.to).Add(link.from)

                Dim key As String = {link.from, link.to}.OrderBy(Function(str) str).JoinBy("+")

                If Not reactions.ContainsKey(key) Then
                    reactions(key) = New List(Of NamedValue(Of String))
                End If

                Call New NamedValue(Of String) With {
                    .Name = reaction.entryId,
                    .Value = reaction.definition
                }.DoCall(AddressOf reactions(key).Add)
            Next
        Next

        Return New KEGGNetwork With {
            .kegg_id = index _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.Value.Distinct.ToArray
                              End Function),
            .reactionList = reactions _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.Value _
                                      .GroupBy(Function(t) t.Name) _
                                      .Select(Function(t) t.First) _
                                      .ToArray
                              End Function)
        }
    End Function

End Class
