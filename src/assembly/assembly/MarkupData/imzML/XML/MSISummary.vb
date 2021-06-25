Imports System.Drawing
Imports Microsoft.VisualBasic.Linq

Namespace MarkupData.imzML

    Public Class MSISummary

        Public Property rowScans As iPixelIntensity()()
        Public Property size As Size

        Public Function GetLayer(summary As IntensitySummary) As IEnumerable(Of PixelScanIntensity)
            Return rowScans _
                .Select(Function(row)
                            Return row _
                                .Select(Function(p)
                                            Dim into As Double

                                            Select Case summary
                                                Case IntensitySummary.Average : into = p.average
                                                Case IntensitySummary.BasePeak : into = p.basePeakIntensity
                                                Case IntensitySummary.Total : into = p.totalIon
                                                Case Else
                                                    Throw New NotImplementedException
                                            End Select

                                            Return New PixelScanIntensity With {
                                                .totalIon = into,
                                                .x = p.x,
                                                .y = p.y
                                            }
                                        End Function)
                        End Function) _
                .IteratesALL
        End Function

    End Class

    Public Enum IntensitySummary
        Total
        BasePeak
        Average
    End Enum

End Namespace