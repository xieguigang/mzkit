#Region "Microsoft.VisualBasic::9cc5605aac0cac319d8eaf9f75779066, metadb\SMILES\FormulaBuilder.vb"

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

    '   Total Lines: 108
    '    Code Lines: 77 (71.30%)
    ' Comment Lines: 12 (11.11%)
    '    - Xml Docs: 83.33%
    ' 
    '   Blank Lines: 19 (17.59%)
    '     File Size: 3.75 KB


    ' Class FormulaBuilder
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetComposition
    ' 
    '     Sub: (+2 Overloads) Push, WalkElement
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection

Public Class FormulaBuilder

    ReadOnly graph As ChemicalFormula

    Dim empirical As New StringBuilder
    Dim composition As New Dictionary(Of String, Integer)
    Dim visited As New Index(Of String)
    Dim atomProfile As Dictionary(Of String, Atom)
    Dim atomGroups As New Dictionary(Of String, AtomGroup)

    Sub New(graph As ChemicalFormula)
        Me.graph = graph
        Me.atomProfile = Atom _
            .DefaultElements _
            .ToDictionary(Function(a) a.label)

        For Each group As AtomGroup In AtomGroup.DefaultAtomGroups
            atomGroups(group.GetIonLabel) = group
            atomGroups($"[{group.label}]") = group
        Next
    End Sub

    Public Function GetComposition(ByRef empirical As String) As Dictionary(Of String, Integer)
        If graph.AllBonds.Count = 0 Then
            ' only a single element
            If graph.AllElements.Count = 1 Then
                Call WalkElement(graph.AllElements.First, Bonds.NA)
            End If
        Else
            Dim last_graph As Integer = -1

            For Each bond As ChemicalKey In graph.AllBonds.OrderBy(Function(k) k.U.graph_id)
                If last_graph > -1 AndAlso bond.U.graph_id <> last_graph Then
                    ' start a new independent chemical graph
                    Call Me.empirical.Append(".")
                End If

                last_graph = bond.U.graph_id

                Call WalkElement(bond.U, bond.bond)
                Call WalkElement(bond.V, bond.bond)
            Next
        End If

        empirical = Me.empirical.ToString

        Return composition
    End Function

    Private Sub WalkElement(element As ChemicalElement, bond As Bonds)
        If Not element.label Like visited Then
            visited += element.label
        Else
            Return
        End If

        Select Case element.elementName
            Case "H" : Call Push("H")
            Case Else

                If atomProfile.ContainsKey(element.elementName) Then
                    Call Push(atomProfile(element.elementName), element)
                ElseIf atomGroups.ContainsKey(element.elementName) Then
                    Call Push(atomGroups(element.elementName), element)
                Else
                    Throw New NotImplementedException("Unknown element name for build formula: " & element.elementName)
                End If
        End Select
    End Sub

    ''' <summary>
    ''' add target element group into the target chemical formula data
    ''' </summary>
    ''' <param name="atom"></param>
    ''' <param name="element"></param>
    Private Sub Push(atom As Atom, element As ChemicalElement)
        Dim n As Integer = Aggregate key As ChemicalKey
                           In graph.FindKeys(element.label)
                           Into Sum(CInt(key.bond))

        Call Push(atom.label)
        Call Push("H", If(element.hydrogen > 0, element.hydrogen, atom.maxKeys - n))
    End Sub

    ''' <summary>
    ''' Add n elements into the target chemical formula composition data
    ''' </summary>
    ''' <param name="element"></param>
    ''' <param name="n"></param>
    Private Sub Push(element As String, Optional n As Integer = 1)
        If Not composition.ContainsKey(element) Then
            composition.Add(element, 0)
        End If

        If n > 0 Then
            composition(element) += n

            If n = 1 Then
                empirical.Append(element)
            Else
                empirical.Append($"{element}{n}")
            End If
        End If
    End Sub
End Class
