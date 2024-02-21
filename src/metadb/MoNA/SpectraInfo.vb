Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra


''' <summary>
''' The reference spectra data which is parsed from the MoNA database
''' </summary>
''' <remarks>
''' is a collection of the mass spectrum <see cref="ms2"/> data.
''' </remarks>
Public Class SpectraInfo

    Public Property MsLevel As String
    Public Property mz As Double
    Public Property precursor_type As String
    Public Property instrument_type As String
    Public Property instrument As String
    Public Property collision_energy As String
    Public Property ion_mode As String
    Public Property ionization As String
    Public Property fragmentation_mode As String
    Public Property resolution As String
    Public Property column As String
    Public Property flow_gradient As String
    Public Property flow_rate As String
    Public Property retention_time As String
    Public Property solvent_a As String
    Public Property solvent_b As String

    Public Property MassPeaks As ms2()

    Public Function ToPeaksMs2(Optional id As String = Nothing) As PeakMs2
        Dim precursor_type As String = Me.precursor_type

        If precursor_type.StringEmpty Then
            Dim ionMode As Integer = Provider.ParseIonMode(ion_mode, allowsUnknown:=True)

            If ionMode <> 0 Then
                precursor_type = $"[M]{If(ionMode > 0, "+", "-")}"
            End If
        End If
        If (Not precursor_type.StringEmpty) AndAlso (precursor_type.Last <> "+" AndAlso precursor_type.Last <> "-") Then
            Dim ionMode As Integer = Provider.ParseIonMode(ion_mode, allowsUnknown:=True)

            If ionMode <> 0 Then
                precursor_type = precursor_type.Trim("["c, "]"c)
                precursor_type = $"[{precursor_type}]{If(ionMode > 0, "+", "-")}"
            End If
        End If

        Return New PeakMs2 With {
            .activation = ionization,
            .collisionEnergy = Val(collision_energy.Match("\d+(\.\d+)?")),
            .intensity = MassPeaks.Sum(Function(a) a.intensity),
            .lib_guid = If(id, $"M{mz.ToString("F0")}T{retention_time}, m/z={mz} {precursor_type}"),
            .mz = mz,
            .mzInto = MassPeaks,
            .precursor_type = precursor_type,
            .rt = Val(retention_time),
            .meta = New Dictionary(Of String, String) From {
                {NameOf(instrument), instrument},
                {NameOf(fragmentation_mode), fragmentation_mode}
            }
        }
    End Function
End Class
