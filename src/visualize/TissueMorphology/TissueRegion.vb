Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Math2D

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

    Public Overrides Function ToString() As String
        Return $"{label} ({color.ToHtmlColor}) has {nsize} pixels."
    End Function

End Class
