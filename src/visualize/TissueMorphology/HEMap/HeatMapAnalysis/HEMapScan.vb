#Region "Microsoft.VisualBasic::fd271c29127132290fea14310f07908b, visualize\TissueMorphology\HEMap\HeatMapAnalysis\HEMapScan.vb"

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

    '   Total Lines: 51
    '    Code Lines: 19
    ' Comment Lines: 27
    '   Blank Lines: 5
    '     File Size: 1.80 KB


    '     Class HEMapScan
    ' 
    '         Properties: block_dims, Blocks, channels, physical_dims
    ' 
    '         Function: CreateSpatialLookup, GetHeatMapLayer
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap

Namespace HEMap

    ''' <summary>
    ''' the scan result of the HE stain image
    ''' </summary>
    ''' <remarks>
    ''' the entire image was divided into multiple blocks <see cref="Cell"/> at first,
    ''' and then the specific color channel layer data was evaluated for run downstream 
    ''' data analysis
    ''' 
    ''' this model is consist with a collection of the block <see cref="Cell"/> scan 
    ''' result.
    ''' </remarks>
    Public Class HEMapScan

        ''' <summary>
        ''' the image analysis data
        ''' </summary>
        ''' <returns></returns>
        Public Property Blocks As Cell()
        ''' <summary>
        ''' the physical dimension size of the raw image input
        ''' </summary>
        ''' <returns></returns>
        Public Property physical_dims As Size
        ''' <summary>
        ''' the block dimension of <see cref="Blocks"/> result
        ''' </summary>
        ''' <returns></returns>
        Public Property block_dims As Size
        ''' <summary>
        ''' the color channels for run the scanning analysis.
        ''' </summary>
        ''' <returns></returns>
        Public Property channels As String()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetHeatMapLayer(heatmap As Layers, channel As String) As PixelData()
            Return Blocks.GetHeatMapLayer(heatmap, channel)
        End Function

        Public Function CreateSpatialLookup() As Grid(Of Cell)
            Return Grid(Of Cell).Create(Blocks)
        End Function
    End Class
End Namespace
