﻿#Region "Microsoft.VisualBasic::35c33c788d4bb13cc90c749aedf01b11, metadb\SMILES\Graph\ChemicalElement.vb"

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

    '   Total Lines: 238
    '    Code Lines: 154 (64.71%)
    ' Comment Lines: 57 (23.95%)
    '    - Xml Docs: 80.70%
    ' 
    '   Blank Lines: 27 (11.34%)
    '     File Size: 7.87 KB


    ' Class ChemicalElement
    ' 
    '     Properties: aromatic, charge, coordinate, elementName, graph_id
    '                 group, hydrogen, Keys
    ' 
    '     Constructor: (+3 Overloads) Sub New
    ' 
    '     Function: GetConnection
    ' 
    '     Sub: CarbonGroup, NitrogenGroup, OxygenGroup, (+2 Overloads) SetAtomGroups, setAtomLabel
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports std = System.Math

''' <summary>
''' the chemical atom element
''' </summary>
Public Class ChemicalElement : Inherits Node

    ''' <summary>
    ''' the atom or atom group element label text
    ''' </summary>
    ''' <returns></returns>
    Public Property elementName As String

    ''' <summary>
    ''' 与当前的这个元素连接的化学键的数量
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Keys As Integer
        Get
            Return degree.In + degree.Out
        End Get
    End Property

    ''' <summary>
    ''' 2D coordinate ``[x,y]``
    ''' </summary>
    ''' <returns></returns>
    Public Property coordinate As Double()
    ''' <summary>
    ''' the atom group name
    ''' </summary>
    ''' <returns></returns>
    Public Property group As String

    ''' <summary>
    ''' the ion charge value
    ''' </summary>
    ''' <returns></returns>
    Public Property charge As Integer
    ''' <summary>
    ''' The number of the hydrogen of current atom group it has
    ''' </summary>
    ''' <returns></returns>
    Public Property hydrogen As Integer = 0
    Public Property graph_id As Integer = 1
    Public Property aromatic As Boolean = False

    Sub New()
    End Sub

    ''' <summary>
    ''' make value copy from the base item
    ''' </summary>
    ''' <param name="base"></param>
    Sub New(base As ChemicalElement, index As Integer)
        Call Me.New(base.elementName, index)

        ' Me.label = base.label
        Me.degree = base.degree
        Me.charge = base.charge
        Me.coordinate = base.coordinate
        Me.hydrogen = base.hydrogen
        ' Me.elementName = base.elementName
        Me.group = base.group
        Me.ID = index
        Me.graph_id = base.graph_id
        Me.aromatic = base.aromatic
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="element">
    ''' the atom or atom group element label text
    ''' </param>
    ''' <param name="index"></param>
    Sub New(element As String, Optional index As Integer? = Nothing)
        Me.label = If(
            index Is Nothing,
            App.GetNextUniqueName($"{element}_"),
            $"{element}_{CInt(index)}"
        )
        Me.elementName = element
    End Sub

    Public Shared Iterator Function GetConnection(formula As ChemicalFormula, atom As ChemicalElement) As IEnumerable(Of (keys As Bonds, ChemicalElement))
        Dim key1, key2 As ChemicalKey

        For Each partner As ChemicalElement In formula.vertex.Where(Function(v) v IsNot atom)
            key1 = formula.QueryEdge(atom.label, partner.label)

            If key1 IsNot Nothing Then
                Yield (key1.bond, partner)
            End If

            key2 = formula.QueryEdge(partner.label, atom.label)

            If key2 IsNot Nothing AndAlso Not key1 Is key2 Then
                Yield (key2.bond, partner)
            End If
        Next
    End Function

    ''' <summary>
    ''' Set atom group label based on the chemical keys
    ''' </summary>
    ''' <param name="formula"></param>
    Public Shared Sub SetAtomGroups(formula As ChemicalFormula)
        ' build connection edges
        For Each atom As ChemicalElement In formula.vertex
            Call SetAtomGroups(
                atom:=atom,
                keys:=Aggregate link
                      In GetConnection(formula, atom)
                      Into Sum(link.keys)
            )
        Next
    End Sub

    Private Sub setAtomLabel(label As String, nH As Integer)
        Dim atom As ChemicalElement = Me

        atom.group = label
        atom.hydrogen = nH
    End Sub

    Private Shared Sub CarbonGroup(ByRef atom As ChemicalElement, keys As Integer)
        If atom.aromatic Then
            atom.setAtomLabel("-CH-", 1)
        Else
            Select Case keys
                Case 1 : atom.setAtomLabel("-CH3", 3)
                Case 2 : atom.setAtomLabel("-CH2-", 2)
                Case 3 : atom.setAtomLabel("-CH=", 1)
                Case Else
                    atom.group = "C"
            End Select
        End If
    End Sub

    Private Shared Sub OxygenGroup(ByRef atom As ChemicalElement, keys As Integer)
        If atom.aromatic Then
            atom.setAtomLabel("-O-", 0)
        Else
            Select Case keys
                Case 1
                    If atom.charge = 0 Then
                        atom.setAtomLabel("-OH", 1)
                    Else
                        ' an ion with negative charge value
                        ' [O-]
                        atom.setAtomLabel("[O-]-", 0)
                    End If
                Case Else
                    ' 苯环氧杂环
                    ' 醚键或类似结构
                    ' 处理非常规键数（如keys=0或3）
                    atom.setAtomLabel("-O-", 0) ' 默认处理，可能需要细化
            End Select
        End If
    End Sub

    Private Shared Sub NitrogenGroup(ByRef atom As ChemicalElement, keys As Integer)
        Dim n As Integer

        ' N -3
        Select Case keys
            Case 1
                If atom.charge = 0 Then
                    atom.setAtomLabel("-NH2", 2)
                ElseIf atom.charge > 0 Then
                    n = 3 + atom.charge - keys
                    atom.setAtomLabel($"[-NH{n}]{atom.charge}+", n)
                Else
                    n = 3 - atom.charge
                    atom.setAtomLabel($"[-NH{n}]{atom.charge}", n)
                End If
            Case 2
                If atom.charge = 0 Then
                    atom.setAtomLabel("-NH-", 1)
                Else
                    n = 2 - atom.charge
                    atom.setAtomLabel($"[-NH{n}-]{atom.charge}+", n)
                End If
            Case 3
                If atom.charge = 0 Then
                    atom.group = "-N="
                Else
                    atom.group = $"[-N=]{atom.charge}+"
                End If
            Case 4
                If atom.charge = 1 Then
                    atom.setAtomLabel("[-NH4]+", 0)
                Else
                    atom.group = "N"
                End If
            Case Else
                atom.group = "N"
        End Select
    End Sub

    ''' <summary>
    ''' Set atom group label based on the chemical keys
    ''' </summary>
    ''' <param name="atom"></param>
    ''' <param name="keys"></param>
    Private Shared Sub SetAtomGroups(atom As ChemicalElement, keys As Integer)
        Select Case atom.elementName
            Case "C" : Call CarbonGroup(atom, keys)
            Case "O" : Call OxygenGroup(atom, keys)
            Case "N" : Call NitrogenGroup(atom, keys)

            Case Else
                Dim key As String = atom.elementName
                Dim group As AtomGroup

                If atom.elementName.StartsWith("["c) AndAlso atom.elementName.EndsWith("]"c) Then
                    ' is atom group
                    key = atom.elementName.GetStackValue("[", "]")

                    If AtomGroup.CheckDefaultLabel(key) Then
                        group = AtomGroup.AtomGroups(key)

                        If atom.charge = 0 Then
                            atom.charge = group.valence.OrderBy(Function(v) std.Abs(keys - v)).First
                        End If
                    End If
                End If

                If atom.charge > 0 Then
                    atom.group = $"[{key}]{atom.charge}+"
                Else
                    atom.group = $"[{key}]{-atom.charge}-"
                End If
        End Select
    End Sub
End Class
