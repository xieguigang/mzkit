#Region "Microsoft.VisualBasic::f3f74c76a1357983451641a9e6adcf24, mzkit\src\visualize\MsImaging\Blender\KnnInterpolation.vb"

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

'   Total Lines: 205
'    Code Lines: 153
' Comment Lines: 18
'   Blank Lines: 34
'     File Size: 7.97 KB


'     Module KnnInterpolation
' 
'         Function: (+5 Overloads) KnnFill, (+2 Overloads) KnnInterpolation
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph

Namespace Blender

    Public Module KnnInterpolation

        <Extension>
        Private Function KnnInterpolation(Of T As PixelScanIntensity)(graph As Grid(Of T), x As Integer, y As Integer, deltaSize As Size, q As Double, aggregate As Func(Of Integer, Integer, T(), T)) As T
            Dim query As T() = graph.Query(x, y, deltaSize).ToArray

            If query.IsNullOrEmpty Then
                Return Nothing
            ElseIf (query.Length / (deltaSize.Width * deltaSize.Height)) <= q Then
                Return Nothing
            End If

            Return aggregate(x, y, query)
        End Function

        <Extension>
        Public Iterator Function KnnFill(Of T As {New, PixelScanIntensity})(summary As IEnumerable(Of T),
                                                                            Optional dx As Integer = 10,
                                                                            Optional dy As Integer = 10,
                                                                            Optional q As Double = 0.65,
                                                                            Optional aggregate As Func(Of Integer, Integer, T(), T) = Nothing) As IEnumerable(Of T)
            Dim allPixels = summary.ToArray
            Dim graph As Grid(Of T) = Grid(Of T).Create(allPixels, Function(p) p.x, Function(p) p.y)

            If allPixels.IsNullOrEmpty Then
                Return
            End If

            Dim size As New Size With {
                .Width = allPixels.Select(Function(i) i.x).Max,
                .Height = allPixels.Select(Function(i) i.y).Max
            }
            Dim point As T
            Dim deltaSize As New Size(dx, dy)

            If aggregate Is Nothing Then
                aggregate =
                    Function(x, y, query)
                        Return New T With {
                            .x = x,
                            .y = y,
                            .totalIon = Aggregate i In query Into Average(i.totalIon)
                        }
                    End Function
            End If

            For i As Integer = 1 To size.Width
                For j As Integer = 1 To size.Height
                    point = graph.GetData(i, j)

                    If point Is Nothing Then
                        point = graph.KnnInterpolation(i, j, deltaSize, q, aggregate)

                        'If Not point Is Nothing Then
                        '    Call graph.Add(point)
                        'End If
                    End If

                    If Not point Is Nothing Then
                        Yield point
                    End If
                Next
            Next
        End Function

        <Extension>
        Public Function KnnFill(summary As MSISummary, Optional dx As Integer = 10, Optional dy As Integer = 10, Optional q As Double = 0.65) As MSISummary
            Dim aggregate As Func(Of Integer, Integer, iPixelIntensity(), iPixelIntensity) =
                Function(x, y, query)
                    Dim total = query.Select(Function(p) p.totalIon).Min
                    Dim baseIntensity = query.Select(Function(p) p.basePeakIntensity).Min
                    Dim average = query.Select(Function(p) p.average).Min

                    Return New iPixelIntensity With {
                        .basePeakIntensity = baseIntensity,
                        .average = average,
                        .basePeakMz = 0,
                        .totalIon = total,
                        .x = x,
                        .y = y
                    }
                End Function
            Dim pixels As iPixelIntensity() = summary _
                .ToArray _
                .KnnFill(dx, dy, q, aggregate) _
                .ToArray

            Return MSISummary.FromPixels(pixels, dims:=summary.size)
        End Function

        <Extension>
        Public Function KnnFill(layer As SingleIonLayer,
                                Optional dx As Integer = 10,
                                Optional dy As Integer = 10,
                                Optional q As Double = 0.65) As SingleIonLayer

            Dim size As Size = layer.DimensionSize
            Dim pixels As PixelData() = KnnFill(layer.MSILayer, size, dx, dy, q)

            Return New SingleIonLayer With {
                .DimensionSize = layer.DimensionSize,
                .IonMz = layer.IonMz,
                .MSILayer = pixels
            }
        End Function

        <Extension>
        Public Function KnnFill(layer As SingleIonLayer, Optional resolution As Integer = 10) As SingleIonLayer
            Dim size As Size = layer.DimensionSize
            Dim dx As Integer = size.Width / resolution
            Dim dy As Integer = size.Height / resolution

            Return layer.KnnFill(dx, dy)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pixels"></param>
        ''' <param name="size">
        ''' the size of the original MS-imaging raw data file
        ''' </param>
        ''' <param name="dx"></param>
        ''' <param name="dy"></param>
        ''' <param name="q"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function KnnFill(pixels As PixelData(), size As Size,
                                Optional dx As Integer = 3,
                                Optional dy As Integer = 3,
                                Optional q As Double = 0.65) As PixelData()

            Dim graph As Grid(Of PixelData) = Grid(Of PixelData).Create(pixels)
            Dim outPixels As New List(Of PixelData)
            Dim point As PixelData
            Dim deltaSize As New Size(dx, dy)

            For i As Integer = 1 To size.Width
                For j As Integer = 1 To size.Height
                    point = graph.GetData(i, j)

                    If point Is Nothing Then
                        point = graph.KnnInterpolation(i, j, deltaSize, q)

                        'If Not point Is Nothing Then
                        '    Call graph.Add(point)
                        'End If
                    End If

                    If Not point Is Nothing Then
                        Call outPixels.Add(point)
                    End If
                Next
            Next

            Return outPixels.ToArray
        End Function

        <Extension>
        Private Function KnnInterpolation(graph As Grid(Of PixelData), x As Integer, y As Integer, deltaSize As Size, q As Double) As PixelData
            ' get non-empty pixels in current region block
            Dim query As PixelData() = graph.Query(x, y, deltaSize).ToArray
            Dim A As Double = deltaSize.Width * deltaSize.Height

            If query.IsNullOrEmpty Then
                Return Nothing
            ElseIf (query.Length / A) <= q Then
                Return Nothing
            End If

            Dim intensity As Double() = (From p As PixelData
                                         In query
                                         Where p.intensity > 0
                                         Let into As Double = p.intensity
                                         Select into).ToArray
            Dim mean As Double

            If intensity.Length = 0 Then
                Return Nothing
            Else
                mean = intensity.Min
            End If

            If mean = 0.0 Then
                Return Nothing
            Else
                Return New PixelData With {
                    .intensity = mean,
                    .level = 0,
                    .mz = 0,
                    .x = x,
                    .y = y
                }
            End If
        End Function

    End Module
End Namespace
