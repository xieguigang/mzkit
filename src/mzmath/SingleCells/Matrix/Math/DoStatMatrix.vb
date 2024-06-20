#Region "Microsoft.VisualBasic::e3185c62bb07b93fd6892c07ba158fa0, mzmath\SingleCells\Matrix\Math\DoStatMatrix.vb"

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

'   Total Lines: 114
'    Code Lines: 87 (76.32%)
' Comment Lines: 13 (11.40%)
'    - Xml Docs: 92.31%
' 
'   Blank Lines: 14 (12.28%)
'     File Size: 4.68 KB


'     Class DoStatMatrix
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: DoIonStats
' 
'         Sub: Solve
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Information
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Math.SIMD
Imports Microsoft.VisualBasic.Parallel

Namespace MatrixMath

    ''' <summary>
    ''' analysis the ion features inside a single cells matrix
    ''' </summary>
    Public Class DoStatMatrix : Inherits VectorTask

        Protected ReadOnly matrix As PixelData()
        Protected ReadOnly feature_size As Integer
        Protected ReadOnly total_cells As Integer

        Protected ReadOnly rsd As Double()
        Protected ReadOnly entropy As Double()
        Protected ReadOnly sparsity As Double()
        Protected ReadOnly cells As Integer()
        Protected ReadOnly max_into As Double()
        Protected ReadOnly base_cell As String()
        Protected ReadOnly q1, q2, q3 As Double()
        ''' <summary>
        ''' row index of the spot which its intensity value is max of current ion feature
        ''' </summary>
        Protected ReadOnly max_spot As Integer()

        Sub New(m As MzMatrix)
            Call MyBase.New(m.featureSize)

            matrix = m.matrix
            feature_size = m.featureSize
            total_cells = matrix.Length

            rsd = Allocate(Of Double)(all:=True)
            entropy = Allocate(Of Double)(all:=True)
            sparsity = Allocate(Of Double)(all:=True)
            cells = Allocate(Of Integer)(all:=True)
            max_into = Allocate(Of Double)(all:=True)
            base_cell = Allocate(Of String)(all:=True)
            q1 = Allocate(Of Double)(all:=True)
            q2 = Allocate(Of Double)(all:=True)
            q3 = Allocate(Of Double)(all:=True)
            max_spot = Allocate(Of Integer)(all:=True)
        End Sub

        Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)
            For i As Integer = start To ends
                Dim offset As Integer = i
                Dim intensity_vec As Double() = (From cell As PixelData In matrix Select cell(offset)).ToArray
                Dim max_i As Integer = which.Max(intensity_vec)
                Dim counts As Integer = Aggregate xi As Double
                                        In intensity_vec
                                        Where xi > 0
                                        Into Count
                Dim quartile As DataQuartile = intensity_vec.Quartile

                base_cell(offset) = matrix(max_i).label
                cells(offset) = counts
                max_spot(offset) = max_i
                max_into(offset) = intensity_vec(max_i)
                q1(offset) = quartile.Q1
                q2(offset) = quartile.Q2
                q3(offset) = quartile.Q3

                intensity_vec = Divide.f64_op_divide_f64_scalar(intensity_vec, intensity_vec.Sum)

                rsd(offset) = intensity_vec.RSD
                entropy(offset) = intensity_vec.ShannonEntropy
                sparsity(offset) = 1 - counts / total_cells
            Next
        End Sub

        ''' <summary>
        ''' do single cell ion feature statistics analysis
        ''' </summary>
        ''' <param name="mat"></param>
        ''' <param name="parallel"></param>
        ''' <returns></returns>
        ''' 
        Public Shared Iterator Function DoIonStats(mat As MzMatrix, Optional parallel As Boolean = True) As IEnumerable(Of SingleCellIonStat)
            Dim task As New DoStatMatrix(mat)

            If parallel Then
                Call task.Run()
            Else
                Call task.Solve()
            End If

            For i As Integer = 0 To mat.featureSize - 1
                Yield New SingleCellIonStat With {
                    .baseCell = task.base_cell(i),
                    .cells = task.cells(i),
                    .entropy = task.entropy(i),
                    .maxIntensity = task.max_into(i),
                    .mz = mat.mz(i),
                    .mzmax = mat.mzmax(i),
                    .mzmin = mat.mzmin(i),
                    .mz_error = MassWindow.ToString(.mzmax, .mzmin),
                    .Q1Intensity = task.q1(i),
                    .Q2Intensity = task.q2(i),
                    .Q3Intensity = task.q3(i),
                    .rsd = task.rsd(i),
                    .sparsity = task.sparsity(i)
                }
            Next
        End Function
    End Class
End Namespace
