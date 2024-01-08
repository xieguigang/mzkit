#Region "Microsoft.VisualBasic::6222f9fcfb1d8c635db20894cd59a09f, mzkit\Rscript\Library\mzkit.plot\Tissue.vb"

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

'   Total Lines: 41
'    Code Lines: 34
' Comment Lines: 0
'   Blank Lines: 7
'     File Size: 1.49 KB


' Module Tissue
' 
'     Function: getTargets, RSD, scanTissue
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology.HEMap
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime.Interop

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
Module Tissue

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
