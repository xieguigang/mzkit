#Region "Microsoft.VisualBasic::f6804942c151fe61b27a27863fdc87dd, visualize\TissueMorphology\SampleData.vb"

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

    '   Total Lines: 84
    '    Code Lines: 63 (75.00%)
    ' Comment Lines: 10 (11.90%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 11 (13.10%)
    '     File Size: 3.33 KB


    ' Module SampleData
    ' 
    '     Function: ExtractSample, ExtractSpatialSpots
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Math.Distributions

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
            Dim A As Integer = region.points.Length
            Dim Nsize As Integer = A * coverage
            Dim samples = Bootstraping.Samples(region.points, Nsize, bags:=n).ToArray
            Dim vec = samples _
                .AsParallel _
                .Select(Function(pack)
                            Dim subset As T() = pack.value _
                                .Select(Function(pt)
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

            data(region.label) = vec
        Next

        Return data
    End Function

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
