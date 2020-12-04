
Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("MsImaging")>
Module MsImaging

    <ExportAPI("viewer")>
    Public Function viewer(imzML As String) As Drawer
        Return New Drawer(imzML)
    End Function

    <ExportAPI("layer")>
    Public Function layer(viewer As Drawer, mz As Double,
                          Optional threshold As Double = 0.1,
                          <RRawVectorArgument>
                          Optional pixelSize As Object = "5,5",
                          Optional ppm As Double = 5,
                          Optional color$ = "YlGnBu:c8") As Bitmap

        Return viewer.DrawLayer(mz, threshold, InteropArgumentHelper.getSize(pixelSize, "5,5"), ppm, color)
    End Function

    ''' <summary>
    ''' flatten image layers
    ''' </summary>
    ''' <param name="layers">
    ''' layer bitmaps should be all in equal size
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("flatten")>
    Public Function flatten(layers As Bitmap()) As Bitmap
        Using g As Graphics2D = New Bitmap(layers(Scan0).Width, layers(Scan0).Height)
            For Each layer As Bitmap In layers
                Call g.DrawImageUnscaled(layer, New Point)
            Next

            Return g.ImageResource
        End Using
    End Function
End Module
