Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Math2D

''' <summary>
''' a tissue polygon region object
''' </summary>
Public Class TissueRegion

    ''' <summary>
    ''' the unique id or the unique tissue tag name
    ''' </summary>
    ''' <returns></returns>
    Public Property label As String
    Public Property color As Color

    ''' <summary>
    ''' a collection of the pixels which is belongs to
    ''' current Tissue Morphology region.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' A raster matrix
    ''' </remarks>
    Public Property points As Point()

    ''' <summary>
    ''' the pixel point count of the current region
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property nsize As Integer
        Get
            Return points.Length
        End Get
    End Property

    Public Iterator Function GetPolygons() As IEnumerable(Of Polygon2D)
        Yield New Polygon2D(
            x:=points.Select(Function(i) CDbl(i.X)).ToArray,
            y:=points.Select(Function(i) CDbl(i.Y)).ToArray
        )
    End Function

    Public Function IsInside(px As Integer, py As Integer) As Boolean
        Return points.Any(Function(p) p.X = px AndAlso p.Y = py)
    End Function

    Public Overrides Function ToString() As String
        Return $"{label} ({color.ToHtmlColor}) has {nsize} pixels."
    End Function

End Class
