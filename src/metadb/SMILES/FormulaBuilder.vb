#Region "Microsoft.VisualBasic::b08b1db674510a8999c9d28e60415458, E:/mzkit/src/metadb/SMILES//FormulaBuilder.vb"

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

    '   Total Lines: 98
    '    Code Lines: 72
    ' Comment Lines: 11
    '   Blank Lines: 15
    '     File Size: 3.34 KB


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
    Dim atomGroups As Dictionary(Of String, Atom)

    Sub New(graph As ChemicalFormula)
        Me.graph = graph
        Me.atomProfile = Atom _
            .DefaultElements _
            .ToDictionary(Function(a) a.label)
        Me.atomGroups = Atom.DefaultAtomGroups _
            .ToDictionary(Function(a)
                              Return a.GetIonLabel
                          End Function)
    End Sub

    Public Function GetComposition(ByRef empirical As String) As Dictionary(Of String, Integer)
        If graph.AllBonds.Count = 0 Then
            ' only a single element
            If graph.AllElements.Count = 1 Then
                Call WalkElement(graph.AllElements.First, Bonds.NA)
            End If
        Else
            For Each bond As ChemicalKey In graph.AllBonds
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
                    Throw New NotImplementedException(element.elementName)
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
