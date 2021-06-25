Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq

Namespace MarkupData.imzML

    Public Class MSISummary

        Public Property rowScans As iPixelIntensity()()
        Public Property size As Size

        Public Function GetBasePeakMz() As LibraryMatrix
            Return New LibraryMatrix With {
                .centroid = False,
                .name = "basePeak M/z",
                .ms2 = rowScans _
                    .IteratesALL _
                    .Select(Function(p)
                                Return New ms2 With {
                                    .mz = p.basePeakMz,
                                    .intensity = p.basePeakIntensity,
                                    .Annotation = $"{p.x},{p.y}"
                                }
                            End Function) _
                    .ToArray
            }
        End Function

        Private Iterator Function GetRowScan(row As iPixelIntensity(), summary As IntensitySummary) As IEnumerable(Of PixelScanIntensity)
            For Each p As iPixelIntensity In row
                Dim into As Double

                Select Case summary
                    Case IntensitySummary.Average : into = p.average
                    Case IntensitySummary.BasePeak : into = p.basePeakIntensity
                    Case IntensitySummary.Total : into = p.totalIon
                    Case Else
                        Throw New NotImplementedException
                End Select

                Yield New PixelScanIntensity With {
                    .totalIon = into,
                    .x = p.x,
                    .y = p.y
                }
            Next
        End Function

        Public Function GetLayer(summary As IntensitySummary) As IEnumerable(Of PixelScanIntensity)
            Return rowScans _
                .Select(Function(row)
                            Return GetRowScan(row, summary)
                        End Function) _
                .IteratesALL
        End Function

        Public Overrides Function ToString() As String
            Return $"[width:={size.Width}, height:={size.Height}]"
        End Function

    End Class

    Public Enum IntensitySummary
        Total
        BasePeak
        Average
    End Enum

End Namespace