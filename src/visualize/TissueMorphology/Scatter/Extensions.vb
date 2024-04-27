#Region "Microsoft.VisualBasic::6c2cb4922fa75f89ed5e618197c887d7, G:/mzkit/src/visualize/TissueMorphology//Scatter/Extensions.vb"

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

    '   Total Lines: 89
    '    Code Lines: 69
    ' Comment Lines: 9
    '   Blank Lines: 11
    '     File Size: 3.20 KB


    ' Module Extensions
    ' 
    '     Function: RasterGeometry2D, (+2 Overloads) ScalePixels
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq

<HideModuleName>
Public Module Extensions

    <Extension>
    Public Iterator Function ScalePixels(region As IEnumerable(Of TissueRegion),
                                         newDims As Size,
                                         Optional currentDims As Size = Nothing) As IEnumerable(Of TissueRegion)

        Dim tissue_map As TissueRegion() = region.ToArray

        If currentDims.IsEmpty Then
            currentDims = tissue_map.GetDimension
        End If

        For Each subregion As TissueRegion In tissue_map
            Yield subregion.ScalePixels(currentDims, newDims)
        Next
    End Function

    <Extension>
    Public Function ScalePixels(region As TissueRegion, currentDims As Size, newDims As Size) As TissueRegion
        Dim scalX As Double = newDims.Width / currentDims.Width
        Dim scalY As Double = newDims.Height / currentDims.Height

        Return New TissueRegion With {
            .color = region.color,
            .label = region.label,
            .points = region.points _
                .Select(Function(pi)
                            Return New Point(pi.X * scalX, pi.Y * scalY)
                        End Function) _
                .ToArray
        }
    End Function

    ''' <summary>
    ''' make polygon shape object raster matrix
    ''' </summary>
    ''' <param name="polygons"></param>
    ''' <param name="dimension"></param>
    ''' <param name="label"></param>
    ''' <param name="color"></param>
    ''' <returns></returns>
    <Extension>
    Public Function RasterGeometry2D(polygons As IEnumerable(Of Polygon2D),
                                     dimension As Size,
                                     label As String,
                                     color As Color) As TissueRegion

        Dim x As New List(Of Integer)
        Dim y As New List(Of Integer)
        Dim regions As Polygon2D() = polygons _
            .SafeQuery _
            .Where(Function(p) p.length > 0) _
            .ToArray

        If regions.Length = 1 AndAlso regions(Scan0).length > 512 Then
            ' is already a pack of density pixels
            Call x.AddRange(regions(Scan0).xpoints.Select(Function(xi) CInt(xi)))
            Call y.AddRange(regions(Scan0).ypoints.Select(Function(yi) CInt(yi)))
        Else
            For i As Integer = 1 To dimension.Width
                For j As Integer = 1 To dimension.Height
#Disable Warning
                    If regions.Any(Function(r) r.inside(i, j)) Then
                        Call x.Add(i)
                        Call y.Add(j)
                    End If
#Enable Warning
                Next
            Next
        End If

        Return New TissueRegion With {
            .color = color,
            .label = label,
            .points = x _
                .Select(Function(xi, i)
                            Return New Point(xi, y(i))
                        End Function) _
                .ToArray
        }
    End Function
End Module
