Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports stdNum = System.Math

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
                .FileName = path,
                .title = path.BaseName,
                .attributes = New Dictionary(Of String, String),
                .TIC = TIC.Select(Function(tick) tick.Intensity).ToArray,
                .times = TIC.Select(Function(tick) tick.Time).ToArray,
                .MS = chromatogramList _
                    .Where(Function(c) c.id <> "TIC") _
                    .Select(AddressOf readMs) _
                    .ToArray _
                    .TimelineTranspose(.times)
            }
        End Function

        <Extension>
        Private Function TimelineTranspose(mzScans As ms1_scan()(), times As Double()) As ms1_scan()()
            Dim time_scans As New List(Of ms1_scan())
            Dim mz_scanList = mzScans.Select(Function(mz) mz.ToList).ToArray
            Dim mzList As Double() = mzScans.Select(Function(mzi) mzi(Scan0).mz).ToArray
            Dim checkScan = Iterator Function(time As Double) As IEnumerable(Of ms1_scan)
                                For i As Integer = 0 To mz_scanList.Length - 1
                                    Dim list = mz_scanList(i)
                                    Dim tick As ms1_scan = list _
                                        .Where(Function(t) stdNum.Abs(t.scan_time - time) <= 0.5) _
                                        .FirstOrDefault

                                    If tick Is Nothing Then
                                        Yield New ms1_scan With {
                                            .intensity = 0,
                                            .mz = mzList(i),
                                            .scan_time = time
                                        }
                                    Else
                                        list.Remove(tick)
                                        Yield tick
                                    End If
                                Next
                            End Function

            For Each time As Double In times
                time_scans.Add(checkScan(time).ToArray)
            Next

            Return time_scans.ToArray
        End Function

        Private Function readMs(chromatogram As MarkupData.mzML.chromatogram) As ms1_scan()
            Dim mz As Double = chromatogram _
                .precursor _
                .isolationWindow _
                .cvParams _
                .Where(Function(par) par.name = "isolation window target m/z") _
                .First _
                .GetDouble
            Dim ticks As ChromatogramTick() = chromatogram.Ticks
            Dim ms1 As ms1_scan() = ticks _
                .Select(Function(tick)
                            Return New ms1_scan With {
                                .intensity = tick.Intensity,
                                .mz = mz,
                                .scan_time = tick.Time
                            }
                        End Function) _
                .ToArray

            Return ms1
        End Function
    End Module
End Namespace