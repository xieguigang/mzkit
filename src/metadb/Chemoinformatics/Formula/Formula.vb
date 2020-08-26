#Region "Microsoft.VisualBasic::6be8870bce9875cff559529d405a7f06, src\metadb\Chemoinformatics\Formula\FormulaComposition.vb"

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

    '     Class FormulaComposition
    ' 
    '         Properties: CountsByElement, EmpiricalFormula
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    '         Operators: *
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Formula

    Public Class Formula

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
    End Class

    Public Class FormulaComposition : Inherits Formula

        Public Property charge As Double
        Public Property ppm As Double
        Public Property exact_mass As Double

        Sub New(counts As IDictionary(Of String, Integer), Optional formula$ = Nothing)
            Call MyBase.New(counts, formula)
        End Sub

        Public Function AppendElement(element As String, count As Integer) As FormulaComposition
            Dim copy As FormulaComposition = GetCopy()

            If copy.CountsByElement.ContainsKey(element) Then
                copy.CountsByElement(element) += count
            Else
                copy.CountsByElement(element) = count
            End If

            copy.exact_mass = copy.exact_mass + Formula.Elements(element).isotopic * count
            copy.charge = copy.charge + Formula.Elements(element).charge * count
            copy.m_formula = Formula.BuildFormula(copy.CountsByElement)

            Return copy
        End Function

        Friend Function GetCopy() As FormulaComposition
            Return New FormulaComposition(CountsByElement, EmpiricalFormula) With {
                .exact_mass = exact_mass,
                .charge = charge,
                .ppm = ppm
            }
        End Function
    End Class
End Namespace
