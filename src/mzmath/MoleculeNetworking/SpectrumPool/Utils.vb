Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq

Namespace PoolData

    Public Class Utils

        Public Const unknown As String = NameOf(unknown)

        Public Shared Function ConservedGuid(spectral As PeakMs2) As String
            Dim desc = spectral.mzInto.OrderByDescending(Function(mzi) mzi.intensity).ToArray
            Dim peaks As String() = desc _
                .Select(Function(m) m.mz.ToString("F4") & ":" & m.intensity) _
                .ToArray
            Dim mz1 As String = spectral.mz.ToString("F4")
            Dim rt As String = spectral.rt.ToString("F2")
            Dim meta As String() = {
                spectral.meta.TryGetValue("biosample", unknown),
                spectral.meta.TryGetValue("organism", unknown),
                spectral.precursor_type
            }
            Dim hashcode As String = peaks _
                .JoinIterates(mz1) _
                .JoinIterates(rt) _
                .JoinIterates(meta) _
                .JoinBy(spectral.mzInto.Length) _
                .MD5

            Return hashcode
        End Function
    End Class
End Namespace