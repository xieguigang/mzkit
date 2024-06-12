#Region "Microsoft.VisualBasic::51ddb66d0e0a6bfb7a2d853a282ceffa, mzmath\SingleCells\SingleCellIonStat.vb"

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

    '   Total Lines: 109
    '    Code Lines: 74 (67.89%)
    ' Comment Lines: 24 (22.02%)
    '    - Xml Docs: 95.83%
    ' 
    '   Blank Lines: 11 (10.09%)
    '     File Size: 4.09 KB


    ' Class SingleCellIonStat
    ' 
    '     Properties: baseCell, cells, maxIntensity, mz, Q1Intensity
    '                 Q2Intensity, Q3Intensity, RSD
    ' 
    '     Function: DoIonStats, DoStatInternal, DoStatSingleIon, fillVector
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile

''' <summary>
''' A ion stat information from a single cell data
''' </summary>
Public Class SingleCellIonStat

    ''' <summary>
    ''' the ion feature m/z value
    ''' </summary>
    ''' <returns></returns>
    Public Property mz As Double
    ''' <summary>
    ''' the cell numbers that contains this ion feature.
    ''' </summary>
    ''' <returns></returns>
    Public Property cells As Integer
    ''' <summary>
    ''' the max intensity value of current ion feature.
    ''' </summary>
    ''' <returns></returns>
    Public Property maxIntensity As Double
    ''' <summary>
    ''' the cell label id which has the max intensity of current ion feature.
    ''' </summary>
    ''' <returns></returns>
    Public Property baseCell As String
    Public Property Q1Intensity As Double
    Public Property Q2Intensity As Double
    Public Property Q3Intensity As Double
    ''' <summary>
    ''' RSD value of the intensity value of current ion feature across multiple cells
    ''' </summary>
    ''' <returns></returns>
    Public Property RSD As Double

    Public Shared Function DoIonStats(raw As IMZPack, Optional da As Double = 0.01, Optional parallel As Boolean = True) As IEnumerable(Of SingleCellIonStat)
        Return raw.MS _
            .Select(Function(scan)
                        Return scan.GetMs.Select(Function(ms1) (scan.scan_id, ms1))
                    End Function) _
            .IteratesALL _
            .DoCall(Function(allIons)
                        Return DoStatInternal(allIons, da, parallel)
                    End Function)
    End Function

    Public Shared Iterator Function DoIonStats(mat As MzMatrix, Optional parallel As Boolean = True) As IEnumerable(Of SingleCellIonStat)
        If parallel Then
            For Each stat In mat.mz _
                .SeqIterator _
                .AsParallel _
                .Select(Function(i)
                            Dim offset As Integer = i
                            Dim cells = mat.matrix _
                                .Select(Function(cell) (cell.label, cell(i))) _
                                .ToArray

                            Return DoStatSingleIon(i.value, cells)
                        End Function)

                Yield stat
            Next
        Else
            Dim offset As Integer
            Dim cells As (String, Double)()

            For i As Integer = 0 To mat.featureSize - 1
                offset = i
                cells = mat.matrix _
                    .Select(Function(cell) (cell.label, cell(offset))) _
                    .ToArray

                Yield DoStatSingleIon(mat.mz(i), cells)
            Next
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="allIons">a tuple of the cell id and ion data</param>
    ''' <param name="da"></param>
    ''' <param name="parallel"></param>
    ''' <returns></returns>
    Private Shared Iterator Function DoStatInternal(allIons As IEnumerable(Of (scan_id As String, ms1 As ms2)), da As Double, parallel As Boolean) As IEnumerable(Of SingleCellIonStat)
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

    Private Shared Function DoStatSingleIon(mz_val As Double, ion As (scan_id As String, intensity As Double)()) As SingleCellIonStat
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

    Private Shared Function DoStatSingleIon(mz_val As Double, ion As (scan_id As String, ms1 As ms2)()) As SingleCellIonStat
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

    Public Shared Function fillVector(v As Double()) As Vector
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
End Class
