Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.csv.IO

Public Module MoleculeNetworking

    <Extension>
    Public Iterator Function CreateMatrix(ms2 As PeakMs2(),
                                          cutoff As Double,
                                          tolerance As Tolerance,
                                          progressCallback As Action(Of String)) As IEnumerable(Of DataSet)
        For Each scan In ms2
            Dim scores = ms2 _
                .Where(Function(a) Not a Is scan) _
                .AsParallel _
                .Select(Function(a)
                            Dim id As String = a.lib_guid
                            Dim score = GlobalAlignment.TwoDirectionSSM(scan.mzInto, a.mzInto, tolerance)

                            Return (id, System.Math.Min(score.forward, score.reverse))
                        End Function) _
                .Where(Function(a) a.Item2 >= cutoff) _
                .ToArray

            Call progressCallback($"{scan.ToString} has {scores} homologous spectrum")

            Yield New DataSet With {
                .ID = scan.lib_guid,
                .Properties = scores.ToDictionary(Function(a) a.id, Function(a) a.Item2)
            }
        Next
    End Function
End Module
