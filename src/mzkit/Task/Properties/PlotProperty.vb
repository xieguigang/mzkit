#Region "Microsoft.VisualBasic::5e57e2fbb14aa6af30803d790c6d5907, mzkit\src\mzkit\Task\Properties\PlotProperty.vb"

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

    '   Total Lines: 51
    '    Code Lines: 39
    ' Comment Lines: 4
    '   Blank Lines: 8
    '     File Size: 3.27 KB


    ' Class PlotProperty
    ' 
    '     Properties: axis_label_font, background, colors, colorSet, gridFill
    '                 height, label_font, legend_font, legend_title, line_width
    '                 padding_bottom, padding_left, padding_right, padding_top, point_size
    '                 show_grid, show_legend, show_tag, title, width
    '                 xlabel, ylabel
    ' 
    '     Function: GetColorSetName, GetPadding
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.MIME.Html.CSS

Public Class PlotProperty

    <Category("Plot")> <Description("The width of the plot image in pixels")> Public Property width As Integer = 2048
    <Category("Plot")> <Description("The height of the plot image in pixels")> Public Property height As Integer = 1600
    <Category("Plot")> <Description("The background color of the plot image")> Public Property background As Color = Color.White
    <Category("Plot")> <Description("Text of the plot its main title")> Public Property title As String
    <Category("Plot")> <Description("Text of the plot its legend title")> Public Property legend_title As String
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
    <Category("Styles")> Public Property label_font As Font = CSSFont.TryParse(CSSFont.Win10NormalLarger).GDIObject(100)
    <Category("Styles")> Public Property legend_font As Font = CSSFont.TryParse(CSSFont.Win10NormalLarge).GDIObject(100)
    <Category("Styles")> Public Property axis_label_font As Font = CSSFont.TryParse(CSSFont.Win10NormalLarge).GDIObject(100)
    <Category("Styles")> <Description("Tweaks of the fill color of the grid background.")> Public Property gridFill As Color = "rgb(245,245,245)".TranslateColor
    <Category("Styles")> Public Property colors As CategoryPalettes = CategoryPalettes.ColorBrewerSet1

    ''' <summary>
    ''' 自定义颜色列表
    ''' </summary>
    ''' <returns></returns>
    <Browsable(False)> Public Property colorSet As String

    Public Function GetColorSetName() As String
        If colors = CategoryPalettes.NA Then
            Return colorSet
        Else
            Return colors.Description
        End If
    End Function

    Public Function GetPadding() As Padding
        Return New Padding(padding_left, padding_top, padding_right, padding_bottom)
    End Function

End Class
