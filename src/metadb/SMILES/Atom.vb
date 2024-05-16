#Region "Microsoft.VisualBasic::58ad4d2cbb16775f09a142d6af0384f3, metadb\SMILES\Atom.vb"

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

    '   Total Lines: 202
    '    Code Lines: 148
    ' Comment Lines: 9
    '   Blank Lines: 45
    '     File Size: 5.96 KB


    ' Class Atom
    ' 
    '     Properties: AtomGroups, isAtomGroup, label, maxKeys, valence
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ChargeLabel, DefaultAtomGroups, DefaultElements, EvaluateIsAtomGroup, GetIonLabel
    '               GetMaxKeys, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports stdNum = System.Math

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

    Public Shared ReadOnly Property AtomGroups As Dictionary(Of String, Atom) = Atom _
        .DefaultAtomGroups _
        .ToDictionary(Function(a)
                          Return a.GetIonLabel
                      End Function)

    Sub New(label As String, ParamArray valence As Integer())
        Me.isAtomGroup = EvaluateIsAtomGroup(label)
        Me.label = label
        Me.valence = valence
        Me.maxKeys = GetMaxKeys()
    End Sub

    Private Function GetMaxKeys() As Integer
        If valence.IsNullOrEmpty Then
            Return 0
        Else
            Return valence.Select(AddressOf stdNum.Abs).Max
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
            Return $"[{label}]{stdNum.Abs(valence(Scan0))}{If(valence(Scan0) > 0, "+", "-")}"
        Else
            Return $"{label} ~ H{maxKeys}"
        End If
    End Function

    Public Shared Iterator Function DefaultAtomGroups() As IEnumerable(Of Atom)
        Yield New Atom("OH", -1)
        Yield New Atom("NO3", -1)
        Yield New Atom("SO4", -2)
        Yield New Atom("CO3", -2)
        Yield New Atom("NH4", 1)
        Yield New Atom("NH2", 1, 2)
        Yield New Atom("SO3", -2)
        Yield New Atom("MnO4", -1)
        Yield New Atom("HCO3", -1)
        Yield New Atom("PO4", -3)
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
    End Function

End Class
