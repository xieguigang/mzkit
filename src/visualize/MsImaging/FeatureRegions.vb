Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.BitmapImage

Public Module FeatureRegions

    <Extension>
    Public Function WriteRegionPoints(raw As mzPack, index As IEnumerable(Of NamedValue(Of Point))) As mzPack
        Dim dimensionSize As Size = GetDimensionSize(raw)
        Dim evalIndex As Func(Of Point, Integer) = Function(i) BitmapBuffer.GetIndex(i.X, i.Y, dimensionSize.Width, channels:=1)
        Dim regions As Dictionary(Of String, Point()) = index _
            .GroupBy(Function(i) i.Name) _
            .ToDictionary(Function(region) region.Key,
                          Function(region)
                              Return region.Select(Function(i) i.Value).ToArray
                          End Function)

    End Function

    <Extension>
    Public Function GetDimensionSize(raw As mzPack) As Size
        Dim allPixels As Point() = raw.MS.Select(AddressOf mzPackPixel.GetPixelPoint).ToArray
        Dim width As Integer = Aggregate pi As Point In allPixels Into Max(pi.X)
        Dim height As Integer = Aggregate pi As Point In allPixels Into Max(pi.Y)

        Return New Size(width, height)
    End Function

End Module
