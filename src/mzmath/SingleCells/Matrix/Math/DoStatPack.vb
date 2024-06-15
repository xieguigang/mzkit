Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile

Namespace MatrixMath

    Module DoStatPack

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="allIons">a tuple of the cell id and ion data</param>
        ''' <param name="da"></param>
        ''' <param name="parallel"></param>
        ''' <returns></returns>
        Private Iterator Function DoStatInternal(allIons As IEnumerable(Of (scan_id As String, ms1 As ms2)), da As Double, parallel As Boolean) As IEnumerable(Of SingleCellIonStat)
            Dim ions = allIons _
                .GroupBy(Function(d) d.ms1.mz, Tolerance.DeltaMass(da)) _
                .ToArray

            If parallel Then
                For Each stat In ions _
                    .AsParallel _
                    .Select(Function(ion)
                                Return DoStatSingleIon(Val(ion.name), ion.value)
                            End Function)

                    Yield stat
                Next
            Else
                For Each ion As NamedCollection(Of (cell_id As String, ms As ms2)) In ions
                    Yield DoStatSingleIon(Val(ion.name), ion.value)
                Next
            End If
        End Function

        Private Function DoStatSingleIon(mz_val As Double, ion As (scan_id As String, intensity As Double)()) As SingleCellIonStat
            Dim baseCell = ion.OrderByDescending(Function(i) i.intensity).First
            Dim intensity As Double() = ion _
                .Select(Function(i) i.intensity) _
                .ToArray
            Dim cells = ion.Select(Function(c) c.scan_id).Distinct.Count
            Dim Q As DataQuartile = intensity.Quartile

            Return New SingleCellIonStat With {
                .mz = mz_val,
                .cells = cells,
                .maxIntensity = baseCell.intensity,
                .baseCell = baseCell.scan_id,
                .Q1Intensity = Q.Q1,
                .Q2Intensity = Q.Q2,
                .Q3Intensity = Q.Q3,
                .RSD = fillVector(intensity).RSD * 100
            }
        End Function

        Private Function DoStatSingleIon(mz_val As Double, ion As (scan_id As String, ms1 As ms2)()) As SingleCellIonStat
            Dim baseCell = ion.OrderByDescending(Function(i) i.ms1.intensity).First
            Dim intensity As Double() = ion _
                .Select(Function(i) i.ms1.intensity) _
                .ToArray
            Dim cells = ion.Select(Function(c) c.scan_id).Distinct.Count
            Dim Q As DataQuartile = intensity.Quartile

            Return New SingleCellIonStat With {
                .mz = mz_val,
                .cells = cells,
                .maxIntensity = baseCell.ms1.intensity,
                .baseCell = baseCell.scan_id,
                .Q1Intensity = Q.Q1,
                .Q2Intensity = Q.Q2,
                .Q3Intensity = Q.Q3,
                .RSD = fillVector(intensity).RSD * 100
            }
        End Function

        Public Function fillVector(v As Double()) As Vector
            If v.Any(Function(vi) vi > 0) Then
                Dim fill As Vector = {v.Where(Function(i) i > 0).Min}
                Dim peakfill As Vector = v.AsVector

                peakfill(peakfill <= 0) = fill

                Return peakfill
            Else
                ' all is zero!
                Return v
            End If
        End Function
    End Module
End Namespace