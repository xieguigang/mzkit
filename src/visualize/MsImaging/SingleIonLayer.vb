Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Math.Quantile

Public Class SingleIonLayer

    Public Property IonMz As Double
    Public Property MSILayer As PixelData()
    Public Property DimensionSize As Size

    Public Function GetIntensity() As Double()
        Return MSILayer.Select(Function(p) p.intensity).ToArray
    End Function

    Public Function GetQuartile() As DataQuartile
        Return GetIntensity.Quartile
    End Function

    Public Shared Function GetLayer(mz As Double, viewer As Drawer, mzErr As Tolerance) As SingleIonLayer
        Dim pixels As PixelData() = viewer _
            .LoadPixels({mz}, mzErr) _
            .ToArray

        Return New SingleIonLayer With {
            .IonMz = mz,
            .DimensionSize = viewer.dimension,
            .MSILayer = pixels
        }
    End Function

End Class
