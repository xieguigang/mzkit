#Region "Microsoft.VisualBasic::2ad151c8f6b71f2e184a20797318dfb6, Chemoinformatics\Formula\Models\FormulaComposition.vb"

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
    '         Properties: charge, HCRatio, ppm
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: AppendElement, GetCopy
    ' 
    ' 
    ' /********************************************************************************/

#End Region


Namespace Formula

    Public Class FormulaComposition : Inherits Formula

        Public Property charge As Double
        Public Property ppm As Double

        ''' <summary>
        ''' Hydrogen/Carbon element ratio
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property HCRatio As Double
            Get
                With CountsByElement
                    If .ContainsKey("C") AndAlso .ContainsKey("H") Then
                        Return !H / !C
                    Else
                        Return -1
                    End If
                End With
            End Get
        End Property

        Sub New(counts As IDictionary(Of String, Integer), Optional formula$ = Nothing)
            Call MyBase.New(counts, formula)
        End Sub

        ''' <summary>
        ''' make a copy and then append a given atom element into formula model
        ''' </summary>
        ''' <param name="element"></param>
        ''' <param name="count"></param>
        ''' <returns></returns>
        Public Function AppendElement(element As String, count As Integer) As FormulaComposition
            Dim copy As FormulaComposition = GetCopy()

            If copy.CountsByElement.ContainsKey(element) Then
                copy.CountsByElement(element) += count
            Else
                copy.CountsByElement(element) = count
            End If

            copy.charge = copy.charge + Formula.Elements(element).charge * count
            copy.m_formula = Formula.BuildFormula(copy.CountsByElement)

            Return copy
        End Function

        Friend Function GetCopy() As FormulaComposition
            Return New FormulaComposition(CountsByElement, EmpiricalFormula) With {
                .charge = charge,
                .ppm = ppm
            }
        End Function
    End Class
End Namespace
