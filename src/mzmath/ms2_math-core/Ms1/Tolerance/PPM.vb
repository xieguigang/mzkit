#Region "Microsoft.VisualBasic::3c074ec69b1b3ffef8c8168ec7ec0343, ms2_math-core\Ms1\Tolerance\PPM.vb"

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

    '     Class PPMmethod
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: AsScore, Equals, MassError, MassErrorDescription, PPM
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports stdNum = System.Math

Namespace Ms1

    ''' <summary>
    ''' PPM tolerance calculator
    ''' </summary>
    Public Class PPMmethod : Inherits Tolerance

        Sub New()
        End Sub

        Sub New(ppm#)
            DeltaTolerance = ppm
        End Sub

        ''' <summary>
        ''' 分子量差值
        ''' </summary>
        ''' <param name="measured#"></param>
        ''' <param name="actualValue#"></param>
        ''' <returns></returns>
        Public Overloads Shared Function PPM(measured#, actualValue#) As Double
            ' （测量值-实际分子量）/ 实际分子量
            ' |(实验数据 - 数据库结果)| / 实验数据 * 1000000
            Dim ppmd# = (stdNum.Abs(measured - actualValue) / actualValue) * 1000000

            If ppmd < 0 Then
                ' 计算溢出了
                Return 10000000000
            End If

            Return ppmd
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function Equals(mz1 As Double, mz2 As Double) As Boolean
            Return ppm(mz1, mz2) <= DeltaTolerance
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function AsScore(mz1 As Double, mz2 As Double) As Double
            Return 1 - (ppm(mz1, mz2) / DeltaTolerance)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function MassError(mz1 As Double, mz2 As Double) As Double
            Return ppm(mz1, mz2)
        End Function

        Public Overrides Function ToString() As String
            Return $"ppm(mz1, mz2) <= {DeltaTolerance}"
        End Function

        Public Overrides Function MassErrorDescription(mz1 As Double, mz2 As Double) As String
            Return $"{MassError(mz1, mz2)} ppm"
        End Function
    End Class
End Namespace
