#Region "Microsoft.VisualBasic::ff5d4a850732b77a2c993116b5ab991b, mzkit\src\metadb\Chemoinformatics\Formula\Models\Formula.vb"

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

'   Total Lines: 203
'    Code Lines: 124
' Comment Lines: 52
'   Blank Lines: 27
'     File Size: 7.71 KB


'     Class Formula
' 
'         Properties: AllAtomElements, Counts, CountsByElement, Elements, EmpiricalFormula
'                     ExactMass
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: ToString
'         Operators: (+3 Overloads) -, (+2 Overloads) *, /, (+3 Overloads) +
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports Microsoft.VisualBasic.Linq

Namespace Formula

    ''' <summary>
    ''' A chemical formula is a notation used by scientists to show the 
    ''' number and type of atoms present in a molecule, using the atomic 
    ''' symbols and numerical subscripts. A chemical formula is a simple 
    ''' representation, in writing, of a three dimensional molecule 
    ''' that exists. A chemical formula describes a substance, down to 
    ''' the exact atoms which make it up.
    ''' </summary>
    <DebuggerDisplay("{EmpiricalFormula} ({ExactMass} = {Counts})")>
    Public Class Formula : Implements IExactMassProvider

        ''' <summary>
        ''' atom_count_tuples
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property CountsByElement As Dictionary(Of String, Integer)

        ''' <summary>
        ''' get formula string
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' this formula string data is build with the 
        ''' <see cref="BuildCanonicalFormula"/> function 
        ''' or user specific input
        ''' </remarks>
        Public ReadOnly Property EmpiricalFormula As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return m_formula
            End Get
        End Property

        Protected Friend m_formula As String = ""

        ''' <summary>
        ''' get all elements label that parse from current formula object
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Elements As String()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return CountsByElement.Keys.ToArray
            End Get
        End Property

        Public Shared ReadOnly Property Empty As Formula
            Get
                Return New Formula(New Dictionary(Of String, Integer), "")
            End Get
        End Property

        Public Shared ReadOnly Property AllAtomElements As IReadOnlyDictionary(Of String, Element) = Element.MemoryLoadElements

        ''' <summary>
        ''' get element count by the given specific atom label
        ''' </summary>
        ''' <param name="atom"></param>
        ''' <returns>
        ''' this default property returns ZERO if the given 
        ''' <paramref name="atom"/> is not found in current 
        ''' formula
        ''' </returns>
        Default Public ReadOnly Property GetAtomCount(atom As String) As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                If Not CountsByElement.ContainsKey(atom) Then
                    Return 0
                Else
                    Return CountsByElement(atom)
                End If
            End Get
        End Property

        ''' <summary>
        ''' sum all isotopic mass of the atom elements. 
        ''' </summary>
        ''' <returns></returns>
        Public Overridable ReadOnly Property ExactMass As Double Implements IExactMassProvider.ExactMass
            Get
                Try
                    Return Aggregate element
                       In CountsByElement
                       Let isotopic As Double = AllAtomElements(element.Key).isotopic
                       Let mass As Double = isotopic * element.Value
                       Into Sum(mass)
                Catch ex As Exception
                    Call $"element key: '{CountsByElement.Keys.Where(Function(e) Not AllAtomElements.ContainsKey(e)).First}' is not exists in hash table!".Warning
                    Return -1
                End Try

            End Get
        End Property

        Friend ReadOnly Property Counts As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return CountsByElement _
                    .Select(Function(a) $"{a.Key}: {a.Value}") _
                    .JoinBy(", ")
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="counts">
        ''' constructor will make a value copy of this element composition vector.
        ''' </param>
        ''' <param name="formula"></param>
        Sub New(counts As IDictionary(Of String, Integer), Optional formula$ = Nothing)
            CountsByElement = New Dictionary(Of String, Integer)(counts)

            If formula.StringEmpty AndAlso Not CountsByElement.Count = 0 Then
                Me.m_formula = Canonical.BuildCanonicalFormula(CountsByElement)
            Else
                Me.m_formula = formula
            End If
        End Sub

        Sub New()
            CountsByElement = New Dictionary(Of String, Integer)
        End Sub

        ''' <summary>
        ''' show <see cref="EmpiricalFormula"/>
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return EmpiricalFormula
        End Function

        Public Function EqualsTo(fb As Formula, Optional isIgnoreHydrogen As Boolean = False) As Boolean
            If Not isIgnoreHydrogen Then
                If Me!H <> fb!H Then
                    Return False
                End If
            End If

            If CountsByElement.Keys.Union(fb.CountsByElement.Keys).Count <> CountsByElement.Count Then
                ' has different element composition
                Return False
            End If

            For Each atom As String In CountsByElement.Keys
                If CountsByElement(atom) <> fb.CountsByElement(fb) Then
                    Return False
                End If
            Next

            Return True
        End Function

        Public Shared Operator *(composition As Formula, n%) As Formula
            Dim newFormula$ = $"({composition}){n}"
            Dim newComposition = composition.CountsByElement _
                .ToDictionary(Function(e) e.Key,
                              Function(e)
                                  Return e.Value * n
                              End Function)

            Return New Formula(newComposition, newFormula)
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator *(n%, composition As Formula) As Formula
            Return composition * n
        End Operator

        Public Shared Operator +(a As Formula, b As Formula) As Formula
            Dim newComposition = a.CountsByElement.Keys _
                .JoinIterates(b.CountsByElement.Keys) _
                .Distinct _
                .ToDictionary(Function(e) e,
                              Function(e)
                                  Return a(e) + b(e)
                              End Function)

            Return New Formula(newComposition)
        End Operator

        ''' <summary>
        ''' a - b will create a new formula composition.
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Public Shared Operator -(a As Formula, b As Formula) As Formula
            Dim newComposition = a.CountsByElement.Keys _
                .JoinIterates(b.CountsByElement.Keys) _
                .Distinct _
                .Select(Function(e)
                            Return (e, n:=a.CountsByElement.TryGetValue(e) - b.CountsByElement.TryGetValue(e))
                        End Function) _
                .Where(Function(e) e.n <> 0) _
                .ToDictionary(Function(e) e.e,
                              Function(e)
                                  Return e.n
                              End Function)

            Return New Formula(newComposition)
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator -(f As Formula, mass As Double) As Double
            Return f.ExactMass - mass
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator -(mass As Double, f As Formula) As Double
            Return mass - f.ExactMass
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator +(mass As Double, f As Formula) As Double
            Return mass + f.ExactMass
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator +(f As Formula, mass As Double) As Double
            Return f.ExactMass + mass
        End Operator

        Public Shared Operator /(f As Formula, n As Integer) As Formula
            Dim newComposition = f.CountsByElement _
                .ToDictionary(Function(e) e.Key,
                              Function(e)
                                  Return CInt(e.Value / n)
                              End Function)

            Return New Formula(newComposition)
        End Operator

        ''' <summary>
        ''' safe get the extract mass from the formula object
        ''' </summary>
        ''' <param name="f"></param>
        ''' <returns>
        ''' this function may returns -1 if the given formula is nothing
        ''' </returns>
        Public Shared Narrowing Operator CType(f As Formula) As Double
            If f Is Nothing Then
                Return -1
            Else
                Return f.ExactMass
            End If
        End Operator
    End Class
End Namespace
