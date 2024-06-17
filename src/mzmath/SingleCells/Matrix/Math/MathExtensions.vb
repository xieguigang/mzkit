#Region "Microsoft.VisualBasic::c5965ebb72d92a9479f818bfe0038ac3, mzmath\SingleCells\Matrix\Math\MathExtensions.vb"

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

    '   Total Lines: 91
    '    Code Lines: 67 (73.63%)
    ' Comment Lines: 5 (5.49%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 19 (20.88%)
    '     File Size: 3.31 KB


    '     Module MathExtensions
    ' 
    '         Function: Entropy, RSD, Sparsity, TotalPeakSumNormalization
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Information
Imports Microsoft.VisualBasic.Math.SIMD

Namespace MatrixMath

    Public Module MathExtensions

        ''' <summary>
        ''' normalize of each spot/cell by total peak sum.
        ''' </summary>
        ''' <param name="m"></param>
        ''' <returns></returns>
        <Extension>
        Public Function TotalPeakSumNormalization(m As MzMatrix, Optional scale As Double = 1000000.0) As MzMatrix
            Dim norm As New List(Of PixelData)

            For Each spot As PixelData In m.matrix
                Dim sum_val As Double = Aggregate into As Double In spot.intensity Into Sum(into)
                Dim norm_vec As Double() = Multiply.f64_scalar_op_multiply_f64(scale, Divide.f64_op_divide_f64_scalar(spot.intensity, sum_val))
                Dim norm_spot As New PixelData With {
                    .intensity = norm_vec,
                    .label = spot.label,
                    .X = spot.X,
                    .Y = spot.Y,
                    .Z = spot.Z
                }

                Call norm.Add(norm_spot)
            Next

            Return New MzMatrix With {
                .matrix = norm.ToArray,
                .matrixType = m.matrixType,
                .mz = m.mz,
                .mzmax = m.mzmax,
                .mzmin = m.mzmin,
                .tolerance = m.tolerance
            }
        End Function

        <Extension>
        Public Function RSD(m As MzMatrix) As Double()
            Dim rsd_vec As Double() = New Double(m.featureSize - 1) {}

            For i As Integer = 0 To rsd_vec.Length - 1
                Dim offset As Integer = i
                Dim col As Double() = (From cell As PixelData In m.matrix Select cell(offset)).ToArray

                rsd_vec(i) = col.RSD
            Next

            Return rsd_vec
        End Function

        <Extension>
        Public Function Entropy(m As MzMatrix) As Double()
            Dim ent_vec As Double() = New Double(m.featureSize - 1) {}

            For i As Integer = 0 To ent_vec.Length - 1
                Dim offset As Integer = i
                Dim col As Double() = (From cell As PixelData In m.matrix Select cell(offset)).ToArray
                Dim sum_val As Double = col.Sum
                Dim norm As Double() = Divide.f64_op_divide_f64_scalar(col, sum_val)
                Dim ent As Double = norm.ShannonEntropy

                ent_vec(offset) = ent
            Next

            Return ent_vec
        End Function

        <Extension>
        Public Function Sparsity(m As MzMatrix) As Double()
            Dim sparsity_vec As Double() = New Double(m.featureSize - 1) {}

            For i As Integer = 0 To sparsity_vec.Length - 1
                Dim offset As Integer = i
                Dim data_n As Integer = (From cell As PixelData In m.matrix Where cell(offset) > 0).Count
                Dim spar As Double = 1 - data_n / m.matrix.Length

                sparsity_vec(i) = spar
            Next

            Return sparsity_vec
        End Function

    End Module
End Namespace
