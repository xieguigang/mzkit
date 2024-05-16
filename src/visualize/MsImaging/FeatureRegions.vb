#Region "Microsoft.VisualBasic::da9c4dc46354b0be9a308cea2592ecb3, visualize\MsImaging\FeatureRegions.vb"

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


    ' Code Statistics:

    '   Total Lines: 93
    '    Code Lines: 77
    ' Comment Lines: 5
    '   Blank Lines: 11
    '     File Size: 3.95 KB


    ' Module FeatureRegions
    ' 
    '     Function: GetDimensionSize, TakeRegion, TrimRegion, WriteRegionPoints
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq

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

        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' get the dimension size of the ms-imaging canvas
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <returns></returns>
    <Extension>
    Public Function GetDimensionSize(raw As mzPack) As Size
        Dim allPixels As Point() = raw.MS.Select(Function(p) p.GetMSIPixel).ToArray
        Dim width As Integer = Aggregate pi As Point In allPixels Into Max(pi.X)
        Dim height As Integer = Aggregate pi As Point In allPixels Into Max(pi.Y)

        Return New Size(width, height)
    End Function

    <Extension>
    Public Iterator Function TrimRegion(Of T As IMSIPixel)(layer As IEnumerable(Of T), polygon As Polygon2D, unionSize As Size) As IEnumerable(Of T)
        Dim xy As Index(Of String) = polygon.xpoints _
            .Select(Iterator Function(xi, idx) As IEnumerable(Of String)
                        For i As Integer = 0 To unionSize.Width
                            For j As Integer = 0 To unionSize.Height
                                Yield $"{xi + i},${polygon.ypoints(idx) + j}"
                            Next
                        Next
                    End Function) _
            .IteratesALL _
            .Distinct _
            .Indexing
        Dim takes As New List(Of PixelData)

        For Each p As T In layer
            If Not $"{p.x},{p.y}" Like xy Then
                Yield p
            End If
        Next
    End Function

    <Extension>
    Public Iterator Function TakeRegion(Of T As IMSIPixel)(layer As IEnumerable(Of T), polygon As Polygon2D, unionSize As Size) As IEnumerable(Of T)
        Dim xy = layer _
            .GroupBy(Function(p) p.x) _
            .ToDictionary(Function(xr) xr.Key,
                          Function(xr)
                              Return xr _
                                  .GroupBy(Function(p) p.y) _
                                  .ToDictionary(Function(p) p.Key,
                                                Function(p)
                                                    Return p.First
                                                End Function)
                          End Function)

        For i As Integer = 0 To polygon.length - 1
            Dim x As Integer = polygon.xpoints(i)
            Dim y As Integer = polygon.ypoints(i)

            For xi As Integer = x To x + unionSize.Width
                If xy.ContainsKey(xi) Then
                    For yi As Integer = y To y + unionSize.Height
                        If xy(xi).ContainsKey(yi) Then
                            Yield xy(xi)(yi)
                        End If
                    Next
                End If
            Next
        Next
    End Function

End Module
