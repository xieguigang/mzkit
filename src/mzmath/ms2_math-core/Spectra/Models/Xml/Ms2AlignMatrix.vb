#Region "Microsoft.VisualBasic::5d2c50178765c4ead3597fce1b5cca4b, src\mzmath\ms2_math-core\Spectra\Models\Xml\Ms2AlignMatrix.vb"

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

    '     Class Ms2AlignMatrix
    ' 
    '         Properties: SSM
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetQueryMatrix, GetReferenceMatrix
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.Scripting
Imports MathCore = Microsoft.VisualBasic.Math

Namespace Spectra.Xml

    ''' <summary>
    ''' read biodeep spectra alignment output matrix
    ''' </summary>
    Public Class Ms2AlignMatrix : Inherits IVector(Of SSM2MatrixFragment)

        ''' <summary>
        ''' 计算两个色谱矩阵之间的余弦相似度
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SSM As Double
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                With Me
                    Return MathCore.SSM(!query, !ref)
                End With
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(source As IEnumerable(Of SSM2MatrixFragment))
            Call MyBase.New(source)
        End Sub

        Public Function GetQueryMatrix() As LibraryMatrix
            Return New LibraryMatrix With {
                .name = "query",
                .ms2 = Me _
                    .Where(Function(a) a.query > 0) _
                    .Select(Function(a)
                                Return New ms2 With {
                                    .mz = a.mz,
                                    .intensity = a.query
                                }
                            End Function) _
                    .ToArray
            }
        End Function

        Public Function GetReferenceMatrix() As LibraryMatrix
            Return New LibraryMatrix With {
                .name = "subject",
                .ms2 = Me _
                    .Where(Function(a) a.ref > 0) _
                    .Select(Function(a)
                                Return New ms2 With {
                                    .mz = a.mz,
                                    .intensity = a.ref
                                }
                            End Function) _
                    .ToArray
            }
        End Function
    End Class
End Namespace
