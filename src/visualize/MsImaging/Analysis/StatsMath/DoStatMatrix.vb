Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel

Namespace StatsMath

    Module DoStatMatrix

        Public Function DoStat(rawdata As MzMatrix, Optional nsize As Integer = 5, Optional parallel As Boolean = True) As IEnumerable(Of IonStat)
            Dim ions As NamedCollection(Of PixelData)() = rawdata.mz _
                .AsParallel _
                .Select(Function(mzi, i)
                            Dim pixels As PixelData() = rawdata.matrix _
                                .Where(Function(s) s.intensity(i) > 0) _
                                .Select(Function(s) New PixelData(s.X, s.Y, s.intensity(i))) _
                                .ToArray

                            Return New NamedCollection(Of PixelData)(mzi.ToString, pixels)
                        End Function) _
                .ToArray
            Dim par As New IonFeatureTask(ions, nsize)

            If parallel Then
                Call par.Run()
            Else
                Call par.Solve()
            End If

            Return par.result
        End Function

        Private Class IonFeatureTask : Inherits VectorTask

            ReadOnly matrix As MzMatrix
            ReadOnly grid_size As Integer

            Friend ReadOnly rsd As Double()
            Friend ReadOnly sparsity As Double()
            Friend ReadOnly entropy As Double()

            Public Sub New(m As MzMatrix, grid_size As Integer)
                MyBase.New(m.featureSize)

                Me.matrix = m
                Me.grid_size = grid_size
            End Sub

            Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)

            End Sub
        End Class
    End Module
End Namespace