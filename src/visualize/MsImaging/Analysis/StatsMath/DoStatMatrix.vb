Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

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
    End Module
End Namespace