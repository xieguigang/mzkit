#Region "Microsoft.VisualBasic::fe756d1ba260f881fc582463960b2eff, metadb\SMILES\Language\Scanner.vb"

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

    '   Total Lines: 313
    '    Code Lines: 172 (54.95%)
    ' Comment Lines: 109 (34.82%)
    '    - Xml Docs: 15.60%
    ' 
    '   Blank Lines: 32 (10.22%)
    '     File Size: 15.11 KB


    '     Class Scanner
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetTokens, GetTokensInternal, MeasureElement, WalkChar
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Text.Parser

Namespace Language

    Public Class Scanner

        Dim SMILES As CharPtr
        Dim buf As New CharBuffer
        Dim openIonStack As Boolean = False

        ' structure information
        '
        ' 1. Configuration Around Double Bonds
        '
        ' Configuration around double bonds is specified by the characters / and \ which
        ' are "directional bonds" and can be thought of as kinds of single or aromatic
        ' (eg. default) bonds. These symbols indicate relative directionality between the
        ' connected atoms, and have meaning only when they occur on both atoms which are
        ' double bonded. For instance, the following SMILES are all valid for E- and
        ' Z-1,2-difluoroethene:
        '
        ' F/C=C/F	F/C=C\F
        ' F\C=C\F	F\C=C/F
        '
        ' An important difference between SMILES chirality conventions and others such as
        ' CIP is that SMILES uses local chirality representation (as opposed to absolute
        ' chirality), which allows partial specifications. An example of this is illustrated
        ' below:
        '
        ' 2. Configuration Around Tetrahedral Centers
        '
        ' SMILES uses a very general type of chirality specification based on local chirality.
        ' Instead of using a rule-based numbering scheme to order neighbor atoms of a chiral
        ' center, orientations are based on the order in which neighbors occur in the SMILES
        ' string. As with all other aspects of SMILES, any valid order is acceptable; the
        ' Daylight software is responsible for retaining the meaning of the chiral specification
        ' when the structure is modified or rearranged (e.g. to make the unique SMILES).

        ' The simplest and most common kind of chirality is tetrahedral; four neighbor atoms
        ' are evenly arranged about a central atom, known as the "chiral center". If all four
        ' neighbors are different from each other in any way, mirror images of the structure
        ' will not be identical. The two mirror images are known as "enantiomers" and are the
        ' only two forms that a tetrahedral center can have. If two (or more) of the four
        ' neighbors are identical to each other, the central atom will not be chiral (its
        ' mirror images can be superimposed in space).
        '
        ' In SMILES, tetrahedral centers may be indicated by a simplified chiral specification
        ' (@ or @@) written as an atomic property following the atomic symbol of the chiral
        ' atom. If a chiral specification is not present for a chiral atom, its chirality is
        ' implicitly not specified. For instance:
        '
        ' NC(C)(F)C(=O)O	N[C@](C)(F)C(=O)O
        ' NC(F)(C)C(=O)O	N[C@@](F)(C)C(=O)O
        ' (unspecified chirality)	(specified chirality)
        '
        ' Looking from the amino N to the chiral C (as the SMILES is written), the three other
        ' neighbors appear anticlockwise in the order that they are written in the top SMILES,
        ' N[C@](C)(F)C(=O)O (methyl-C, F, carboxy-C), and clockwise in the bottom one,
        ' N[C@@](F)(C)C(=O)O. The symbol "@" indicates that the following neighbors are listed
        ' anticlockwise (it is a "visual mnemonic" in that the symbol looks like an anticlockwise
        ' spiral around a central circle). "@@" indicates that the neighbors are listed clockwise
        ' (you guessed it, anti-anti-clockwise).
        '
        ' If the central carbon is not the very first atom in the SMILES and has an implicit
        ' hydrogen attached (it can have at most one and still be chiral), the implicit hydrogen
        ' is taken to be the first neighbor atom of the three neighbors that follow a tetrahedral
        ' specification. If the central carbon is first in the SMILES, the implicit hydrogen is
        ' taken to be the "from" atom. Hydrogens may always be written explicitly (as [H]) in
        ' which case they are treated like any other atom. In each case, the implied order is
        ' exactly as written in SMILES. Some of the valid SMILES for the alanine are:
        '
        ' N[C@@]([H])(C)C(=O)O	N[C@]([H])(C)C(=O)O
        ' N[C@@H](C)C(=O)O	N[C@H](C)C(=O)O
        ' N[C@H](C(=O)O)C	N[C@@H](C(=O)O)C
        ' [H][C@](N)(C)C(=O)O	[H][C@@](N)(C)C(=O)O
        ' [C@H](N)(C)C(=O)O	[C@@H](N)(C)C(=O)O

        ' The chiral order of the ring closure bond is implied by the lexical order that the ring
        ' closure digit appears on the chiral atom (not in the lexical order of the "substituent"
        ' atom).
        '
        ' C[C@H]1CCCCO1 or O1CCCC[C@@H]1C

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <DebuggerStepThrough>
        Sub New(SMILES As String)
            ' removes all of the structure related information
            Me.SMILES = SMILES _
                .Replace("@", "") _
                .Replace("\", "") _
                .Replace("/", "")
        End Sub

        ''' <summary>
        ''' Parse SMILES tokens
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function GetTokens() As IEnumerable(Of Token)
            For Each t As Token In GetTokensInternal()
                If TypeOf t Is MultipleTokens Then
                    For Each ti As Token In DirectCast(t, MultipleTokens).Multiple
                        Yield ti
                    Next
                Else
                    Yield t
                End If
            Next
        End Function

        ''' <summary>
        ''' Parse SMILES tokens
        ''' </summary>
        ''' <returns></returns>
        Private Iterator Function GetTokensInternal() As IEnumerable(Of Token)
            Do While Not SMILES.EndRead
                For Each t As Token In WalkChar(++SMILES)
                    Yield t
                Next
            Loop

            If buf > 0 Then
                Yield MeasureElement(New String(buf.PopAllChars))
            End If
        End Function

        Private Iterator Function WalkChar(c As Char) As IEnumerable(Of Token)
            If c = "("c OrElse c = ")"c OrElse c = "."c Then
                If buf > 0 Then
                    Yield MeasureElement(New String(buf.PopAllChars))
                End If

                Yield MeasureElement(c)
            ElseIf (Not openIonStack) AndAlso c Like ChemicalBonds Then
                ' negative charge symbol - 
                ' also means a kind of chemical bound
                ' so use a flag openIonStack to distingush
                ' such different
                If buf > 0 Then
                    Yield MeasureElement(New String(buf.PopAllChars))
                End If

                Yield New Token(ElementTypes.Key, c)
            Else
                If Char.IsLetter(c) AndAlso (buf > 0) Then
                    ' handling the parser for token like 'c3OC4c', 'ccc'
                    If (Char.IsUpper(c) OrElse
                        (Char.IsLower(c) AndAlso Char.IsNumber(buf.Last))) OrElse
                        (Char.IsLower(c) AndAlso Char.IsLower(buf.GetChar(0))) Then

                        If buf(Scan0) = "["c Then
                            openIonStack = True
                            Call Debug.WriteLine("[")
                        Else
                            Yield MeasureElement(New String(buf.PopAllChars))
                        End If
                    ElseIf Char.IsLower(c) AndAlso buf.Size = 2 AndAlso atoms.ContainsKey(buf.ToString) Then
                        ' atom|c
                        Yield MeasureElement(New String(buf.PopAllChars))
                        Yield MeasureElement(c.ToString)
                    End If
                ElseIf c = "]"c Then
                    buf += c
                    openIonStack = False

                    Dim tmpStr = New String(buf.PopAllChars)
                    Dim isIon As Boolean = tmpStr.First = "["c AndAlso tmpStr.Last = "]"c
                    Dim charge As String = tmpStr.Match("(\d?[+-])|([+-]\d?)")
                    Dim chargeVal As Integer? = Nothing

                    If Not charge.StringEmpty Then
                        If charge = "+" Then
                            chargeVal = 1
                        ElseIf charge = "-" Then
                            chargeVal = -1
                        ElseIf charge.Last = "-"c Then
                            chargeVal = CInt(-Val(charge))
                        ElseIf charge.First = "-"c Then
                            chargeVal = CInt(Val(charge))
                        Else
                            chargeVal = CInt(Val(charge.Replace("+", "")))
                        End If
                    End If

                    tmpStr = tmpStr.GetStackValue("[", "]")

                    ' handling some special ion group
                    If AtomGroup.CheckDefaultLabel(tmpStr) Then
                        If charge.StringEmpty Then
                            chargeVal = AtomGroup.GetDefaultValence(tmpStr, -1)
                        End If
                        Yield New Token(ElementTypes.AtomGroup, tmpStr) With {.charge = chargeVal}
                        Return
                    ElseIf Not charge.StringEmpty Then
                        tmpStr = tmpStr.Replace(charge, "")
                    End If

                    Dim tmp As String = ""

                    For Each c In tmpStr
                        If Char.IsUpper(c) Then
                            If tmp.Length > 0 Then
                                Dim ion = MeasureElement(tmp)
                                ion.charge = chargeVal
                                Yield ion
                            End If

                            tmp = c
                        Else
                            tmp = tmp & c
                        End If
                    Next

                    If tmp.Length > 0 Then
                        Dim ion = MeasureElement(tmp)
                        ion.charge = chargeVal
                        Yield ion
                    End If

                    Return
                ElseIf c = "["c Then
                    openIonStack = True

                    If buf > 0 Then
                        Yield MeasureElement(New String(buf.PopAllChars))
                    End If
                End If

                buf += c
            End If
        End Function

        Shared ReadOnly atoms As Dictionary(Of String, Element) = Element.MemoryLoadElements

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="str"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Atoms in aromatic rings are specified by lower case letters, e.g., 
        ''' 
        ''' 1. aliphatic carbon is represented by the capital letter C, 
        ''' 2. aromatic carbon by lower case c.
        ''' </remarks>
        Private Function MeasureElement(str As String) As Token
            Dim ring As Integer? = Nothing

            If str.Length >= 3 AndAlso (str.First = "["c AndAlso str.Last = "]"c) Then
                ' [H]
                str = str.GetStackValue("[", "]")
            End If
            If str.IsPattern("[A-Za-z]+\d+") Then
                ' removes number
                ring = Integer.Parse(str.Match("\d+"))
                str = str.Match("[a-zA-Z]+")
            ElseIf str.IsPattern("\d+[A-Za-z]+") Then
                ' 0C unsure how to parse it
                str = str.Match("[a-zA-Z]+")
            End If

            Select Case str
                Case "("
                    Return New Token(ElementTypes.Open, str)
                Case ")"
                    Return New Token(ElementTypes.Close, str)
                Case "."
                    Return New Token(ElementTypes.Disconnected, str)
                Case ""
                    Return New Token(ElementTypes.None, str)
                Case Else
                    ' 在 SMILES（简化分子输入线进入系统）表示法中，CCCCCC 和 cccccc 分别代表不同的化学结构：
                    ' 1. CCCCCC：这代表六个连续的饱和碳原子，每个碳原子之间都是单键连接。在化学式中，这通常表示为 CH3-CH2-CH2-CH2-CH2-CH3，这是己烷的化学式。己烷是一种无色、无味的液体，在常温常压下为液态，广泛用作溶剂和燃料。
                    ' 2. cccccc：这代表六个连续的芳香族碳原子，通常指的是苯环结构中的一部分。在化学式中，苯环由六个碳原子组成，形成一个六元环C6H6，每个碳原子之间交替有一个单键和一个双键。cccccc 表示一个完整的苯环。苯是一种无色液体，具有特殊的芳香气味，广泛用于化学工业中。
                    Static aromatic As Index(Of String) = {"c", "o", "n", "s", "p"}

                    If Layout2D.atomMaxCharges.ContainsKey(str) Then
                        Return New Token(ElementTypes.Element, str) With {
                            .ring = ring
                        }
                    ElseIf str.IsPattern("\d+") Then
                        Return New Token(ElementTypes.None, str)
                    ElseIf str Like aromatic Then
                        ' aromatic carbon by lower case c.
                        Return New Token(ElementTypes.Element, str.ToUpper) With {
                            .aromatic = True
                        }
                    ElseIf atoms.ContainsKey(str) Then
                        ' Au/Cu/Na/Cl elements
                        Return New Token(ElementTypes.Element, str)
                    ElseIf str.Last Like aromatic Then
                        Dim xxx As New MultipleTokens

                        ' example like token string: Oc
                        ' its name pattern has pass the test of multiple character atom element label,
                        ' example as Oc is the same string pattern as Cu/Au/Li
                        ' but Oc actually be composed by two element O and C aromatic carbon
                        For Each c As Char In str
                            Call xxx.Multiple.Add(MeasureElement(c))
                        Next

                        Return xxx
                    ElseIf str = "E" OrElse str = "Z" Then
                        Return New Token(ElementTypes.Isomers, str)
                    Else
                        Throw New NotImplementedException("Unknown token text for measure element type: " & str)
                    End If
            End Select
        End Function
    End Class
End Namespace
