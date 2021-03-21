#Region "Microsoft.VisualBasic::6f5983e2981e0470534b39ccb843bf52, src\mzmath\ms2_math-core\Spectra\SpectrumTree\Comparison.vb"

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

    '     Class Comparison
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Compares
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Spectra

    Public Class Comparison

        ReadOnly equalsScore# = 0.85
        ReadOnly gtScore# = 0.6
        ReadOnly alignment As AlignmentProvider

        Const InvalidScoreRange$ = "Scores for x < y should be in range (0, 1] and its value is also less than score for spectra equals!"

        Sub New(align As AlignmentProvider, Optional equalsScore# = 0.85, Optional gtScore# = 0.6)
            Me.gtScore = gtScore
            Me.equalsScore = equalsScore
            Me.alignment = align

            If equalsScore < 0 OrElse equalsScore > 1 Then
                Throw New InvalidConstraintException("Scores for spectra equals is invalid, it should be in range (0, 1].")
            End If
            If gtScore < 0 OrElse gtScore > 1 OrElse gtScore > equalsScore Then
                Throw New InvalidConstraintException(InvalidScoreRange)
            End If
        End Sub

        Public Function Compares(x As PeakMs2, y As PeakMs2) As Integer
            Dim scoreVal As Double = alignment.GetScore(x.mzInto, y.mzInto)

            If scoreVal >= equalsScore Then
                Return 0
            ElseIf scoreVal >= gtScore Then
                Return 1
            Else
                Return -1
            End If
        End Function
    End Class
End Namespace
