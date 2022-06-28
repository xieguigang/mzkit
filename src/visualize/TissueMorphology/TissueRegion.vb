Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging

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

    Public Overrides Function ToString() As String
        Return $"{label} ({color.ToHtmlColor}) has {nsize} pixels."
    End Function

End Class
