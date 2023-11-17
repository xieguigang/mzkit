#Region "Microsoft.VisualBasic::1e6fa5f7f3cf92ba5639293809865672, mzkit\src\mzmath\SingleCells\SingleCellIonStat.vb"

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

'   Total Lines: 86
'    Code Lines: 74
' Comment Lines: 1
'   Blank Lines: 11
'     File Size: 3.25 KB


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
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile

Public Class SingleCellIonStat

    Public Property mz As Double
    Public Property cells As Integer
    Public Property maxIntensity As Double
    Public Property baseCell As String
    Public Property Q1Intensity As Double
    Public Property Q2Intensity As Double
    Public Property Q3Intensity As Double
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

    Private Shared Iterator Function DoStatInternal(allIons As IEnumerable(Of (scan_id As String, ms1 As ms2)), da As Double, parallel As Boolean) As IEnumerable(Of SingleCellIonStat)
        Dim ions = allIons _
            .GroupBy(Function(d) d.ms1.mz, Tolerance.DeltaMass(da)) _
            .ToArray

        If parallel Then
            For Each stat In ions _
                .AsParallel _
                .Select(Function(ion)
                            Return DoStatSingleIon(ion)
                        End Function)

                Yield stat
            Next
        Else
            For Each ion As NamedCollection(Of (cell_id As String, ms As ms2)) In ions
                Yield DoStatSingleIon(ion)
            Next
        End If
    End Function

    Private Shared Function DoStatSingleIon(ion As NamedCollection(Of (scan_id As String, ms1 As ms2))) As SingleCellIonStat
        Dim baseCell = ion.OrderByDescending(Function(i) i.ms1.intensity).First
        Dim intensity As Double() = ion _
            .Select(Function(i) i.ms1.intensity) _
            .ToArray
        Dim cells = ion.Select(Function(c) c.scan_id).Distinct.Count
        Dim Q As DataQuartile = intensity.Quartile

        Return New SingleCellIonStat With {
            .mz = Val(ion.name),
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
