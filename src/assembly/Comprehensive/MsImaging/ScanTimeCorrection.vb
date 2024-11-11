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
'    Code Lines: 29 (51.79%)
' Comment Lines: 19 (33.93%)
'    - Xml Docs: 89.47%
' 
'   Blank Lines: 8 (14.29%)
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
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
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

        ''' <summary>
        ''' make bugs fixed for RT pixel correction
        ''' </summary>
        ''' <param name="MSI"></param>
        ''' <param name="println">
        ''' set this parameter value to nothing will mute the log message echo. 
        ''' </param>
        ''' <returns></returns>
        Public Shared Function ScanMeltdown(MSI As mzPack,
                                            Optional gridSize As Integer = 2,
                                            Optional println As Action(Of String) = Nothing) As mzPack

            Dim graph = Grid(Of ScanMS1).Create(MSI.MS, Function(d) d.GetMSIPixel)
            Dim scans As New List(Of ScanMS1)
            Dim dims As New Size(graph.width, graph.height)
            Dim pixel As ScanMS1

            If println Is Nothing Then
                println =
                    Sub()

                    End Sub
            Else
                Call println("make bugs fixed for RT pixel correction!")
            End If

            For i As Integer = 1 To dims.Width
                For j As Integer = 1 To dims.Height
                    pixel = graph.GetData(i, j)

                    If pixel Is Nothing Then
                        For xi As Integer = -1 To -gridSize Step -1
                            pixel = graph.GetData(i + xi, j)

                            If Not pixel Is Nothing Then
                                Exit For
                            End If
                        Next
                    End If

                    If Not pixel Is Nothing Then
                        scans.Add(pixel)
                    Else
                        Call println($"Missing pixel data at [{i}, {j}]!")
                    End If
                Next
            Next

            Return New mzPack With {
                .Application = FileApplicationClass.MSImaging,
                .Chromatogram = MSI.Chromatogram,
                .MS = scans.ToArray,
                .Scanners = MSI.Scanners,
                .source = MSI.source,
                .Thumbnail = MSI.Thumbnail,
                .Annotations = MSI.Annotations,
                .metadata = MSI.metadata
            }
        End Function
    End Class
End Namespace
