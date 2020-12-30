Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace mzData.mzWebCache

    Public Class MSScan

        Public Overridable Property rt As Double
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


