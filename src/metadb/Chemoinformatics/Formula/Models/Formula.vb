#Region "Microsoft.VisualBasic::afecfe38c11908522708632754dde495, src\metadb\Chemoinformatics\Formula\Models\Formula.vb"

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

    '     Class Formula
    ' 
    '         Properties: Counts, CountsByElement, Elements, EmpiricalFormula, ExactMass
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: BuildFormula, ToString
    '         Operators: (+3 Overloads) -, (+2 Overloads) *, /, (+3 Overloads) +
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace Formula

    <DebuggerDisplay("{EmpiricalFormula} ({ExactMass} = {Counts})")>
    Public Class Formula

        ''' <summary>
        ''' atom_count_tuples
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property CountsByElement As Dictionary(Of String, Integer)
        Public ReadOnly Property EmpiricalFormula As String
            Get
                Return m_formula
            End Get
        End Property

        Friend m_formula As String

        Public Shared ReadOnly Property Elements As IReadOnlyDictionary(Of String, Element) = Element.MemoryLoadElements

        Default Public ReadOnly Property GetAtomCount(atom As String) As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return CountsByElement.TryGetValue(atom)
            End Get
        End Property

        ''' <summary>
        ''' sum all isotopic mass of the atom elements. 
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ExactMass As Double
            Get
                Return Aggregate element
                       In CountsByElement
                       Let mass As Double = Elements(element.Key).isotopic * element.Value
                       Into Sum(mass)
            End Get
        End Property

        Friend ReadOnly Property Counts As String
            Get
                Return CountsByElement.Select(Function(a) $"{a.Key}: {a.Value}").JoinBy(", ")
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

            If formula.StringEmpty Then
                Me.m_formula = BuildFormula(CountsByElement)
            Else
                Me.m_formula = formula
            End If
        End Sub

        Public Shared Function BuildFormula(countsByElement As Dictionary(Of String, Integer)) As String
            Return countsByElement _
                .Where(Function(e) e.Value > 0) _
                .Select(Function(e)
                            Return If(e.Value = 1, e.Key, e.Key & e.Value)
                        End Function) _
                .JoinBy("")
        End Function

        ''' <summary>
        ''' show <see cref="EmpiricalFormula"/>
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return EmpiricalFormula
        End Function

        Public Shared Operator *(composition As Formula, n%) As Formula
            Dim newFormula$ = $"({composition}){n}"
            Dim newComposition = composition _
                .CountsByElement _
                .ToDictionary(Function(e) e.Key,
                              Function(e)
                                  Return e.Value * n
                              End Function)

            Return New Formula(newComposition, newFormula)
        End Operator

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

        Public Shared Operator -(f As Formula, mass As Double) As Double
            Return f.ExactMass - mass
        End Operator

        Public Shared Operator -(mass As Double, f As Formula) As Double
            Return mass - f.ExactMass
        End Operator

        Public Shared Operator +(mass As Double, f As Formula) As Double
            Return mass + f.ExactMass
        End Operator

        Public Shared Operator +(f As Formula, mass As Double) As Double
            Return f.ExactMass + mass
        End Operator

        Public Shared Operator /(f As Formula, n As Integer) As Formula
            Dim newComposition = f _
                .CountsByElement _
                .ToDictionary(Function(e) e.Key,
                              Function(e)
                                  Return CInt(e.Value / n)
                              End Function)

            Return New Formula(newComposition)
        End Operator
    End Class
End Namespace
