#Region "Microsoft.VisualBasic::654b2236f55e8e429d548c7e4e4afbb7, src\mzkit\Task\Properties\PlotProperty.vb"

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

    ' Class PlotProperty
    ' 
    '     Properties: background, gridFill, height, line_width, padding_bottom
    '                 padding_left, padding_right, padding_top, point_size, show_grid
    '                 show_legend, show_tag, title, width, xlabel
    '                 ylabel
    ' 
    '     Function: GetPadding
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS

Public Class PlotProperty

    <Category("Plot")> <Description("The width of the plot image in pixels")> Public Property width As Integer = 2048
    <Category("Plot")> <Description("The height of the plot image in pixels")> Public Property height As Integer = 1600
    <Category("Plot")> <Description("The background color of the plot image")> Public Property background As Color = Color.White
    <Category("Plot")> <Description("Text of the plot its main title")> Public Property title As String
    <Category("Plot")> <Description("Text label of the X axis")> Public Property xlabel As String = "X"
    <Category("Plot")> <Description("Text label of the Y axis")> Public Property ylabel As String = "Y"

    <Category("Padding")> <DisplayName("top")> Public Property padding_top As Integer = 100
    <Category("Padding")> <DisplayName("left")> Public Property padding_left As Integer = 100
    <Category("Padding")> <DisplayName("right")> Public Property padding_right As Integer = 100
    <Category("Padding")> <DisplayName("bottom")> Public Property padding_bottom As Integer = 100

    <Category("Styles")> <Description("Show data legend on the plot image?")> Public Property show_legend As Boolean = True
    <Category("Styles")> <Description("Show data grid of the data plot in the background?")> Public Property show_grid As Boolean = True
    <Category("Styles")> <Description("Show data tag label for the interested object of the data plot?")> Public Property show_tag As Boolean = True
    <Category("Styles")> <Description("Tweaks of the line width of the data line plot.")> Public Property line_width As Single = 5
    <Category("Styles")> <Description("Tweaks of the point size of the data scatter plot.")> Public Property point_size As Single = 10
    <Category("Styles")> <Description("Tweaks of the fill color of the grid background.")> Public Property gridFill As Color = "rgb(245,245,245)".TranslateColor

    Public Function GetPadding() As Padding
        Return New Padding(padding_left, padding_top, padding_right, padding_bottom)
    End Function

End Class

