#Region "Microsoft.VisualBasic::24be4ba885e5b6e1865c106a688d9c34, visualize\TissueMorphology\SampleData.vb"

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

    '   Total Lines: 172
    '    Code Lines: 110 (63.95%)
    ' Comment Lines: 42 (24.42%)
    '    - Xml Docs: 95.24%
    ' 
    '   Blank Lines: 20 (11.63%)
    '     File Size: 7.17 KB


    ' Module SampleData
    ' 
    '     Function: BootstrapSample, BootstrapSampleBags, (+2 Overloads) ExtractSample, ExtractSpatialSpots
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Module SampleData

    ''' <summary>
    ''' Analysis sample bootstrapping for a speicifc ion layer feature
    ''' </summary>
    ''' <param name="layer">
    ''' the layer data of target ion m/z in the ms-imaging raw data file
    ''' </param>
    ''' <param name="regions"></param>
    ''' <returns>
    ''' [region_label => [color => expression_vector]]
    ''' </returns>
    <Extension>
    Public Function ExtractSample(Of T As Pixel)(layer As T(),
                                                 regions As IEnumerable(Of TissueRegion),
                                                 Optional n As Integer = 32,
                                                 Optional coverage As Double = 0.3) As Dictionary(Of String, Double())

        Dim data As New Dictionary(Of String, Double())
        Dim matrix = Grid(Of T).Create(layer, Function(i) New Point(i.X, i.Y))

        For Each region As TissueRegion In regions
            data(region.label) = matrix.ExtractSample(region, n, coverage:=coverage)
        Next

        Return data
    End Function

    ''' <summary>
    ''' Make bootstrapping sampling of the spatial pixels inside a given tissue region.
    ''' </summary>
    ''' <param name="region"></param>
    ''' <param name="n"></param>
    ''' <param name="coverage"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function BootstrapSampleBags(region As TissueRegion,
                                                 Optional n As Integer = 32,
                                                 Optional coverage As Double = 0.3) As IEnumerable(Of NamedCollection(Of Point))
        Dim A As Integer = region.points.Length
        Dim Nsize As Integer = A * coverage
        Dim regionId As Integer = 0

        If Nsize <= 0 Then
            Nsize = 1
        End If

        For Each bag As SeqValue(Of Point()) In Bootstraping.Samples(region.points, Nsize, bags:=n)
            regionId += 1
            Yield New NamedCollection(Of Point)($"{region.label}.{regionId}", bag.value)
        Next
    End Function

    ''' <summary>
    ''' Create samples data for a single spatial region
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="matrix"></param>
    ''' <param name="region"></param>
    ''' <param name="n"></param>
    ''' <param name="coverage"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function BootstrapSample(Of T As {RasterPixel, IVector})(matrix As Grid(Of T), region As TissueRegion,
                                                                             Optional n As Integer = 32,
                                                                             Optional coverage As Double = 0.3) As IEnumerable(Of NamedCollection(Of Double))
        Dim A As Integer = region.points.Length
        Dim Nsize As Integer = A * coverage
        Dim dims As Integer = matrix.EnumerateData.First.Data.TryCount

        For Each sample As NamedCollection(Of Point) In Tqdm.Wrap(region.BootstrapSampleBags(n, coverage).ToArray)
            Dim pixelSamples As T() = sample.Select(Function(p) matrix.GetData(p.X, p.Y)).ToArray
            Dim sum As Vector = Nothing

            If pixelSamples.IsNullOrEmpty Then
                Yield New NamedCollection(Of Double)(sample.name, Vector.Zero(dims))
            Else
                For Each p As T In pixelSamples
                    sum = sum + p.Data.AsVector
                Next

                Yield New NamedCollection(Of Double)(sample.name, sum / A)
            End If

            Call App.FlushMemory()
        Next
    End Function

    ''' <summary>
    ''' Create expression samples data for a specific molecule 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="matrix">the sptial rawdata matrix</param>
    ''' <param name="region"></param>
    ''' <param name="n"></param>
    ''' <param name="coverage"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ExtractSample(Of T As Pixel)(matrix As Grid(Of T), region As TissueRegion,
                                                 Optional n As Integer = 32,
                                                 Optional coverage As Double = 0.3) As Double()
        Dim A As Integer = region.points.Length
        Dim Nsize As Integer = A * coverage
        ' make bootstrapping sampling of the spatial spots
        Dim samples = Bootstraping.Samples(region.points, Nsize, bags:=n).ToArray
        Dim vec As Double() = samples _
            .AsParallel _
            .Select(Function(pack)
                        Dim subset As T() = pack.value _
                            .Select(Function(pt)
                                        ' get spatial data from a sampling result
                                        Return matrix.GetData(pt.X, pt.Y)
                                    End Function) _
                            .Where(Function(p) Not p Is Nothing) _
                            .ToArray
                        Dim d As Double() = subset _
                            .Select(Function(i) i.Scale) _
                            .ToArray

                        If d.Length = 0 Then
                            Return 0.0
                        Else
                            Return d.Sum / A
                        End If
                    End Function) _
            .ToArray

        Return vec
    End Function

    ''' <summary>
    ''' Convert the tissue regions as the cluster labelled spots
    ''' </summary>
    ''' <param name="regions"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function ExtractSpatialSpots(regions As IEnumerable(Of TissueRegion)) As IEnumerable(Of PhenographSpot)
        For Each region As TissueRegion In regions
            Dim tags As String() = region.tags
            Dim region_color As String = region.color.ToHtmlColor

            If tags.IsNullOrEmpty Then
                tags = New String(region.nsize - 1) {}
            End If

            For i As Integer = 0 To region.nsize - 1
                Dim pt As Point = region.points(i)
                Dim spot As New PhenographSpot With {
                    .color = region_color,
                    .x = pt.X,
                    .y = pt.Y,
                    .id = $"{ .x},{ .y}",
                    .phenograph_cluster = region.label,
                    .sample_tag = tags(i)
                }

                Yield spot
            Next
        Next
    End Function
End Module
