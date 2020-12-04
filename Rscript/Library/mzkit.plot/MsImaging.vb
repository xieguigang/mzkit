
Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Microsoft.VisualBasic.CommandLine.Reflection
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
                          Optional ppm As Double = 5) As Bitmap

        Return viewer.DrawLayer(mz, threshold, InteropArgumentHelper.getSize(pixelSize, "5,5"), ppm)
    End Function
End Module
