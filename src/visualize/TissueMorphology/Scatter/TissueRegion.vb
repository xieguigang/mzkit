#Region "Microsoft.VisualBasic::c2b555d63d7827d323866bf8be262d47, mzkit\src\visualize\TissueMorphology\Scatter\TissueRegion.vb"

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

    '   Total Lines: 57
    '    Code Lines: 30
    ' Comment Lines: 19
    '   Blank Lines: 8
    '     File Size: 1.71 KB


    ' Class TissueRegion
    ' 
    '     Properties: color, label, nsize, points
    ' 
    '     Function: GetPolygons, IsInside, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Math2D

''' <summary>
''' a tissue polygon region object
''' </summary>
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
    ''' <remarks>
    ''' A raster matrix
    ''' </remarks>
    Public Property points As Point()

    ''' <summary>
    ''' the sample tags of the <see cref="points"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property tags As String()

    ''' <summary>
    ''' the pixel point count of the current region
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property nsize As Integer
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return points.Length
        End Get
    End Property

    Public Function GetRectangle() As Rectangle
        Dim x = points.Select(Function(p) p.X).ToArray
        Dim y = points.Select(Function(p) p.Y).ToArray
        Dim pos As New Point(x.Min, y.Min)
        Dim size As New Size(x.Max - x.Min, y.Max - y.Min)

        Return New Rectangle(pos, size)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Iterator Function GetPolygons() As IEnumerable(Of Polygon2D)
        Yield New Polygon2D(
            x:=points.Select(Function(i) CDbl(i.X)).ToArray,
            y:=points.Select(Function(i) CDbl(i.Y)).ToArray
        )
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IsInside(px As Integer, py As Integer) As Boolean
        Return points.Any(Function(p) p.X = px AndAlso p.Y = py)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Dim rect = GetRectangle()
        Return $"{label} ({color.ToHtmlColor}) has {nsize} pixels. #({rect.X},{rect.Y}) size={rect.Width}x{rect.Height}"
    End Function

End Class
