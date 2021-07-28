#Region "Microsoft.VisualBasic::24f58e28ea53c3dc1fc15f12fe98501b, src\visualize\MsImaging\FeatureRegions.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    ' Module FeatureRegions
    ' 
    '     Function: GetDimensionSize, WriteRegionPoints
    ' 
    ' /********************************************************************************/

#End Region

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

