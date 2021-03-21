#Region "Microsoft.VisualBasic::3fd6b0ef538e99977e01e2e82ba76be1, src\mzkit\Task\DrawScatter.vb"

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

    ' Module DrawScatter
    ' 
    '     Function: Draw3DPeaks, (+2 Overloads) DrawScatter
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Quantile

Public Module DrawScatter

    <Extension>
    Public Function Draw3DPeaks(raw As Raw) As Image
        Dim ms1 As ms1_scan() = raw _
            .GetMs1Scans _
            .Select(Function(m1)
                        Return m1.mz.Select(Function(mzi, i) New ms1_scan With {.mz = mzi, .intensity = m1.into(i), .scan_time = m1.rt})
                    End Function) _
            .IteratesALL _
            .ToArray
        Dim maxinto As Double = ms1.Select(Function(x) x.intensity).GKQuantile.Query(0.8)
        Dim XIC = ms1 _
            .GroupBy(Function(m) m.mz, Tolerance.DeltaMass(0.1)) _
            .Select(Function(mz)
                        Return New NamedCollection(Of ChromatogramTick) With {
                            .name = mz.name,
                            .value = mz _
                                .Where(Function(t) t.intensity >= maxinto) _
                                .OrderBy(Function(t) t.scan_time) _
                                .Select(Function(t)
                                            Return New ChromatogramTick With {.Time = t.scan_time, .Intensity = t.intensity}
                                        End Function) _
                                .ToArray
                        }
                    End Function) _
            .Where(Function(c) c.Length > 3) _
            .ToArray

        Return XIC.TICplot(parallel:=True, showLabels:=False, showLegends:=False).AsGDIImage
    End Function

    <Extension>
    Public Function DrawScatter(raw As Raw) As Image
        Dim ms1 As ms1_scan() = raw _
            .GetMs1Scans _
            .Select(Function(m1)
                        Return m1.mz.Select(Function(mzi, i) New ms1_scan With {.mz = mzi, .intensity = m1.into(i), .scan_time = m1.rt})
                    End Function) _
            .IteratesALL _
            .ToArray

        Return RawScatterPlot.Plot(samples:=ms1, rawfile:=raw.source.FileName).AsGDIImage
    End Function

    <Extension>
    Public Function DrawScatter(raw As mzPack) As Image
        Dim ms1 As ms1_scan() = raw.MS _
            .Select(Function(m1)
                        Return m1.mz.Select(Function(mzi, i) New ms1_scan With {.mz = mzi, .intensity = m1.into(i), .scan_time = m1.rt})
                    End Function) _
            .IteratesALL _
            .ToArray

        Return RawScatterPlot.Plot(samples:=ms1).AsGDIImage
    End Function
End Module

