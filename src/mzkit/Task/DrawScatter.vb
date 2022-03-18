#Region "Microsoft.VisualBasic::f62d380127b07c1b330191eed21d752f, mzkit\src\mzkit\Task\DrawScatter.vb"

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

    '   Total Lines: 104
    '    Code Lines: 93
    ' Comment Lines: 0
    '   Blank Lines: 11
    '     File Size: 4.50 KB


    ' Module DrawScatter
    ' 
    '     Function: Draw3DPeaks, (+2 Overloads) DrawScatter, GetContourData, (+2 Overloads) GetMs1Points
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Serialization.JSON

Public Module DrawScatter

    <Extension>
    Public Function GetContourData(raw As Raw) As ContourLayer()
        Dim cacheRaw As String = raw.cache
        Dim output_cache As String = TempFileSystem.GetAppSysTempFile("__save.json", App.PID.ToHexString, "contour_layers_")
        Dim cli As String = $"""{RscriptPipelineTask.GetRScript("ms1_contour.R")}"" --mzPack ""{cacheRaw}"" --cache ""{output_cache}"""
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Rscript.Path, cli)

        Call cli.__DEBUG_ECHO
        Call pipeline.Run()

        Return output_cache.LoadJsonFile(Of ContourLayer())
    End Function

    <Extension>
    Public Function Draw3DPeaks(raw As Raw, colorSet As String, size$, padding$) As Image
        Dim ms1 As ms1_scan() = GetMs1Points(raw)
        Dim maxinto As Double = ms1.Select(Function(x) x.intensity).Quartile.Q3
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

        Return XIC.TICplot(
            parallel:=True,
            showLabels:=False,
            showLegends:=False,
            colorsSchema:=colorSet,
            fillAlpha:=60,
            margin:=padding,
            gridFill:="white",
            size:=size
        ).AsGDIImage
    End Function

    Private Function GetMs1Points(raw As Raw) As ms1_scan()
        Return raw.GetMs1Scans.GetMs1Points
    End Function

    <Extension>
    Public Function GetMs1Points(raw As IEnumerable(Of ScanMS1)) As ms1_scan()
        Return raw _
            .Select(Function(m1)
                        Return m1.mz _
                            .Select(Function(mzi, i)
                                        Return New ms1_scan With {
                                            .mz = mzi,
                                            .intensity = m1.into(i),
                                            .scan_time = m1.rt
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToArray
    End Function

    <Extension>
    Public Function DrawScatter(raw As Raw, colorSet As String) As Image
        Return RawScatterPlot.Plot(samples:=GetMs1Points(raw), rawfile:=raw.source.FileName, sampleColors:=colorSet).AsGDIImage
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
