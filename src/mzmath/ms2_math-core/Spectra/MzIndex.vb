#Region "Microsoft.VisualBasic::70cb2f194275ed37cd960bf073f0082d, mzmath\ms2_math-core\Spectra\MzIndex.vb"

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

    '   Total Lines: 59
    '    Code Lines: 27 (45.76%)
    ' Comment Lines: 20 (33.90%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 12 (20.34%)
    '     File Size: 1.71 KB


    '     Class MzIndex
    ' 
    '         Properties: index, mz
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString, Tuple
    '         Operators: *
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Spectra

    ''' <summary>
    ''' represents the ion m/z as a index
    ''' </summary>
    ''' <remarks>
    ''' a tuple value of ion m/z feature couple with its offset index
    ''' </remarks>
    Public Class MzIndex

        Public Property mz As Double

        ''' <summary>
        ''' the index value
        ''' </summary>
        ''' <returns></returns>
        Public Property index As Integer

        Sub New()
        End Sub

        Sub New(mz As Double, Optional index As Integer = 0)
            Me.mz = mz
            Me.index = index
        End Sub

        ''' <summary>
        ''' get the fallback tuple data
        ''' </summary>
        ''' <returns></returns>
        Public Function Tuple() As (mz As Double, Integer)
            Return (mz, index)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"[{index}] {mz.ToString}"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Widening Operator CType(mzVal As (mz As Double, index As Integer)) As MzIndex
            Return New MzIndex(mzVal.mz, mzVal.index)
        End Operator

        ''' <summary>
        ''' calculate the binary data file offset
        ''' </summary>
        ''' <param name="sizeof"></param>
        ''' <param name="index"></param>
        ''' <returns></returns>
        Public Shared Operator *(sizeof As Integer, index As MzIndex) As Integer
            Return sizeof * index.index
        End Operator

    End Class

End Namespace
