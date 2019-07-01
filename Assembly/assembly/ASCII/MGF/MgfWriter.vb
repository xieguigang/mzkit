Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.MassSpectrum.Math.Spectra

Namespace ASCII.MGF

    Public Module MgfWriter

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function MgfIon(matrix As LibraryMatrix, Optional precursor As ms2 = Nothing) As Ions
            If precursor Is Nothing Then
                precursor = New ms2 With {
                    .mz = matrix.ms2.Max(Function(m) m.mz),
                    .intensity = 1,
                    .quantity = 1
                }
            End If

            Return New Ions With {
                .Peaks = matrix.ms2,
                .Title = matrix.Name,
                .Charge = 1,
                .PepMass = New NamedValue With {
                    .name = precursor.mz,
                    .text = precursor.quantity
                }
            }
        End Function

        <Extension>
        Private Function ionTitle(ion As Ions) As String
            If ion.Meta.IsNullOrEmpty Then
                Return ion.Title
            Else
                Return $"{ion.Title} {ion.Meta.Select(Function(m) $"{m.Key}:""{m.Value}""").JoinBy(" ")}"
            End If
        End Function

        <Extension>
        Public Sub WriteAsciiMgf(ion As Ions, out As StreamWriter)
            Call out.WriteLine("BEGIN IONS")
            Call out.WriteLine("TITLE=" & ion.ionTitle)
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