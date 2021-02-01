Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram

Namespace GCMS

    Public Module mzMLReader

        Public Function LoadFile(path As String) As Raw
            Dim chromatogramList = mzML _
                .LoadChromatogramList(path) _
                .ToArray
            Dim TIC As ChromatogramTick() = chromatogramList _
                .Where(Function(c) c.id = "TIC") _
                .First _
                .Ticks _
                .OrderBy(Function(tick) tick.Time) _
                .ToArray

            Return New Raw With {
                .fileName = path,
                .title = path.BaseName,
                .attributes = New Dictionary(Of String, String),
                .tic = TIC.Select(Function(tick) tick.Intensity).ToArray,
                .times = TIC.Select(Function(tick) tick.Time).ToArray,
                .ms = chromatogramList _
                    .Where(Function(c) c.id <> "TIC") _
                    .Select(AddressOf readMs) _
                    .ToArray
            }
        End Function

        Private Function readMs(chromatogram As MarkupData.mzML.chromatogram) As ms1_scan()

        End Function
    End Module
End Namespace