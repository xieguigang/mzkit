#Region "Microsoft.VisualBasic::3e707c6684ef3205a926abcf7ea7e4ba, metadb\SMILES\Atom.vb"

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

    '   Total Lines: 193
    '    Code Lines: 137 (70.98%)
    ' Comment Lines: 12 (6.22%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 44 (22.80%)
    '     File Size: 5.61 KB


    ' Class Atom
    ' 
    '     Properties: isAtomGroup, label, maxKeys, valence
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ChargeLabel, CheckSingleValence, DefaultElements, EvaluateIsAtomGroup, GetIonLabel
    '               GetMaxKeys, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports std = System.Math

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

        Yield New Atom("Ag", 1, 2, 3)
        Yield New Atom("Au", 1, 3)

        Yield New Atom("Ce", 3, 4)
        Yield New Atom("Hf", 3, 4)
        Yield New Atom("Th", 3, 4)

        Yield New Atom("Co", 2, 3, 4)
        Yield New Atom("Ni", 2, 3, 4)
        Yield New Atom("Pd", 2, 3, 4)

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

        Yield New Atom("Se", -2, 2, 4, 6)
        Yield New Atom("Te", -2, 2, 4, 6)

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

        Yield New Atom("Os", 2, 3, 4, 5, 6, 8)

        Yield New Atom("Gd", 2, 3)

        Yield New Atom("Ge", -4, 2, 4)
        Yield New Atom("Sm", 2, 3)

        ' --- 1. 氢 ---
        ' 常见 +1 价 (酸、水、有机物)，在金属氢化物 (如 NaH) 中显 -1 价
        Yield New Atom("H", 1, -1)

        ' --- 2. 碳 ---
        ' 有机化学核心元素。
        ' 常见化合价：-4 (CH4), +2 (CO), +4 (CO2)
        Yield New Atom("C", -4, 4, 2)

        ' --- 3. 氧 ---
        ' 非常活泼，通常显 -2 价 (水、氧化物)。
        ' -1 价存在于过氧化物 (如 H2O2)，+2 价极少见 (如 OF2)
        Yield New Atom("O", -2, -1, 2)

        ' --- 4. 氮 ---
        ' 化合价变化非常丰富。
        ' -3 (NH3/铵根), +5 (HNO3/硝酸盐), +3 (HNO2/亚硝酸盐), +1/+2/+4 (氧化物)
        Yield New Atom("N", -3, 5, 3, 1, 2, 4)

        ' --- 5. 磷 ---
        ' 与氮同族。
        ' -3 (PH3/磷化物), +5 (H3PO4/磷酸盐), +3 (H3PO3/亚磷酸盐)
        Yield New Atom("P", -3, 5, 3)

        ' --- 6. 硫 ---
        ' -2 (H2S/硫化物), +6 (H2SO4/硫酸盐), +4 (SO2/亚硫酸盐)
        Yield New Atom("S", -2, 6, 4, 2)

        ' --- 7. 钾 ---
        ' 碱金属，极活泼，几乎只有 +1 价
        Yield New Atom("K", 1)

        ' --- 8. 钠 ---
        ' 碱金属，极活泼，几乎只有 +1 价
        Yield New Atom("Na", 1)

        ' --- 9. 氯 ---
        ' 卤素。
        ' -1 (HCl/氯化物)，正价常见于含氧酸盐：+1 (次氯酸), +5 (氯酸), +7 (高氯酸), +3 (亚氯酸)
        Yield New Atom("Cl", -1, 1, 5, 7, 3)

        ' --- 10. 铁 ---
        ' 过渡金属。
        ' +2 (亚铁离子 Fe2+), +3 (三价铁离子 Fe3+)
        Yield New Atom("Fe", 2, 3)

        ' --- 11. 铜 ---
        ' 过渡金属。
        ' +2 (铜离子 Cu2+，最常见), +1 (亚铜离子 Cu+，如 Cu2O)
        Yield New Atom("Cu", 2, 1)
    End Function

End Class
