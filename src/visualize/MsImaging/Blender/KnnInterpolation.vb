Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Math.Distributions

Namespace Blender

    Public Module KnnInterpolation

        <Extension>
        Private Function KnnInterpolation(graph As Grid(Of iPixelIntensity), x As Integer, y As Integer, deltaSize As Size, q As Double) As iPixelIntensity
            Dim query As iPixelIntensity() = graph.Query(x, y, deltaSize).ToArray

            If query.IsNullOrEmpty Then
                Return Nothing
            ElseIf (query.Length / (deltaSize.Width * deltaSize.Height)) <= q Then
                Return Nothing
            End If

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

        <Extension>
        Public Function KnnFill(summary As MSISummary, Optional dx As Integer = 10, Optional dy As Integer = 10, Optional q As Double = 0.65) As MSISummary
            Dim graph As Grid(Of iPixelIntensity) = Grid(Of iPixelIntensity).Create(summary.ToArray, Function(p) p.x, Function(p) p.y)
            Dim size As Size = summary.size
            Dim pixels As New List(Of iPixelIntensity)
            Dim point As iPixelIntensity
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
                        Call pixels.Add(point)
                    End If
                Next
            Next

            Return MSISummary.FromPixels(pixels)
        End Function

        <Extension>
        Public Function KnnFill(layer As SingleIonLayer, Optional dx As Integer = 10, Optional dy As Integer = 10, Optional q As Double = 0.65) As SingleIonLayer
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
            Dim query As PixelData() = graph.Query(x, y, deltaSize).ToArray

            If query.IsNullOrEmpty Then
                Return Nothing
            ElseIf (query.Length / (deltaSize.Width * deltaSize.Height)) <= q Then
                Return Nothing
            End If

            Dim intensity As Double() = query.Select(Function(p) p.intensity).TabulateBin
            Dim mean As Double = intensity.Average

            Return New PixelData With {
                .intensity = mean,
                .level = 0,
                .mz = 0,
                .x = x,
                .y = y
            }
        End Function

    End Module
End Namespace