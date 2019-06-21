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
        Public Function WriteAsciiMgf(ion As Ions, out As StreamWriter) As Boolean

        End Function
    End Module
End Namespace