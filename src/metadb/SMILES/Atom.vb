#Region "Microsoft.VisualBasic::1067414cd9f38edfb36b4304daac631a, metadb\SMILES\Atom.vb"

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

'   Total Lines: 224
'    Code Lines: 163 (72.77%)
' Comment Lines: 12 (5.36%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 49 (21.88%)
'     File Size: 6.72 KB


' Class Atom
' 
'     Properties: AtomGroups, isAtomGroup, label, maxKeys, valence
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: ChargeLabel, DefaultAtomGroups, DefaultElements, EvaluateIsAtomGroup, GetIonLabel
'               GetMaxKeys, LoadAtoms, ToString
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports std = System.Math

Public Class AtomGroup : Inherits Atom

    Public Property mainElement As String

    Public Shared ReadOnly Property AtomGroups As Dictionary(Of String, AtomGroup) = LoadAtoms()

    Sub New(label As String, main As String, ParamArray valence As Integer())
        Call MyBase.New(label, valence)
        mainElement = main
    End Sub

    Private Shared Function LoadAtoms() As Dictionary(Of String, AtomGroup)
        Dim atoms = DefaultAtomGroups.ToArray
        Dim groups As New Dictionary(Of String, AtomGroup)

        For Each atom As AtomGroup In atoms
            Call groups.Add(atom.GetIonLabel, atom)

            If Not groups.ContainsKey(atom.label) Then
                Call groups.Add(atom.label, atom)
            End If
        Next

        Return groups
    End Function

    ''' <summary>
    ''' Check of the given atom group label is existsed inside the default atom group list
    ''' </summary>
    ''' <param name="label"></param>
    ''' <returns></returns>
    Public Shared Function CheckDefaultLabel(label As String) As Boolean
        Return _AtomGroups.ContainsKey(label)
    End Function

    Public Shared Function GetDefaultValence(label As String, val As Integer) As Integer
        If _AtomGroups.ContainsKey(label) Then
            Return _AtomGroups(label).valence(0)
        Else
            Return val
        End If
    End Function

    Public Shared Iterator Function DefaultAtomGroups() As IEnumerable(Of AtomGroup)
        Yield New AtomGroup("OH", "O", -1)
        Yield New AtomGroup("NO3", "N", -1)
        Yield New AtomGroup("SO4", "S", -2)
        Yield New AtomGroup("CO3", "C", -2)
        Yield New AtomGroup("NH4", "N", 1)
        Yield New AtomGroup("NH2", "N", 1, 2)
        Yield New AtomGroup("SO3", "S", -2)
        Yield New AtomGroup("MnO4", "Mn", -1)
        Yield New AtomGroup("HCO3", "C", -1)
        Yield New AtomGroup("PO4", "P", -3)
        Yield New AtomGroup("CH", "C", -1, -2, -3)
        Yield New AtomGroup("CH2", "C", -1, -2)
        Yield New AtomGroup("CH3", "C", -1)
        Yield New AtomGroup("CH4", "C", -1)
        Yield New AtomGroup("CH5", "C", -1) ' 氢化甲基阴离子（hydridomethyl anion）
        Yield New AtomGroup("COOH", "C", -1)
        Yield New AtomGroup("COO", "C", -2)
    End Function
End Class

''' <summary>
''' the atoms metadata for supports build formula and graph in SMILES parser
''' </summary>
Public Class Atom

    Public Property label As String

    ''' <summary>
    ''' Possible net charge value of current element atom
    ''' </summary>
    ''' <returns></returns>
    Public Property valence As Integer()

    ''' <summary>
    ''' the max number of the chemical keys
    ''' (max charge number)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property maxKeys As Integer
    Public ReadOnly Property isAtomGroup As Boolean

    Sub New(label As String, ParamArray valence As Integer())
        Me.isAtomGroup = EvaluateIsAtomGroup(label)
        Me.label = label
        Me.valence = valence
        Me.maxKeys = GetMaxKeys()
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function CheckSingleValence() As Boolean
        Return valence.TryCount = 1
    End Function

    Private Function GetMaxKeys() As Integer
        If valence.IsNullOrEmpty Then
            Return 0
        Else
            Return valence.Select(AddressOf std.Abs).Max
        End If
    End Function

    Public Shared Function EvaluateIsAtomGroup(label As String) As Boolean
        Dim countAtomGroup = Aggregate c As Char
                             In label
                             Where Char.IsLetter(c) AndAlso Char.IsUpper(c)
                             Into Count
        Return countAtomGroup > 1
    End Function

    Public Shared Function ChargeLabel(charge As Integer) As String
        If charge = 1 Then
            Return "+"
        ElseIf charge = -1 Then
            Return "-"
        ElseIf charge > 0 Then
            Return $"+{charge}"
        Else
            Return charge.ToString
        End If
    End Function

    Public Function GetIonLabel() As String
        If isAtomGroup Then
            Dim chargeLabel As String = Atom.ChargeLabel(valence(Scan0))
            Dim label As String = $"{Me.label}{chargeLabel}"

            Return label
        Else
            Return label
        End If
    End Function

    Public Overrides Function ToString() As String
        If isAtomGroup Then
            Return $"[{label}]{std.Abs(valence(Scan0))}{If(valence(Scan0) > 0, "+", "-")}"
        Else
            Return $"{label} ~ H{maxKeys}"
        End If
    End Function

    Public Shared Iterator Function DefaultElements() As IEnumerable(Of Atom)
        Yield New Atom("Li", 1)
        Yield New Atom("Na", 1)
        Yield New Atom("K", 1)
        Yield New Atom("Rb", 1)
        Yield New Atom("Cs", 1)
        Yield New Atom("Fr", 1)

        Yield New Atom("Be", 2)
        Yield New Atom("Mg", 2)
        Yield New Atom("Ca", 2)
        Yield New Atom("Zn", 2)
        Yield New Atom("Sr", 2)
        Yield New Atom("Cd", 2)
        Yield New Atom("Ba", 2)
        Yield New Atom("Ra", 2)

        Yield New Atom("Hg", 1, 2)

        Yield New Atom("Sc", 3)
        Yield New Atom("Ac", 3)
        Yield New Atom("Ga", 3)
        Yield New Atom("La", 3)
        Yield New Atom("Y", 3)
        Yield New Atom("B", 3)

        Yield New Atom("Bi", 3, 5)

        Yield New Atom("V", 2, 3, 4, 5)
        Yield New Atom("Nb", 2, 3, 4, 5)
        Yield New Atom("Ta", 2, 3, 4, 5)

        Yield New Atom("Pa", 3, 4, 5)

        Yield New Atom("Cr", 2, 3, 6)
        Yield New Atom("Po", 2, 4, 6)

        Yield New Atom("Ti", 2, 3, 4)
        Yield New Atom("Zr", 2, 3, 4)

        Yield New Atom("In", 1, 3)
        Yield New Atom("Tl", 1, 3)

        Yield New Atom("Cu", 1, 2, 3)
        Yield New Atom("Ag", 1, 2, 3)
        Yield New Atom("Au", 1, 3)

        Yield New Atom("H", 1, -1)

        Yield New Atom("Ce", 3, 4)
        Yield New Atom("Hf", 3, 4)
        Yield New Atom("Th", 3, 4)

        Yield New Atom("Co", 2, 3, 4)
        Yield New Atom("Ni", 2, 3, 4)
        Yield New Atom("Pd", 2, 3, 4)

        Yield New Atom("C", 2, 4)
        Yield New Atom("Si", 2, 4)
        Yield New Atom("Pb", 2, 4)
        Yield New Atom("Sn", 2, 4)

        Yield New Atom("Rh", 2, 3, 4, 5, 6)
        Yield New Atom("Ir", 2, 3, 4, 5, 6)
        Yield New Atom("Pt", 2, 3, 4, 5, 6)

        Yield New Atom("F", -1)

        Yield New Atom("Al", 3)

        Yield New Atom("Tc", 4, 5, 6, 7)
        Yield New Atom("Re", 4, 5, 6, 7)

        Yield New Atom("Np", 3, 4, 5, 6, 7)
        Yield New Atom("Pu", 3, 4, 5, 6, 7)

        Yield New Atom("O", -2, -1, 2)

        Yield New Atom("S", -2, 2, 4, 6)
        Yield New Atom("Se", -2, 2, 4, 6)
        Yield New Atom("Te", -2, 2, 4, 6)

        Yield New Atom("N", -3, 1, 2, 3, 4, 5)
        Yield New Atom("P", -3, 1, 3, 4, 5)

        Yield New Atom("As", -3, 3, 5)
        Yield New Atom("Sb", -3, 3, 5)

        Yield New Atom("U", 3, 4, 5, 6)

        Yield New Atom("W", 2, 3, 4, 5, 6)
        Yield New Atom("Mo", 2, 3, 4, 5, 6)

        Yield New Atom("Mn", 2, 3, 4, 6, 7)

        Yield New Atom("Ru", 2, 3, 4, 5, 6, 7, 8)
        Yield New Atom("Xe", 1, 4, 6, 8)

        Yield New Atom("Br", -1, 1, 3, 5, 7)
        Yield New Atom("I", -1, 1, 3, 5, 7)
        Yield New Atom("Cl", -1, 1, 3, 4, 5, 6, 7)

        Yield New Atom("Fe", 2, 3, 4, 5, 6, 8)
        Yield New Atom("Os", 2, 3, 4, 5, 6, 8)

        Yield New Atom("Ge", -4, 2, 4)
        Yield New Atom("Sm", 2, 3)
    End Function

End Class
