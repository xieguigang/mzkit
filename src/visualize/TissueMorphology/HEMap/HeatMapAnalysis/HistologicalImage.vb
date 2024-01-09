#Region "Microsoft.VisualBasic::479f83b9145237467fdb738fcca6ccf2, mzkit\src\visualize\TissueMorphology\HEMap\HistologicalImage.vb"

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

'   Total Lines: 108
'    Code Lines: 76
' Comment Lines: 19
'   Blank Lines: 13
'     File Size: 4.12 KB


' Module HistologicalImage
' 
'     Function: GridScan, HeatMap, MonoScale
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq

Namespace HEMap

    ''' <summary>
    ''' helper module for processing haematoxylin and eosin staining image
    ''' </summary>
    Public Module HistologicalImage

        ''' <summary>
        ''' convert to gray scale
        ''' </summary>
        ''' <param name="HE"></param>
        ''' <returns></returns>
        <Extension>
        Public Function MonoScale(HE As Image) As Image
            Return HE.Grayscale
        End Function

        <Extension>
        Public Function HeatMap(HE As Image,
                                Optional scale As ScalerPalette = ScalerPalette.turbo,
                                Optional mapLevels As Integer = 64) As GraphicsData

            Return HE.Image2DMap(
                scaleName:=scale.Description,
                mapLevels:=mapLevels
            )
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="target"></param>
        ''' <param name="colors"></param>
        ''' <param name="gridSize"></param>
        ''' <param name="tolerance">
        ''' the color channel tolerance, [0, 255]
        ''' </param>
        ''' <param name="densityGrid"></param>
        ''' <returns></returns>
        Public Iterator Function GridScan(target As Image,
                                          Optional colors As String() = Nothing,
                                          Optional gridSize As Integer = 25,
                                          Optional tolerance As Integer = 15,
                                          Optional densityGrid As Integer = 5) As IEnumerable(Of Cell)

            Dim A As Double = gridSize ^ 2
            Dim sx, sy As Integer
            Dim colorData As New Dictionary(Of String, Color)

            If Not colors Is Nothing Then
                For Each cl As String In colors
                    Call colorData.Add(cl, cl.TranslateColor)
                Next
            End If

            Using bitmap As BitmapBuffer = BitmapBuffer.FromImage(target)
                For i As Integer = 1 To bitmap.Width Step gridSize
                    For j As Integer = 1 To bitmap.Height Step gridSize
                        Dim block As Color()() = bitmap _
                            .GetPixel(New Rectangle(i, j, gridSize - 1, gridSize - 1)) _
                            .ToArray
                        Dim cell As Cell = block.CellEvaluation(
                            sx, sy,
                            i, j,
                            gridSize, tolerance, densityGrid,
                            colors, colorData
                        )

                        Yield cell
                    Next

                    sy = 0
                    sx += 1
                Next
            End Using
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="block">the current block data</param>
        ''' <param name="sx"></param>
        ''' <param name="sy"></param>
        ''' <param name="i">image physical x</param>
        ''' <param name="j">image physical y</param>
        ''' <param name="gridSize"></param>
        ''' <param name="tolerance"></param>
        ''' <param name="densityGrid"></param>
        ''' <param name="colors"></param>
        ''' <param name="colorData"></param>
        ''' <returns></returns>
        <Extension>
        Private Function CellEvaluation(block As Color()(),
                                        ByRef sx As Integer,
                                        ByRef sy As Integer,
                                        i As Integer,
                                        j As Integer,
                                        gridSize As Integer,
                                        tolerance As Integer,
                                        densityGrid As Integer,
                                        colors As String(),
                                        colorData As Dictionary(Of String, Color)) As Cell

            Dim r = block.IteratesALL.Select(Function(c) CDbl(c.R)).Average
            Dim g = block.IteratesALL.Select(Function(c) CDbl(c.G)).Average
            Dim b = block.IteratesALL.Select(Function(c) CDbl(c.B)).Average
            Dim matrix As Grid(Of Color) = block _
                .Select(Function(row, y)
                            Return row.Select(Function(c, x) (c, x, y))
                        End Function) _
                .IteratesALL _
                .DoCall(AddressOf Grid(Of Color).Create)
            Dim black As [Object] = [Object].Eval(matrix, Color.Black, gridSize, tolerance, densityGrid)
            Dim cell As New Cell With {
                .X = i,
                .Y = j,
                .B = b,
                .G = g,
                .R = r,
                .Black = black,
                .ScaleX = sx,
                .ScaleY = sy
            }

            ' evaluate the color channels
            If Not colors Is Nothing Then
                For Each cl As String In colors
                    cell.layers(cl) = [Object].Eval(matrix, colorData(cl), gridSize, tolerance, densityGrid)
                Next
            End If

            sy += 1

            Return cell
        End Function

        <Extension>
        Public Iterator Function ScanColor(image As Image, targets As Color(), Optional tolerance As Double = 15) As IEnumerable(Of (target As Color, pos As Point))
            Using bitmap As BitmapBuffer = BitmapBuffer.FromImage(image)
                For i As Integer = 1 To bitmap.Width - 1
                    For j As Integer = 1 To bitmap.Height - 1
                        Dim pixel As Color = bitmap.GetPixel(i, j)
                        Dim xy As New Point(i, j)

                        For Each channel As Color In targets
                            If channel.Equals(pixel, tolerance:=tolerance) Then
                                Yield (channel, xy)
                            End If
                        Next
                    Next
                Next
            End Using
        End Function
    End Module
End Namespace