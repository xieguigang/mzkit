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

<Package("tissue")>
Module Tissue

    ''' <summary>
    ''' analysis the HE-stain image by blocks
    ''' </summary>
    ''' <param name="tissue"></param>
    ''' <param name="colors"></param>
    ''' <param name="gridSize"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="densityGrid"></param>
    ''' <returns>
    ''' the collection of the image blocks analysis result
    ''' </returns>
    <ExportAPI("scan_tissue")>
    <RApiReturn(GetType(Cell))>
    Public Function scanTissue(tissue As Image, Optional colors As String() = Nothing,
                               Optional gridSize As Integer = 25,
                               Optional tolerance As Integer = 15,
                               Optional densityGrid As Integer = 5) As Cell()

        Return HistologicalImage.GridScan(
            target:=tissue,
            colors:=colors,
            gridSize:=gridSize,
            tolerance:=tolerance,
            densityGrid:=densityGrid
        ) _
        .ToArray
    End Function

    <ExportAPI("heatmap_layer")>
    <RApiReturn(GetType(PixelData))>
    Public Function getTargets(tissue As Cell(),
                               Optional heatmap As Layers = Layers.Density,
                               Optional target As String = "black") As PixelData()

        Return tissue.GetHeatMapLayer(heatmap, target)
    End Function

    <ExportAPI("RSD")>
    Public Function RSD(layer As PixelData(),
                        Optional nbags As Integer = 300,
                        Optional nsamples As Integer = 32) As Double()

        Return layer.RSD(nbags, nsamples)
    End Function
End Module
