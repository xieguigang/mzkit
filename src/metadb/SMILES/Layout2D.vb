﻿#Region "Microsoft.VisualBasic::115e59fe77c7e02293031acfffffdc90, metadb\SMILES\Layout2D.vb"

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

    '   Total Lines: 132
    '    Code Lines: 97 (73.48%)
    ' Comment Lines: 9 (6.82%)
    '    - Xml Docs: 33.33%
    ' 
    '   Blank Lines: 26 (19.70%)
    '     File Size: 4.38 KB


    ' Module Layout2D
    ' 
    '     Function: AutoLayout, EvaluateAngleDelta
    ' 
    '     Sub: LayoutTarget
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

''' <summary>
''' methods for evaluate 2D layout of the molecule atoms
''' </summary>
Module Layout2D

    <Extension>
    Public Function AutoLayout(chemical As ChemicalFormula,
                               Optional radius As Double = 10,
                               Optional strict As Boolean = True) As ChemicalFormula

        Dim atom As ChemicalElement = chemical.AllElements.FirstOrDefault

        If Not chemical.AllElements.Any Then
            Return chemical
        Else
            atom.coordinate = New Double() {0, 0}
        End If

        Do While True
            chemical.LayoutTarget(atom, radius, 0, strict)
            atom = chemical _
                .AllElements _
                .Where(Function(a) a.coordinate.IsNullOrEmpty) _
                .FirstOrDefault

            If atom Is Nothing Then
                Exit Do
            End If
        Loop

        Return chemical
    End Function

    Friend ReadOnly atomMaxCharges As Dictionary(Of String, Atom) = Atom _
        .DefaultElements _
        .JoinIterates(AtomGroup.DefaultAtomGroups) _
        .ToDictionary(Function(a)
                          Return a.label
                      End Function)

    Private Function EvaluateAngleDelta(atom As ChemicalElement, bonds As ChemicalKey(),
                                        ByRef n As Integer,
                                        strict As Boolean) As Double

        Dim maxN As Integer
        Dim key As String = atom.elementName

        If atom.elementName.StartsWith("["c) AndAlso atom.elementName.EndsWith("]"c) Then
            key = atom.elementName.GetStackValue("[", "]")
        End If

        If atomMaxCharges.ContainsKey(key) Then
            maxN = atomMaxCharges(key).maxKeys
        ElseIf AtomGroup.CheckDefaultLabel(key) Then
            maxN = AtomGroup.AtomGroups(key).maxKeys
        Else
            maxN = 1
        End If

        n = Aggregate b As ChemicalKey
            In bonds
            Into Sum(b.bond)

        If bonds.Length > maxN OrElse (n > maxN) Then
            Dim msg As String = $"The atom element '{atom.elementName}' its max key is {maxN}, but {n} bounds is connected with this atom element!"

            If strict Then
                Throw New InvalidConstraintException(msg)
            Else
                Call msg.Warning

                Return 2 * std.PI / n
            End If
        End If

        ' fix for the missing H element
        ' bonds in SMILES
        n = maxN - n
        n += bonds.Length

        Return 2 * std.PI / n
    End Function

    <Extension>
    Public Sub LayoutTarget(chemical As ChemicalFormula, atom As ChemicalElement, radius As Double, alpha As Double,
                            Optional strict As Boolean = True)

        ' get number of bounds (n) of
        ' the current atom links,
        ' then we can evaluate the angle
        ' of each bond, which is 360/n.
        Dim bonds As ChemicalKey() = chemical.AllBonds _
            .Where(Function(b)
                       Return b.U Is atom OrElse b.V Is atom
                   End Function) _
            .ToArray
        Dim n As Integer = 0
        Dim angleDelta As Double = EvaluateAngleDelta(atom, bonds, n, strict)

        If alpha = 0 Then
            If atom.elementName = "C" AndAlso bonds.Length = 1 Then
                alpha = 2 * std.PI * 2 / 3
            Else
                alpha = angleDelta
            End If
        End If

        If atom.coordinate.IsNullOrEmpty Then
            atom.coordinate = {0, 0}
        End If

        Dim center As New PointF(atom.coordinate(0), atom.coordinate(1))

        For Each bond As ChemicalKey In From b In bonds Where b.U Is atom
            Dim [next] As ChemicalElement = bond.V
            Dim layout As New PointF With {
                .X = center.X + (radius * std.Cos(alpha)),
                .Y = center.Y + (radius * std.Sin(alpha))
            }

            [next].coordinate = {layout.X, layout.Y}
            alpha += angleDelta

            Call chemical.LayoutTarget([next], radius, alpha, strict)
        Next
    End Sub
End Module
