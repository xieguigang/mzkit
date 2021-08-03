Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Expressions

Namespace MsImaging

    ''' <summary>
    ''' raw data file reader helper code
    ''' </summary>
    Public Module MsImagingRaw

        <Extension>
        Public Function MSICombineRowScans(src As IEnumerable(Of mzPack),
                                           correction As Correction,
                                           Optional intocutoff As Double = 0.05,
                                           Optional progress As RunSlavePipeline.SetMessageEventHandler = Nothing) As mzPack

            Dim pixels As New List(Of ScanMS1)
            Dim cutoff As New RelativeIntensityCutoff(intocutoff)

            If progress Is Nothing Then
                progress = Sub(msg)
                               ' do nothing
                           End Sub
            End If

            For Each row As mzPack In src
                Dim i As i32 = 1
                Dim y As Integer = row.source _
                    .Match("\d+") _
                    .DoCall(AddressOf Integer.Parse)

                Call progress($"load: {row.source}...")

                For Each scan As ScanMS1 In row.MS
                    Dim x As Integer = If(correction Is Nothing, ++i, correction.GetPixelRowX(scan))
                    Dim ms As ms2() = cutoff.Trim(scan.GetMs)
                    Dim mz As Double() = ms.Select(Function(m) m.mz).ToArray
                    Dim into As Double() = ms.Select(Function(m) m.intensity).ToArray

                    pixels += New ScanMS1 With {
                        .BPC = scan.BPC,
                        .into = into,
                        .mz = mz,
                        .meta = New Dictionary(Of String, String) From {{NameOf(x), x}, {NameOf(y), y}},
                        .rt = scan.rt,
                        .scan_id = $"[{row.source}] {scan.scan_id}",
                        .TIC = scan.TIC
                    }
                Next
            Next

            Return New mzPack With {.MS = pixels.ToArray}
        End Function
    End Module
End Namespace