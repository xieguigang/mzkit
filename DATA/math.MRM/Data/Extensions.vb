#Region "Microsoft.VisualBasic::dc64209d7bb31a161781269ed850b55d, math.MRM\Data\Extensions.vb"

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

    '     Module Extensions
    ' 
    '         Function: PopulatePeaks
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Scripting
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzML
Imports SMRUCC.MassSpectrum.Math.Chromatogram

Namespace Dumping

    Public Module Extensions

        <Extension>
        Public Function PopulatePeaks(ionPairs As IonPair(), raw$, Optional baselineQuantile# = 0.65) As (ion As IonPair, peak As MRMPeak)()
            Dim ionData = LoadChromatogramList(path:=raw) _
                .MRMSelector(ionPairs) _
                .Where(Function(ion) Not ion.chromatogram Is Nothing) _
                .Select(Function(ion)
                            Dim vector As IVector(Of ChromatogramTick) = ion.chromatogram.Ticks.Shadows
                            Dim peak = vector.MRMPeak(baselineQuantile:=baselineQuantile)
                            Dim peakTicks = vector.PickArea(range:=peak)
                            Dim mrm As New MRMPeak With {
                                .Window = peak,
                                .Base = vector.Baseline(baselineQuantile),
                                .Ticks = peakTicks,
                                .PeakHeight = Aggregate t In .Ticks Into Max(t.Intensity)
                            }

                            Return (ion.ion, mrm)
                        End Function) _
                .ToArray

            Return ionData
        End Function
    End Module
End Namespace



