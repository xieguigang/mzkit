
Imports Microsoft.VisualBasic.Math.SignalProcessing

Namespace MarkupData.mzML

    Public Class UVScan

        Public Property wavelength As Double()
        Public Property intensity As Double()
        Public Property total_ion_current As Double
        Public Property scan_time As Double

        Public Overrides Function ToString() As String
            Return $"total_ions:{total_ion_current.ToString("G3")} at {CInt(scan_time)} sec"
        End Function

        Public Function GetSignalModel() As GeneralSignal
            Return New GeneralSignal With {
                .description = ToString(),
                .Measures = wavelength,
                .measureUnit = "wavelength",
                .reference = ToString(),
                .Strength = intensity,
                .meta = New Dictionary(Of String, String) From {
                    {"title", .reference}
                }
            }
        End Function

    End Class
End Namespace