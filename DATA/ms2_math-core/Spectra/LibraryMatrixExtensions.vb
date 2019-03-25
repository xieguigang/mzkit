#Region "Microsoft.VisualBasic::7973fddcf2d53f27e7bdb3887271ed0f, ms2_math-core\Spectra\LibraryMatrixExtensions.vb"

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

    '     Module LibraryMatrixExtensions
    ' 
    '         Function: AsMatrix, Max, Shrink
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.MassSpectrum.Math.Ms1

Namespace Spectra

    ''' <summary>
    ''' Library matrix math
    ''' </summary>
    Public Module LibraryMatrixExtensions

        ''' <summary>
        ''' MAX(<see cref="ms2.quantity"/>)
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Max(matrix As LibraryMatrix) As Double
            Return matrix.ms2.Max(Function(r) r.quantity)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function AsMatrix(lib_ms2 As IEnumerable(Of Library)) As LibraryMatrix
            Return lib_ms2 _
                .Select(Function(l)
                            Return New ms2 With {
                                .mz = l.ProductMz,
                                .quantity = l.LibraryIntensity,
                                .intensity = l.LibraryIntensity
                            }
                        End Function) _
                .ToArray
        End Function

        ''' <summary>
        ''' 将符合误差范围的二级碎片合并在一起
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Shrink(matrix As LibraryMatrix, tolerance As Tolerance) As LibraryMatrix
            Return matrix _
                .GroupBy(Function(ms2) ms2.mz, AddressOf tolerance.Assert) _
                .Select(Function(g)
                            ' 合并在一起的二级碎片的相应强度取最高的为结果
                            Return g(Which.Max(g.Select(Function(m) m.intensity)))
                        End Function) _
                .ToArray
        End Function
    End Module
End Namespace
