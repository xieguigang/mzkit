Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver

''' <summary>
''' helper module for processing haematoxylin and eosin staining image
''' </summary>
Public Module HistologicalImage

    ''' <summary>
    ''' convert to gray scale
    ''' </summary>
    ''' <param name="HE"></param>
    ''' <returns></returns>
    <Extension>
    Public Function MonoScale(HE As Image) As Image
        Return HE.Grayscale
    End Function

    <Extension>
    Public Function HeatMap(HE As Image,
                            Optional scale As ScalerPalette = ScalerPalette.turbo,
                            Optional mapLevels As Integer = 64) As GraphicsData

        Return HE.Image2DMap(
            scaleName:=scale.Description,
            mapLevels:=mapLevels
        )
    End Function

End Module
