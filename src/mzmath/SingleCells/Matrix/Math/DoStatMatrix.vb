Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Information
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Math.SIMD
Imports Microsoft.VisualBasic.Parallel

Namespace MatrixMath

    Friend Class DoStatMatrix : Inherits VectorTask

        ReadOnly matrix As PixelData()
        ReadOnly feature_size As Integer
        ReadOnly total_cells As Integer

        ReadOnly rsd As Double()
        ReadOnly entropy As Double()
        ReadOnly sparsity As Double()
        ReadOnly cells As Integer()
        ReadOnly max_into As Double()
        ReadOnly base_cell As String()
        ReadOnly q1, q2, q3 As Double()

        Sub New(m As MzMatrix)
            Call MyBase.New(m.featureSize)

            matrix = m.matrix
            feature_size = m.featureSize
            total_cells = matrix.Length


        End Sub

        Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)
            For i As Integer = start To ends
                Dim offset As Integer = i
                Dim intensity_vec As Double() = (From cell As PixelData In matrix Select cell(offset)).ToArray
                Dim max_i As Integer = which.Max(intensity_vec)
                Dim counts As Integer = Aggregate xi As Double In intensity_vec Where xi > 0 Into Count
                Dim quartile As DataQuartile = intensity_vec.Quartile

                base_cell(offset) = matrix(max_i).label
                cells(offset) = counts
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
                    .RSD = task.rsd(i),
                    .sparsity = task.sparsity(i)
                }
            Next
        End Function
    End Class
End Namespace