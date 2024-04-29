#Region "Microsoft.VisualBasic::a1519aedde0bbed7b500fe9dff08d9ee, E:/mzkit/Rscript/Library/mzkit_app/src/mzplot//Tissue.vb"

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

    '   Total Lines: 190
    '    Code Lines: 111
    ' Comment Lines: 60
    '   Blank Lines: 19
    '     File Size: 8.70 KB


    ' Module Tissue
    ' 
    '     Function: blockMapDf, cellsDf, getTargets, mark_nucleus, RSD
    '               scanTissue
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology.HEMap
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

''' <summary>
''' tools for HE-stain image analysis
''' 
''' Pathology is practiced by visual inspection of histochemically stained tissue slides. 
''' While the hematoxylin and eosin (H&amp;E) stain is most commonly used, special stains can
''' provide additional contrast to different tissue components.
''' 
''' Histological analysis of stained human tissue samples is the gold standard for evaluation 
''' of many diseases, as the fundamental basis of any pathologic evaluation is the examination
''' of histologically stained tissue affixed on a glass slide using either a microscope or 
''' a digitized version of the histologic image following the image capture by a whole slide
''' image (WSI) scanner. The histological staining step is a critical part of the pathology
''' workflow and is required to provide contrast and color to tissue by facilitating a chromatic 
''' distinction among different tissue constituents. The most common stain (otherwise referred 
''' to as the routine stain) is the hematoxylin and eosin (H&amp;E), which is applied to nearly 
''' all clinical cases, covering ~80% of all the human tissue staining performed globally1. 
''' The H&amp;E stain is relatively easy to perform and is widely used across the industry. 
''' In addition to H&amp;E, there are a variety of other histological stains with different
''' properties which are used by pathologists to better highlight different tissue 
''' constituents.
''' </summary>
<Package("tissue")>
<RTypeExport("he_map", GetType(HEMapScan))>
Module Tissue

    Sub Main()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(HEMapScan), AddressOf blockMapDf)
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(Cell()), AddressOf blockMapDf)
    End Sub

    Private Function blockMapDf(cells As HEMapScan, args As list, env As Environment) As dataframe
        Return cellsDf(cells.Blocks, args, env)
    End Function

    Private Function cellsDf(cells As Cell(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = cells _
                .Select(Function(c) $"{c.X},{c.Y}") _
                .ToArray
        }

        Call df.add("physic_x", cells.Select(Function(c) c.X))
        Call df.add("physic_y", cells.Select(Function(c) c.Y))
        Call df.add("block_x", cells.Select(Function(c) c.ScaleX))
        Call df.add("block_y", cells.Select(Function(c) c.ScaleY))
        Call df.add("r", cells.Select(Function(c) c.R))
        Call df.add("g", cells.Select(Function(c) c.G))
        Call df.add("b", cells.Select(Function(c) c.B))
        Call df.add("black_pixels", cells.Select(Function(c) c.black.Pixels))
        Call df.add("black_density", cells.Select(Function(c) c.black.Density))
        Call df.add("black_ratio", cells.Select(Function(c) c.black.Ratio))

        Dim all_colors As String() = cells _
            .Select(Function(c) c.layers.Keys) _
            .IteratesALL _
            .Distinct _
            .ToArray

        For Each color As String In all_colors
            Call df.add($"{color}_pixels", cells.Select(Function(c) c.GetChannelValue(color, Layers.Pixels)))
            Call df.add($"{color}_density", cells.Select(Function(c) c.GetChannelValue(color, Layers.Density)))
            Call df.add($"{color}_ratio", cells.Select(Function(c) c.GetChannelValue(color, Layers.Ratio)))
        Next

        Return df
    End Function

    ''' <summary>
    ''' analysis the HE-stain image by blocks
    ''' </summary>
    ''' <param name="tissue">the image of the HE-stain tissue image</param>
    ''' <param name="colors">target colors for run scaning</param>
    ''' <param name="grid_size">grid size for divid the image into blocks</param>
    ''' <param name="tolerance">the color tolerance value</param>
    ''' <param name="density_grid">the grid size for evaluate the color density</param>
    ''' <returns>
    ''' the collection of the image blocks analysis result
    ''' </returns>
    <ExportAPI("scan_tissue")>
    <RApiReturn(GetType(HEMapScan))>
    Public Function scanTissue(tissue As Image, Optional colors As String() = Nothing,
                               Optional grid_size As Integer = 25,
                               Optional tolerance As Integer = 15,
                               Optional density_grid As Integer = 5) As Object

        Dim cells As Cell() = HistologicalImage.GridScan(
            target:=tissue,
            colors:=colors,
            gridSize:=grid_size,
            tolerance:=tolerance,
            densityGrid:=density_grid
        ) _
        .ToArray

        Return New HEMapScan With {
            .Blocks = cells,
            .channels = colors,
            .physical_dims = tissue.Size,
            .block_dims = New Size(
                .physical_dims.Width / grid_size,
                .physical_dims.Height / grid_size
            )
        }
    End Function

    ''' <summary>
    ''' extract the possible nucleus pixel points from the image
    ''' </summary>
    ''' <param name="tissue">the H&amp;E stain image</param>
    ''' <param name="nucleus">the target color of the nucleus</param>
    ''' <param name="env"></param>
    ''' <returns>a dataframe object that contains the nucleus pixel location 
    ''' result, this result dataframe object contains at least two data 
    ''' fields: ``x`` and ``y``. such geometry pixel position could be used 
    ''' for the downstream data analysis pipeline.</returns>
    <ExportAPI("mark_nucleus")>
    Public Function mark_nucleus(tissue As Image,
                                 <RRawVectorArgument(TypeCodes.string)>
                                 Optional nucleus As Object = "#8230b8|#903ab5|#8826b1|#9026ae|#c23ec5",
                                 Optional tolerance As Double = 13,
                                 Optional env As Environment = Nothing) As Object

        Dim nucleus_colors = CLRVector.asCharacter(nucleus) _
            .SafeQuery _
            .Select(Function(c) c.TranslateColor) _
            .ToArray
        Dim scan_xy = tissue.ScanColor(targets:=nucleus_colors, tolerance) _
            .Select(Function(a) a.pos) _
            .Distinct _
            .ToArray
        Dim nucleus_df As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        Call nucleus_df.add("x", scan_xy.Select(Function(p) p.X))
        Call nucleus_df.add("y", scan_xy.Select(Function(p) p.Y))

        Return nucleus_df
    End Function

    ''' <summary>
    ''' generates heatmap value
    ''' 
    ''' convert a specific <see cref="Layers"/> channel inside the tissue 
    ''' image scanning result as spatial heatmap matrix data.
    ''' </summary>
    ''' <param name="tissue"></param>
    ''' <param name="heatmap">the <see cref="Layers"/> channel scan for the target colors 
    ''' for heatmap rendering.</param>
    ''' <param name="target">the target color channel</param>
    ''' <returns></returns>
    <ExportAPI("heatmap_layer")>
    <RApiReturn(GetType(PixelData))>
    Public Function getTargets(tissue As HEMapScan,
                               Optional heatmap As Layers = Layers.Density,
                               Optional target As String = "black") As Object

        Return tissue.GetHeatMapLayer(heatmap, target)
    End Function

    ''' <summary>
    ''' evaluate the spatial RSD of a specific channel
    ''' </summary>
    ''' <param name="layer"></param>
    ''' <param name="nbags"></param>
    ''' <param name="nsamples"></param>
    ''' <returns>a numeric vector of the rsd value.</returns>
    <ExportAPI("RSD")>
    <RApiReturn(GetType(Double))>
    Public Function RSD(layer As PixelData(),
                        Optional nbags As Integer = 300,
                        Optional nsamples As Integer = 32) As Object

        Return layer.RSD(nbags, nsamples)
    End Function
End Module
