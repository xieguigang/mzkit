﻿#Region "Microsoft.VisualBasic::0227696f9d79c9edf8294f882857c0f6, metadna\metaDNA\Models\Networking\KEGGNetwork.vb"

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

    '   Total Lines: 151
    '    Code Lines: 112 (74.17%)
    ' Comment Lines: 16 (10.60%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 23 (15.23%)
    '     File Size: 6.04 KB


    ' Class KEGGNetwork
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: (+2 Overloads) CreateNetwork, FindPartners, FindReactions, MapReduce, networkResolver
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

''' <summary>
''' the metabolite network resolver
''' </summary>
''' <remarks>
''' provides the compound connection and partner relationship 
''' inside the metadna inference.
''' </remarks>
Public Class KEGGNetwork : Inherits Networking

    ''' <summary>
    ''' the partner id mapping which is build based on the reaction network
    ''' </summary>
    Dim kegg_id As Dictionary(Of String, String())
    Dim reactionList As Dictionary(Of String, NamedValue(Of String)())

    Private Sub New()
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function FindPartners(kegg_id As String) As IEnumerable(Of String)
        If Me.kegg_id.ContainsKey(kegg_id) Then
            Return Me.kegg_id(kegg_id)
        Else
            Return {}
        End If
    End Function

    ''' <summary>
    ''' find reaction list for export report table
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function FindReactions(a As String, b As String) As NamedValue(Of String)()
        Return New String() {a, b} _
            .OrderBy(Function(str) str) _
            .JoinBy("+") _
            .DoCall(AddressOf reactionList.TryGetValue)
    End Function

    Public Shared Function CreateNetwork(network As IEnumerable(Of Reaction)) As KEGGNetwork
        Dim index As New Dictionary(Of String, List(Of String))
        Dim reactions As New Dictionary(Of String, List(Of NamedValue(Of String)))
        Dim eq As Equation = Nothing

        For Each reaction As Reaction In network
            If reaction.Equation.StringEmpty Then
                Call $"{reaction.ID}: {reaction.Definition} (equation is empty!)".Warning
                Continue For
            Else
                eq = reaction.ReactionModel
            End If

            For Each cid1 As String In eq.Reactants.Keys
                For Each cid2 As String In eq.Products.Keys
                    If Not index.ContainsKey(cid1) Then
                        index(cid1) = New List(Of String)
                    End If
                    If Not index.ContainsKey(cid2) Then
                        index(cid2) = New List(Of String)
                    End If

                    index(cid1).Add(cid2)
                    index(cid2).Add(cid1)

                    Dim key As String = {cid1, cid2} _
                        .OrderBy(Function(str) str) _
                        .JoinBy("+")

                    If Not reactions.ContainsKey(key) Then
                        reactions(key) = New List(Of NamedValue(Of String))
                    End If

                    Call New NamedValue(Of String) With {
                        .Name = reaction.ID,
                        .Value = reaction.Definition
                    }.DoCall(AddressOf reactions(key).Add)
                Next
            Next
        Next

        Return networkResolver(index, reactions)
    End Function

    Private Shared Function networkResolver(index As Dictionary(Of String, List(Of String)), reactions As Dictionary(Of String, List(Of NamedValue(Of String)))) As KEGGNetwork
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

                Dim key As String = {link.from, link.to} _
                    .OrderBy(Function(str) str) _
                    .JoinBy("+")

                If Not reactions.ContainsKey(key) Then
                    reactions(key) = New List(Of NamedValue(Of String))
                End If

                Call New NamedValue(Of String) With {
                    .Name = reaction.entryId,
                    .Value = reaction.definition
                }.DoCall(AddressOf reactions(key).Add)
            Next
        Next

        Return networkResolver(index, reactions)
    End Function

    Public Shared Iterator Function MapReduce(maps As IEnumerable(Of Map), KO As String(), network As IEnumerable(Of ReactionClass)) As IEnumerable(Of ReactionClass)
        For Each reaction As ReactionClass In network

        Next
    End Function
End Class
