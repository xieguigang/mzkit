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