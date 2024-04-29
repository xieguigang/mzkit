#Region "Microsoft.VisualBasic::9a6738d5f226c36149f68de1af0fbbe8, E:/mzkit/src/mzmath/mz_deco//MzBins.vb"

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

    '   Total Lines: 63
    '    Code Lines: 50
    ' Comment Lines: 3
    '   Blank Lines: 10
    '     File Size: 2.33 KB


    ' Module MzBins
    ' 
    '     Function: (+2 Overloads) GetMzBins, GetScatter
    ' 
    ' /********************************************************************************/

#End Region

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
            .OrderByDescending(Function(bin) bin.signalMax) _
            .ToArray

        For Each peak As SignalPeak In peaks
            Yield New MassWindow With {
                .mass = peak.rt,
                .mzmin = peak.rtmin,
                .mzmax = peak.rtmax,
                .annotation = (peak.integration).ToString("F2") & "%"
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
