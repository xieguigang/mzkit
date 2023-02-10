#Region "Microsoft.VisualBasic::2b9a0698c090a67096e93bdc837de426, mzkit\src\mzmath\ms2_math-core\Ms1\PrecursorType\PrecursorAdductsAssignRuler.vb"

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

    '   Total Lines: 48
    '    Code Lines: 30
    ' Comment Lines: 11
    '   Blank Lines: 7
    '     File Size: 1.52 KB


    '     Class PrecursorAdductsAssignRuler
    ' 
    '         Function: IonNegativeTypes, IonPositiveTypes, PossibleTypes
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Ms1.PrecursorType

    Public Class PrecursorAdductsAssignRuler

        ''' <summary>
        ''' 制定一些加和物离子规则，例如：
        ''' 
        ''' 化学式中含有活泼离子，例如
        ''' 
        ''' 1. Na+，负离子下很可能为[M-Na]-
        ''' 2. Cl-, 正离子下很可能为[M-Cl]+
        ''' </summary>
        ''' <param name="formula"></param>
        ''' <param name="ionMode"></param>
        ''' <returns></returns>
        Public Function PossibleTypes(formula As String, ionMode As Integer) As String
            If formula.StringEmpty Then
                Return Nothing
            End If

            If ionMode = 1 Then
                Return IonPositiveTypes(formula)
            Else
                Return IonNegativeTypes(formula)
            End If
        End Function

        Private Function IonNegativeTypes(formula As String) As String
            For Each metal As String In {"Na", "Li", "H"}
                If formula.Contains(metal) Then
                    Return $"[M-{metal}]-"
                End If
            Next

            Return Nothing
        End Function

        Private Function IonPositiveTypes(formula As String) As String
            For Each ion As String In {"Cl"}
                If formula.Contains(ion) Then
                    Return $"[M-{ion}]+"
                End If
            Next

            Return Nothing
        End Function
    End Class
End Namespace
