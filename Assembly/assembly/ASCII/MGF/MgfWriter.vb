Imports System.IO
Imports System.Runtime.CompilerServices
Imports SMRUCC.MassSpectrum.Math.Spectra

Namespace ASCII.MGF

    Public Module MgfWriter

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function MgfIon(matrix As LibraryMatrix) As Ions
            Return New Ions With {
                .Peaks = matrix.ms2,
                .Title = matrix.Name
            }
        End Function

        <Extension>
        Public Sub WriteAsciiMgf(ion As Ions, out As StreamWriter)
            Call out.WriteLine("BEGIN IONS")
            Call out.WriteLine("TITLE=" & ion.Title)
            Call out.WriteLine("RTINSECONDS=" & ion.RtInSeconds)
            Call out.WriteLine($"PEPMASS={ion.PepMass.name} {ion.PepMass.text}")
            Call out.WriteLine("CHARGE=" & ion.Charge)

            For Each fragment As ms2 In ion.Peaks
                Call out.WriteLine($"{fragment.mz} {fragment.intensity}")
            Next

            Call out.WriteLine("END IONS")
        End Sub
    End Module
End Namespace