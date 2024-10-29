#Region "Microsoft.VisualBasic::f7f9f2a0e8688c199bde27d44580676e, mzmath\SingleCells\Matrix\Math\FeatureTrIQClips.vb"

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

    '   Total Lines: 63
    '    Code Lines: 54 (85.71%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (14.29%)
    '     File Size: 2.38 KB


    '     Class FeatureTrIQClips
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetClipMatrix
    ' 
    '         Sub: Solve
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Parallel

Namespace MatrixMath

    Public Class FeatureTrIQClips : Inherits VectorTask

        ReadOnly raw As MzMatrix
        ReadOnly features As Double()()
        ReadOnly q As Double = 0.6
        ReadOnly levels As Integer = 100

        Public Sub New(m As MzMatrix, Optional q As Double = 0.6, Optional levels As Integer = 100)
            MyBase.New(m.featureSize)

            Me.raw = m
            Me.features = Allocate(Of Double())(all:=True)
            Me.q = q
            Me.levels = levels
        End Sub

        Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)
            For i As Integer = start To ends
                Dim offset As Integer = i
                Dim v As Double() = raw.matrix _
                    .Select(Function(x) x(offset)) _
                    .ToArray
                Dim cutoff As Double = TrIQ.FindThreshold(v, q, levels)

                features(offset) = v.ClipUpper(cutoff).ToArray
            Next
        End Sub

        Public Function GetClipMatrix() As MzMatrix
            Return New MzMatrix With {
                .matrixType = raw.matrixType,
                .mz = raw.mz,
                .mzmax = raw.mzmax,
                .mzmin = raw.mzmin,
                .tolerance = raw.tolerance,
                .matrix = raw.matrix _
                    .Select(Function(a, i)
                                Dim offset As Integer = i
                                Dim vi As Double() = features _
                                    .Select(Function(mzi) mzi(offset)) _
                                    .ToArray
                                Dim pi As New PixelData() With {
                                    .label = a.label,
                                    .X = a.X,
                                    .Y = a.Y,
                                    .Z = a.Z,
                                    .intensity = vi
                                }

                                Return pi
                            End Function) _
                    .ToArray
            }
        End Function
    End Class
End Namespace
