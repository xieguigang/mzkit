#Region "Microsoft.VisualBasic::1a334a9b9985ce1fa94ff216310ed705, E:/mzkit/src/metadb/Chemoinformatics//Formula/Models/FormulaComposition.vb"

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

    '   Total Lines: 78
    '    Code Lines: 47
    ' Comment Lines: 21
    '   Blank Lines: 10
    '     File Size: 2.75 KB


    '     Class FormulaComposition
    ' 
    '         Properties: charge, HCRatio, massdiff, ppm
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: AppendElement, GetCopy, Ratio
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Formula

    ''' <summary>
    ''' the formula search result
    ''' </summary>
    Public Class FormulaComposition : Inherits Formula

        ''' <summary>
        ''' the charge value of current formula
        ''' </summary>
        ''' <returns></returns>
        Public Property charge As Double
        ''' <summary>
        ''' the ppm error between the theriocal mass value and the input mass value
        ''' </summary>
        ''' <returns></returns>
        Public Property ppm As Double
        Public Property massdiff As Double

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

        Public Function Ratio(element1 As String, element2 As String) As Double
            If Not CountsByElement.ContainsKey(element1) Then
                Return 0
            ElseIf Not CountsByElement.ContainsKey(element2) Then
                Return -1
            Else
                Return CountsByElement(element1) / CountsByElement(element2)
            End If
        End Function

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

            copy.charge = copy.charge + Formula.AllAtomElements(element).charge * count
            copy.m_formula = Canonical.BuildCanonicalFormula(copy.CountsByElement)

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
