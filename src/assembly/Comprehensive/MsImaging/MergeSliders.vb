Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq

Public Module MergeSliders

    ''' <summary>
    ''' Merge multiple sample object into one sample file
    ''' </summary>
    ''' <param name="samples">
    ''' put the samples from left to right based on the 
    ''' data orders in this input sequence.
    ''' </param>
    ''' <param name="relativePos">
    ''' this parameter is set to True by default, which 
    ''' means all of the scan position will be adjusted 
    ''' automatically based on its input orders
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function JoinMSISamples(samples As IEnumerable(Of mzPack), Optional relativePos As Boolean = True) As mzPack
        Dim polygons = samples _
            .Select(Function(ms) (ms, New Polygon2D(ms.MS.Select(Function(a) a.GetMSIPixel)))) _
            .ToArray
        Dim maxHeight As Integer = polygons.Select(Function(a) a.Item2.ypoints).IteratesALL.Max


    End Function
End Module
