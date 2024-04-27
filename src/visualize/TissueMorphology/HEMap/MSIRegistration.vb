#Region "Microsoft.VisualBasic::e055a893e5243863fc1cf43b0ba79bef, G:/mzkit/src/visualize/TissueMorphology//HEMap/MSIRegistration.vb"

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

    '   Total Lines: 47
    '    Code Lines: 31
    ' Comment Lines: 10
    '   Blank Lines: 6
    '     File Size: 1.89 KB


    '     Module MSIRegistration
    ' 
    '         Function: SpatialTranslation
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace HEMap

    Public Module MSIRegistration

        ''' <summary>
        ''' Apply the spatial mapping between the MSI/HEstain
        ''' </summary>
        ''' <param name="register"></param>
        ''' <param name="MSI"></param>
        ''' <returns></returns>
        <Extension>
        Public Function SpatialTranslation(register As SpatialRegister, MSI As PointF()) As PointF()
            ' Dim shape As New Polygon2D(MSI)
            ' Dim original_offset As PointF = shape.GetRectangle.Location
            Dim newPolygon As New Polygon2D(MSI.Rotate(-register.rotation.ToRadians))
            Dim y As Double() = newPolygon.ypoints
            Dim newDims As Size = newPolygon.GetSize
            Dim dx As New DoubleRange(newPolygon.xpoints)
            Dim dy As New DoubleRange(newPolygon.ypoints)
            Dim scale1 As New DoubleRange(0, register.MSIscale.Width)
            Dim scale2 As New DoubleRange(0, register.MSIscale.Height)

            ' translate the MSI spot to view spot
            MSI = newPolygon.xpoints _
                .Select(Function(xi, i)
                            Return New PointF With {
                                .X = dx.ScaleMapping(xi, scale1),
                                .Y = dy.ScaleMapping(y(i), scale2)
                            }
                        End Function) _
                .ToArray

            ' do offset
            newPolygon = New Polygon2D(MSI) + register.offset

            Return newPolygon.AsEnumerable.ToArray
        End Function
    End Module
End Namespace
