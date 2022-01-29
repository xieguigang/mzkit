#Region "Microsoft.VisualBasic::92ede38e3fc01272d1278cb3c645170d, src\mzkit\Task\Properties\MsImageProperty.vb"

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

    ' Enum SmoothFilters
    ' 
    '     Gauss, GaussMax, GaussMean, GaussMedian, GaussMin
    '     Max, Mean, Median, Min, None
    ' 
    '  
    ' 
    ' 
    ' 
    ' Class MsImageProperty
    ' 
    '     Properties: background, colors, fileSize, imageSmooth, logE
    '                 lowerbound, mapLevels, max, maxCut, method
    '                 min, pixel_height, pixel_width, scale, scan_x
    '                 scan_y, tolerance, upperbound, UUID
    ' 
    '     Constructor: (+3 Overloads) Sub New
    ' 
    '     Function: GetMSIInfo, GetTolerance, Smooth
    ' 
    '     Sub: Reset, SetIntensityMax
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Filters
Imports Microsoft.VisualBasic.Linq

Public Enum SmoothFilters
    None
    Gauss
    Median
    Mean
    Min
    Max
    GaussMedian
    GaussMean
    GaussMin
    GaussMax
End Enum

Public Class MsImageProperty

    <Category("imzML")> Public ReadOnly Property fileSize As String
    <Category("imzML")> Public ReadOnly Property UUID As String
    <Category("imzML")> Public ReadOnly Property scan_x As Integer
    <Category("imzML")> Public ReadOnly Property scan_y As Integer

    <Category("Render")> Public Property background As Color
    <Category("Render")> <DisplayName("width")> Public Property pixel_width As Integer = 10
    <Category("Render")> <DisplayName("height")> Public Property pixel_height As Integer = 10

    <Description("Log(e) transformation of the intensity value?")>
    <Category("Render")> Public Property logE As Boolean = True
    <Category("Render")> Public Property colors As ScalerPalette = ScalerPalette.viridis
    <Category("Render") > Public Property mapLevels As Integer = 30
    <Category("Render")> Public Property imageSmooth As SmoothFilters
    <Category("Render")> Public Property scale As InterpolationMode = InterpolationMode.Bilinear

    <Category("Pixel M/z Data")> Public Property tolerance As Double = 0.1
    <Category("Pixel M/z Data")> Public Property method As ToleranceMethod = ToleranceMethod.Da

    <Category("Intensity")> Public ReadOnly Property min As Double
    <Category("Intensity")> Public ReadOnly Property max As Double

    <Category("Intensity")> Public Property upperbound As Double
    <Category("Intensity")> Public Property lowerbound As Double
    <Category("Intensity")> Public Property maxCut As Double = 0.75

    Sub New(render As Drawer)
        scan_x = render.dimension.Width
        scan_y = render.dimension.Height
        background = Color.White

        If TypeOf render.pixelReader Is ReadIbd Then
            UUID = DirectCast(render.pixelReader, ReadIbd).UUID
            fileSize = DirectCast(render.pixelReader, ReadIbd) _
                .ibd _
                .size _
                .DoCall(AddressOf StringFormats.Lanudry)
        End If
    End Sub

    Sub New()
    End Sub

    Sub New(info As Dictionary(Of String, String))
        scan_x = Integer.Parse(info!scan_x)
        scan_y = Integer.Parse(info!scan_y)
        UUID = info!uuid
        fileSize = info!fileSize
    End Sub

    Public Shared Function GetMSIInfo(render As Drawer) As Dictionary(Of String, String)
        Dim uuid As String = ""
        Dim fileSize As String = ""

        If TypeOf render.pixelReader Is ReadIbd Then
            uuid = DirectCast(render.pixelReader, ReadIbd).UUID
            fileSize = DirectCast(render.pixelReader, ReadIbd) _
                .ibd _
                .size _
                .DoCall(AddressOf StringFormats.Lanudry)
        End If

        Return New Dictionary(Of String, String) From {
            {NameOf(scan_x), render.dimension.Width},
            {NameOf(scan_y), render.dimension.Height},
            {NameOf(uuid), uuid},
            {NameOf(fileSize), fileSize}
        }
    End Function

    Public Sub Reset(MsiDim As Size, UUID As String, fileSize As String)
        _scan_x = MsiDim.Width
        _scan_y = MsiDim.Height
        _background = Color.White
        _UUID = UUID
        _fileSize = fileSize
    End Sub

    Public Function Smooth(img As Bitmap) As Bitmap
        If imageSmooth = SmoothFilters.None Then
            Return img
        End If

        Select Case imageSmooth
            Case SmoothFilters.Gauss : Return GaussBlur.GaussBlur(img)
            Case SmoothFilters.GaussMax : Return New Filters.Matrix(GaussBlur.GaussBlur(img)).GetSmoothBitmap(Matrix2DFilters.Max)
            Case SmoothFilters.GaussMin : Return New Filters.Matrix(GaussBlur.GaussBlur(img)).GetSmoothBitmap(Matrix2DFilters.Min)
            Case SmoothFilters.GaussMean : Return New Filters.Matrix(GaussBlur.GaussBlur(img)).GetSmoothBitmap(Matrix2DFilters.Mean)
            Case SmoothFilters.GaussMedian : Return New Filters.Matrix(GaussBlur.GaussBlur(img)).GetSmoothBitmap(Matrix2DFilters.Median)
            Case SmoothFilters.Max : Return New Filters.Matrix(img).GetSmoothBitmap(Matrix2DFilters.Max)
            Case SmoothFilters.Min : Return New Filters.Matrix(img).GetSmoothBitmap(Matrix2DFilters.Min)
            Case SmoothFilters.Mean : Return New Filters.Matrix(img).GetSmoothBitmap(Matrix2DFilters.Mean)
            Case SmoothFilters.Median : Return New Filters.Matrix(img).GetSmoothBitmap(Matrix2DFilters.Median)
            Case Else
                Throw New NotImplementedException
        End Select
    End Function

    Public Sub SetIntensityMax(max As Double)
        _min = 0
        _max = max

        lowerbound = 0
        upperbound = max * maxCut
    End Sub

    Public Function GetTolerance() As Tolerance
        If method = ToleranceMethod.Da Then
            Return Ms1.Tolerance.DeltaMass(tolerance)
        Else
            Return Ms1.Tolerance.PPM(tolerance)
        End If
    End Function
End Class
