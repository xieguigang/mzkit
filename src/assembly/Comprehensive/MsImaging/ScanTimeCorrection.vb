#Region "Microsoft.VisualBasic::4fdb1cdf96bbdb73c0b57fadebe1f764, assembly\Comprehensive\MsImaging\ScanTimeCorrection.vb"

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

    '   Total Lines: 56
    '    Code Lines: 29
    ' Comment Lines: 19
    '   Blank Lines: 8
    '     File Size: 2.00 KB


    '     Class ScanTimeCorrection
    ' 
    '         Properties: pixels, pixelsTime, totalTime
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetPixelPoint, GetPixelRow, GetPixelRowX
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Imaging.BitmapImage

Namespace MsImaging

    ''' <summary>
    ''' implements the x-axis encoder based on the scan time offsets
    ''' </summary>
    Public Class ScanTimeCorrection : Inherits Correction

        Public ReadOnly Property totalTime As Double
        ''' <summary>
        ''' pixels in row or total pixels by width times height
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property pixels As Integer
        Public ReadOnly Property pixelsTime As Double

        Sub New(totalTime As Double, pixels As Integer)
            Me.totalTime = totalTime
            Me.pixels = pixels
            Me.pixelsTime = totalTime / pixels
        End Sub

        ''' <summary>
        ''' if the raw data file is row scans
        ''' </summary>
        ''' <param name="rt"></param>
        ''' <returns>X of the point</returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetPixelRow(rt As Double) As Integer
            Return 1 + CInt(rt / pixelsTime)
        End Function

        ''' <summary>
        ''' if the raw data file is 2D scans
        ''' </summary>
        ''' <param name="rt"></param>
        ''' <returns>[x, y]</returns>
        Public Function GetPixelPoint(rt As Double) As Point
            ' 在这个二维扫描之中，已经有了n个像素点了
            Dim n As Integer = GetPixelRow(rt)
            Dim pt As Point = BitmapBuffer.ToPixel2D(n, width:=pixels, channels:=1)

            Return pt
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetPixelRowX(scanMs1 As ScanMS1) As Integer
            Return GetPixelRow(scanMs1.rt)
        End Function
    End Class
End Namespace
