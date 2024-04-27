#Region "Microsoft.VisualBasic::d0ada6fdef3178d6b2be9b08d8a6c662, G:/mzkit/src/metadb/MoNA//SpectraInfo.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 66
    '    Code Lines: 51
    ' Comment Lines: 6
    '   Blank Lines: 9
    '     File Size: 2.63 KB


    ' Class SpectraInfo
    ' 
    '     Properties: collision_energy, column, flow_gradient, flow_rate, fragmentation_mode
    '                 instrument, instrument_type, ion_mode, ionization, MassPeaks
    '                 MsLevel, mz, precursor_type, resolution, retention_time
    '                 solvent_a, solvent_b
    ' 
    '     Function: ToPeaksMs2
    ' 
    ' /********************************************************************************/

#End Region

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
