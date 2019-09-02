Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.MassSpectrum.Math.Spectra

Namespace ASCII.MGF

    Public Module MgfWriter

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function MgfIon(matrix As PeakMs2) As Ions
            Return New Ions With {
                .Charge = 1,
                .Peaks = matrix.mzInto.Array,
                .PepMass = New NamedValue With {
                    .name = matrix.mz,
                    .text = matrix.Ms2Intensity
                },
                .RtInSeconds = matrix.rt,
                .Title = $"{matrix.file}#{matrix.scan}",
                .Meta = New Dictionary(Of String, String) From {
                    {"rawfile", matrix.file},
                    {"collisionEnergy", matrix.collisionEnergy},
                    {"activation", matrix.activation},
                    {"scan", matrix.scan}
                },
                .Rawfile = matrix.file,
                .Accession = $"{matrix.file}#{matrix.scan}"
            }
        End Function

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
                Return $"{ion.Title} {ion.Meta.Select(Function(m) $"{m.Key}:""{m.Value}""").JoinBy(", ")}"
            End If
        End Function

        <Extension>
        Private Sub writeIf(out As StreamWriter, key$, value$)
            If Not value.StringEmpty Then
                Call out.WriteLine($"{key}={value}")
            End If
        End Sub

        <Extension>
        Public Sub WriteAsciiMgf(ion As Ions, out As StreamWriter)
            Call out.WriteLine("BEGIN IONS")
            Call out.WriteLine("TITLE=" & ion.ionTitle)
            Call out.WriteLine("RTINSECONDS=" & ion.RtInSeconds)
            Call out.WriteLine($"PEPMASS={ion.PepMass.name} {ion.PepMass.text}")
            Call out.WriteLine("CHARGE=" & ion.Charge)

            ' Optional
            Call out.writeIf("ACCESSION", ion.Accession)
            Call out.writeIf("INSTRUMENT", ion.Instrument)
            Call out.writeIf("RAWFILE", ion.Rawfile)
            Call out.writeIf("DB", ion.Database)
            Call out.writeIf("SEQ", ion.Sequence)
            Call out.writeIf("LOCUS", ion.Locus)

            For Each fragment As ms2 In ion.Peaks
                Call out.WriteLine($"{fragment.mz} {fragment.intensity}")
            Next

            Call out.WriteLine("END IONS")
        End Sub
    End Module
End Namespace