#Region "Microsoft.VisualBasic::5baddbbae79774bf2dc3dbe5334f89fd, visualize\MsImaging\Plot\MSIRegionPlot.vb"

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

    '   Total Lines: 35
    '    Code Lines: 25 (71.43%)
    ' Comment Lines: 3 (8.57%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (20.00%)
    '     File Size: 1.35 KB


    ' Class MSIRegionPlot
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: MeasureRegionPolygon
    ' 
    '     Sub: PlotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares

''' <summary>
''' plot a specific spatial cluster region
''' </summary>
Public Class MSIRegionPlot : Inherits Plot

    Public Sub New(theme As Theme)
        MyBase.New(theme)
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Throw New NotImplementedException()
    End Sub

    Public Shared Function MeasureRegionPolygon(x As Integer(), y As Integer(),
                                                Optional scale As Integer = 5,
                                                Optional degree As Double = 20,
                                                Optional resolution As Integer = 100,
                                                Optional q As Double = 0.1) As GeneralPath

        Dim shape As GeneralPath = ContourLayer.GetOutline(x, y, scale)

        If degree > 0 AndAlso resolution > 0 Then
            shape = shape.Bspline(degree, resolution)
            shape = shape.FilterSmallPolygon(q)
        End If

        Return shape
    End Function
End Class
