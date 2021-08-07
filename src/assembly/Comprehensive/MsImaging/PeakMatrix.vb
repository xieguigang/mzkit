Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq

Namespace MsImaging

    Public Module PeakMatrix

        <Extension>
        Public Iterator Function TopIonsPeakMatrix(raw As mzPack, Optional topN As Integer = 3, Optional tolerance As String = "da:0.05") As IEnumerable(Of DataSet)
            Dim mzErr As Tolerance = Ms1.Tolerance.ParseScript(tolerance)
            Dim topPeaks = raw.MS _
                .AsParallel _
                .Select(Function(scan)
                            Dim pid As String = $"{scan.meta!x},{scan.meta!y}"
                            Dim topIons As ms2() = scan _
                                .GetMs _
                                .ToArray _
                                .Centroid(mzErr, New RelativeIntensityCutoff(0)) _
                                .OrderByDescending(Function(i) i.intensity) _
                                .Take(topN) _
                                .ToArray

                            Return (pid, topIons)
                        End Function) _
                .ToArray
            Dim allMz As Double() = topPeaks _
                .Select(Function(i) i.topIons) _
                .IteratesALL _
                .ToArray _
                .Centroid(mzErr, New RelativeIntensityCutoff(0)) _
                .Select(Function(i) i.mz) _
                .OrderBy(Function(mz) mz) _
                .ToArray

            For Each pixel In topPeaks
                Yield New DataSet With {
                    .ID = pixel.pid,
                    .Properties = allMz _
                        .ToDictionary(Function(mz) mz.ToString,
                                      Function(mz)
                                          Dim matchIon As ms2 = pixel.topIons _
                                              .Where(Function(i) mzErr(i.mz, mz)) _
                                              .FirstOrDefault

                                          If matchIon Is Nothing Then
                                              Return 0
                                          Else
                                              Return matchIon.intensity
                                          End If
                                      End Function)
                }
            Next
        End Function

    End Module
End Namespace