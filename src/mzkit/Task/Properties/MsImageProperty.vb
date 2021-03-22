#Region "Microsoft.VisualBasic::cb07909d39ee582dbac4f0c62db0616c, src\mzkit\Task\Properties\MsImageProperty.vb"

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

    ' Class MsImageProperty
    ' 
    '     Properties: background, colors, fileSize, mapLevels, method
    '                 pixel_height, pixel_width, scan_x, scan_y, threshold
    '                 tolerance, UUID
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetTolerance
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors

Public Class MsImageProperty

    <Category("imzML")> Public ReadOnly Property fileSize As String
    <Category("imzML")> Public ReadOnly Property UUID As String
    <Category("imzML")> Public ReadOnly Property scan_x As Integer
    <Category("imzML")> Public ReadOnly Property scan_y As Integer

    <Category("Render")> Public Property background As Color
    <Category("Render")> <DisplayName("width")> Public Property pixel_width As Integer = 10
    <Category("Render")> <DisplayName("height")> Public Property pixel_height As Integer = 10
    <Category("Render")> Public Property threshold As Double = 0.01
    <Category("Render")> Public Property colors As Palettes = Palettes.Office2016
    <Category("Render")> Public Property mapLevels As Integer = 30

    <Category("Pixel M/z Data")> Public Property tolerance As Double = 0.1
    <Category("Pixel M/z Data")> Public Property method As ToleranceMethod = ToleranceMethod.Da

    Sub New(render As Drawer)
        scan_x = render.dimension.Width
        scan_y = render.dimension.Height
        background = Color.White
        UUID = render.UUID
        fileSize = StringFormats.Lanudry(render.ibd.size)
    End Sub

    Public Function GetTolerance() As Tolerance
        If method = ToleranceMethod.Da Then
            Return Ms1.Tolerance.DeltaMass(tolerance)
        Else
            Return Ms1.Tolerance.PPM(tolerance)
        End If
    End Function
End Class

