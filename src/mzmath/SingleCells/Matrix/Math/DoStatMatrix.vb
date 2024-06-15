Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.Parallel

Namespace MatrixMath

    Friend Class DoStatMatrix : Inherits VectorTask

        ReadOnly matrix As PixelData()
        ReadOnly feature_size As Integer

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

        End Sub

        Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)

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