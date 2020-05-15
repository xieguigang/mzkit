#Region "Microsoft.VisualBasic::363a80ff6889b4f85ff59daee0359205, src\mzmath\MwtWinDll\MwtWinDll\Formula\FormulaComposition.vb"

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
    '         Function: GetEmpiricalFormula, ToString
    '         Operators: *
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace FormulaFinder

    Public Class FormulaComposition

        Public ReadOnly Property CountsByElement As Dictionary(Of String, Integer)
        Public ReadOnly Property EmpiricalFormula As String

        Default Public ReadOnly Property GetAtomCount(atom As String) As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return CountsByElement.TryGetValue(atom)
            End Get
        End Property

        Sub New(counts As IDictionary(Of String, Integer), Optional formula$ = Nothing)
            CountsByElement = New Dictionary(Of String, Integer)(counts)

            If formula.StringEmpty Then
                EmpiricalFormula = GetEmpiricalFormula(composition:=CountsByElement)
            Else
                EmpiricalFormula = formula
            End If
        End Sub

        Public Shared Function GetEmpiricalFormula(composition As IDictionary(Of String, Integer)) As String
            Return composition _
                .Select(Function(e) If(e.Value = 1, e.Key, e.Key & e.Value)) _
                .JoinBy("")
        End Function

        Public Overrides Function ToString() As String
            Return EmpiricalFormula
        End Function

        Public Shared Operator *(composition As FormulaComposition, n%) As FormulaComposition
            Dim newFormula$ = $"({composition}){n}"
            Dim newComposition = composition _
                .CountsByElement _
                .ToDictionary(Function(e) e.Key,
                              Function(e)
                                  Return e.Value * n
                              End Function)

            Return New FormulaComposition(newComposition, newFormula)
        End Operator
    End Class
End Namespace
