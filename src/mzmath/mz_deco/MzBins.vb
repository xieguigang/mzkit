Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Distributions.BinBox
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.Math.SignalProcessing.PeakFinding

''' <summary>
''' algorithm for find the real m/z
''' </summary>
Public Module MzBins

    <Extension>
    Public Function GetScatter(mz As IEnumerable(Of Double), Optional mzdiff As Double = 0.001) As (x As Double(), y As Double())
        Dim mzpool As Double() = mz.SafeQuery _
            .OrderBy(Function(mzi) mzi) _
            .ToArray

        If mzpool.IsNullOrEmpty Then
            Return Nothing
        End If

        Dim hist As DataBinBox(Of Double)() = CutBins _
            .FixedWidthBins(mzpool, width:=mzdiff, Function(xi) xi, mzpool.First, mzpool.Last) _
            .ToArray
        Dim x As Double() = hist.Select(Function(bi) bi.Boundary.Min).ToArray
        Dim y As Double() = hist.Select(Function(bi) CDbl(bi.Count)).ToArray

        Return (x, y)
    End Function

    Public Iterator Function GetMzBins(mz As Double(), hist As Double(),
                                       Optional angle As Double = 3,
                                       Optional baseline As Double = 0.65) As IEnumerable(Of MassWindow)

        Dim scatter As New GeneralSignal(mz, hist)
        Dim peaks As SignalPeak() = New ElevationAlgorithm(angle, baseline) _
            .FindAllSignalPeaks(scatter) _
            .ToArray

        For Each peak As SignalPeak In peaks
            Yield New MassWindow With {
                .mass = peak.rt,
                .mzmin = peak.rtmin,
                .mzmax = peak.rtmax
            }
        Next
    End Function

    <Extension>
    Public Function GetMzBins(mz As IEnumerable(Of Double), Optional mzdiff As Double = 0.001) As IEnumerable(Of MassWindow)
        With mz.GetScatter(mzdiff)
            If .x.IsNullOrEmpty Then
                Return {}
            Else
                Return GetMzBins(.x, .y)
            End If
        End With
    End Function

End Module
