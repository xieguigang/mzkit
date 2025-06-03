﻿#Region "Microsoft.VisualBasic::7541f7642f5b90ada6b9e56e7342f262, metadb\Chemoinformatics\Formula\Models\Formula.vb"

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

    '   Total Lines: 456
    '    Code Lines: 251 (55.04%)
    ' Comment Lines: 151 (33.11%)
    '    - Xml Docs: 98.01%
    ' 
    '   Blank Lines: 54 (11.84%)
    '     File Size: 17.79 KB


    '     Class Formula
    ' 
    '         Properties: AllAtomElements, Counts, CountsByElement, Elements, EmpiricalFormula
    '                     Empty, ExactMass, H
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: CanonicalFormula, CheckElement, CompareFormalCharge, EqualsTo, ToString
    '                   TryEvaluateExactMass
    '         Operators: (+4 Overloads) -, (+2 Overloads) *, /, (+4 Overloads) +, <>
    '                    =
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

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

        Public Shared ReadOnly Property H As Formula
            Get
                Return FormulaScanner.ScanFormula("H")
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
                Return TryEvaluateExactMass()
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
        ''' constructor will make a value copy of the input element composition <paramref name="counts"/> vector.
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

        ''' <summary>
        ''' make value copy of the given formula
        ''' </summary>
        ''' <param name="copy">make the copy of the element counts</param>
        ''' <remarks>
        ''' this constructor function will removes all elements with zero count.
        ''' </remarks>
        Sub New(copy As Formula)
            CountsByElement = New Dictionary(Of String, Integer)
            m_formula = copy.m_formula

            For Each element As KeyValuePair(Of String, Integer) In copy.CountsByElement
                If element.Value > 0 Then
                    Call CountsByElement.Add(element.Key, element.Value)
                End If
            Next
        End Sub

        ''' <summary>
        ''' construct a new empty formula object
        ''' </summary>
        ''' <remarks>
        ''' you can create a new formula string by adding new atom number 
        ''' profiles into atom composition <see cref="CountsByElement"/>.
        ''' </remarks>
        Sub New()
            CountsByElement = New Dictionary(Of String, Integer)
        End Sub

        ''' <summary>
        ''' Check of the specific element is inside current formula object?
        ''' </summary>
        ''' <param name="elementName"></param>
        ''' <returns>
        ''' this function returns true if the given element is existsed inside the
        ''' formula composition list and also the corresponding element counts 
        ''' is greater than zero. 
        ''' </returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CheckElement(elementName As String) As Boolean
            Return CountsByElement.ContainsKey(elementName) AndAlso CountsByElement(elementName) > 0
        End Function

        Public Function CanonicalFormula() As String
            Return Canonical.BuildCanonicalFormula(CountsByElement)
        End Function

        Private Function TryEvaluateExactMass() As Double
            Try
                Return Aggregate element
                   In CountsByElement
                   Let isotopic As Double = AllAtomElements(element.Key).isotopic
                   Let mass As Double = isotopic * element.Value
                   Into Sum(mass)
            Catch ex As Exception
                Dim notFound As String = CountsByElement _
                    .Keys _
                    .Where(Function(e)
                               Return Not AllAtomElements.ContainsKey(e)
                           End Function) _
                    .First

                Call $"Formula element key: '{notFound}' (inside {ToString()}) is not a valid atom element!".Warning
                Return -1
            End Try
        End Function

        ''' <summary>
        ''' show <see cref="EmpiricalFormula"/>
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return EmpiricalFormula
        End Function

        ''' <summary>
        ''' check the given formula <paramref name="fb"/> is equals to current formula object?
        ''' </summary>
        ''' <param name="fb"></param>
        ''' <param name="isIgnoreHydrogen"></param>
        ''' <returns></returns>
        Public Function EqualsTo(fb As Formula, Optional isIgnoreHydrogen As Boolean = False) As Boolean
            If Me Is fb Then
                Return True
            End If

            If Not isIgnoreHydrogen Then
                If Me!H <> fb!H Then
                    Return False
                End If
            End If

            If CountsByElement.Count <> fb.CountsByElement.Count Then
                Return False
            End If

            Dim union As Integer = CountsByElement.Keys.Union(fb.CountsByElement.Keys).Count

            If union <> CountsByElement.Count OrElse union <> fb.CountsByElement.Count Then
                ' has different element composition
                Return False
            End If

            For Each atom As String In CountsByElement.Keys
                If CountsByElement(atom) <> fb.CountsByElement(atom) Then
                    Return False
                End If
            Next

            Return True
        End Function

        ''' <summary>
        ''' Check of the two formula has formal charge 
        ''' </summary>
        ''' <param name="fb"></param>
        ''' <param name="deltaCharge"></param>
        ''' <returns>
        ''' does these two formula reference to a same compound but with different formal charge value?
        ''' false means these formula reference to different compound object
        ''' </returns>
        Public Function CompareFormalCharge(fb As Formula, Optional ByRef deltaCharge As Integer = Nothing) As Boolean
            If fb Is Nothing Then
                Return False
            ElseIf Me Is fb Then
                Return True
            End If

            Dim unionKeys As String() = CountsByElement.Keys.Union(fb.CountsByElement.Keys).ToArray
            Dim c1 = CountsByElement
            Dim c2 = fb.CountsByElement

            ' check for H, Na, Cl, K
            Static check_ions As Index(Of String) = {"H", "Na", "Cl", "K"}

            For Each atom As String In unionKeys
                If c1.ContainsKey(atom) AndAlso c2.ContainsKey(atom) Then
                    If c1(atom) <> c2(atom) Then
                        If atom <> "H" Then
                            Return False
                        Else
                            deltaCharge += std.Abs(c1(atom) - c2(atom))
                        End If
                    End If
                ElseIf atom Like check_ions Then
                    deltaCharge += std.Abs(c1.TryGetValue(atom) - c2.TryGetValue(atom))
                Else
                    Return False
                End If
            Next

            Return True
        End Function

        ''' <summary>
        ''' check <paramref name="f1"/> equals to <paramref name="f2"/> via <see cref="EqualsTo(Formula, Boolean)"/> function.
        ''' </summary>
        ''' <param name="f1"></param>
        ''' <param name="f2"></param>
        ''' <returns></returns>
        Public Shared Operator =(f1 As Formula, f2 As Formula) As Boolean
            If f1 Is Nothing OrElse f2 Is Nothing Then
                Return False
            End If

            Return f1.EqualsTo(f2)
        End Operator

        Public Shared Operator <>(f1 As Formula, f2 As Formula) As Boolean
            Return Not (f1 = f2)
        End Operator

        ''' <summary>
        ''' Multiply of the formula composition
        ''' </summary>
        ''' <param name="composition"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' this function will create a new formula object
        ''' </remarks>
        Public Shared Operator *(composition As Formula, n%) As Formula
            Dim newFormula$ = $"({composition}){n}"
            Dim newComposition = composition.CountsByElement _
                .ToDictionary(Function(e) e.Key,
                              Function(e)
                                  Return e.Value * n
                              End Function)

            Return New Formula(newComposition, newFormula)
        End Operator

        ''' <summary>
        ''' Multiply of the formula composition
        ''' </summary>
        ''' <param name="n"></param>
        ''' <param name="composition"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' this function will create a new formula object
        ''' </remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator *(n%, composition As Formula) As Formula
            Return composition * n
        End Operator

        ''' <summary>
        ''' make union of the atom count number profile <see cref="CountsByElement"/> 
        ''' between two given formula object.
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Public Shared Operator +(a As Formula, b As Formula) As Formula
            Dim newComposition As Dictionary(Of String, Integer)

            If a Is Nothing AndAlso b Is Nothing Then
                Return New Formula
            ElseIf a Is Nothing Then
                newComposition = b.CountsByElement
            ElseIf b Is Nothing Then
                newComposition = a.CountsByElement
            Else
                newComposition = a.CountsByElement.Keys _
                    .JoinIterates(b.CountsByElement.Keys) _
                    .Distinct _
                    .ToDictionary(Function(e) e,
                                  Function(e)
                                      Return a(e) + b(e)
                                  End Function)
            End If

            Return New Formula(newComposition)
        End Operator

        ''' <summary>
        ''' a - b will create a new formula composition.
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns>
        ''' the generated formula may contains the negative count element
        ''' </returns>
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

        ''' <summary>
        ''' Parse the formula string <paramref name="fs"/> and then substract the element counts from the given formula <paramref name="f"/>.
        ''' </summary>
        ''' <param name="f"></param>
        ''' <param name="fs"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' the generated formula may contains the negative count element
        ''' </remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator -(f As Formula, fs As String) As Formula
            Return f - FormulaScanner.ScanFormula(fs)
        End Operator

        ''' <summary>
        ''' Parse the formula string <paramref name="fs"/> and then add the element counts into the given formula <paramref name="f"/>.
        ''' </summary>
        ''' <param name="f"></param>
        ''' <param name="fs"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator +(f As Formula, fs As String) As Formula
            Return f + FormulaScanner.ScanFormula(fs)
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
