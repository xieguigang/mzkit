#Region "Microsoft.VisualBasic::9a7d7e79f3b7584f3af0e519c65d919e, G:/mzkit/src/visualize/MsImaging//Layer/LayerHelpers.vb"

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

    '   Total Lines: 116
    '    Code Lines: 98
    ' Comment Lines: 8
    '   Blank Lines: 10
    '     File Size: 4.86 KB


    ' Module LayerHelpers
    ' 
    '     Function: evalMz, GetMSIIons
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.DataMining.DensityQuery
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports stdNum = System.Math

Public Module LayerHelpers

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="mzdiff"></param>
    ''' <returns>
    ''' a collection of m/z tagged density value
    ''' </returns>
    <Extension>
    Public Iterator Function GetMSIIons(raw As mzPack,
                                        Optional mzdiff As Tolerance = Nothing,
                                        Optional gridSize As Integer = 5,
                                        Optional qcut As Double = 0.05,
                                        Optional intoCut As Double = 0,
                                        Optional verbose As Boolean = True) As IEnumerable(Of DoubleTagged(Of SingleIonLayer))

        Dim cellSize As New Size(gridSize, gridSize)
        Dim graph As Grid(Of ScanMS1) = Grid(Of ScanMS1).Create(raw.MS, Function(scan) scan.GetMSIPixel)
        Dim mzErr As Tolerance = mzdiff Or Tolerance.DefaultTolerance
        Dim reader As PixelReader = New ReadRawPack(raw, verbose:=verbose)
        Dim ncut As Integer = graph.size * qcut
        Dim allMz As NamedCollection(Of (mzi As ms2, pt As Point))() = graph _
            .EnumerateData _
            .AsParallel _
            .Select(Function(scan)
                        Dim pt As Point = scan.GetMSIPixel

                        Return scan _
                            .GetMs _
                            .ToArray _
                            .Centroid(mzErr, New RelativeIntensityCutoff(intoCut)) _
                            .Select(Function(mzi) (mzi, pt))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(d) d.mzi.mz, mzErr) _
            .Where(Function(d) d.Count > ncut) _
            .ToArray
        Dim k As Integer = allMz.Length / 10
        Dim j As i32 = 0
        Dim info As String

        For i As Integer = 0 To allMz.Length - 1
            Yield allMz(i).evalMz(graph, cellSize)

            If ++j = k Then
                j = 0
                info = $"({CInt(100 * i / allMz.Length)}%) {Val(allMz(i).name).ToString("F4")}"

                Call RunSlavePipeline.SendProgress(stdNum.Round(i / allMz.Length, 2), info)
            End If
        Next
    End Function

    <Extension>
    Private Function evalMz(allMz As NamedCollection(Of (mzi As ms2, pt As Point)), graph As Grid(Of ScanMS1), cellSize As Size) As DoubleTagged(Of SingleIonLayer)
        Dim mz As Double = allMz _
            .OrderByDescending(Function(d) d.mzi.intensity) _
            .First _
            .mzi _
            .mz
        Dim layer As New SingleIonLayer With {
            .IonMz = mz.ToString("F4"),
            .DimensionSize = New Size(graph.width, graph.height),
            .MSILayer = Grid(Of (mzi As ms2, pt As Point)) _
                .Create(allMz, Function(d) d.Item2) _
                .EnumerateData _
                .Select(Function(d)
                            Return New PixelData With {
                                .intensity = d.mzi.intensity,
                                .level = 0,
                                .mz = d.mzi.mz,
                                .x = d.pt.X,
                                .y = d.pt.Y
                            }
                        End Function) _
                .ToArray
        }
        Dim density As NamedValue(Of Double)() = layer.MSILayer _
            .Density(
                getName:=Function(pt) $"{pt.x},{pt.y}",
                getX:=Function(p) p.x,
                getY:=Function(p) p.y,
                gridSize:=cellSize,
                parallel:=True
            ) _
            .ToArray
        Dim q As Double = density.Select(Function(d) d.Value).Median

        Return New DoubleTagged(Of SingleIonLayer) With {
            .Tag = mz,
            .Value = layer,
            .TagStr = q
        }
    End Function

End Module
