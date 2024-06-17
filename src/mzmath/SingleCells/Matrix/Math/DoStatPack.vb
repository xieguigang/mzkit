#Region "Microsoft.VisualBasic::c5839b5fbde837924c5ff469a1cf5719, mzmath\SingleCells\Matrix\Math\DoStatPack.vb"

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

    '   Total Lines: 95
    '    Code Lines: 75 (78.95%)
    ' Comment Lines: 8 (8.42%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 12 (12.63%)
    '     File Size: 3.80 KB


    '     Module DoStatPack
    ' 
    '         Function: DoStatInternal, (+2 Overloads) DoStatSingleIon, fillVector
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
        Friend Iterator Function DoStatInternal(allIons As IEnumerable(Of (scan_id As String, ms1 As ms2)), da As Double, parallel As Boolean) As IEnumerable(Of SingleCellIonStat)
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
