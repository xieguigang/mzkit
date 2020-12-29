Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace mzData.mzWebCache

    Public Class ScanMS2 : Inherits MSScan

        Public Property parentMz As Double
        Public Property intensity As Double
        Public Property polarity As Integer

    End Class

    Public Class ScanMS1 : Inherits MSScan

        Public Property TIC As Double
        Public Property BPC As Double
        Public Property products As ScanMS2()

    End Class

    Public Class MSScan

        Public Property rt As Integer
        Public Property scan_id As String
        Public Property mz As Double()
        Public Property into As Double()

        Public Overrides Function ToString() As String
            Return scan_id
        End Function

        Public Iterator Function GetMs() As IEnumerable(Of ms2)
            For i As Integer = 0 To mz.Length - 1
                Yield New ms2 With {
                    .mz = mz(i),
                    .intensity = into(i),
                    .quantity = .intensity
                }
            Next
        End Function
    End Class
End Namespace