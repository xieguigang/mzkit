Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Module MoleculeNetworking

    <Extension>
    Public Iterator Function CreateMatrix(ms2 As PeakMs2(),
                                          cutoff As Double,
                                          tolerance As Tolerance,
                                          progressCallback As Action(Of String)) As IEnumerable(Of DataSet)
        Dim i As i32 = 1

        For Each scan In ms2
            Dim scores = ms2 _
                .Where(Function(a) Not a Is scan) _
                .AsParallel _
                .Select(Function(a)
                            Dim id As String = a.lib_guid
                            Dim score = GlobalAlignment.TwoDirectionSSM(scan.mzInto, a.mzInto, tolerance)

                            Return (id, System.Math.Min(score.forward, score.reverse))
                        End Function) _
                .ToArray

            Call progressCallback($"[{++i}/{ms2.Length}] {scan.ToString} has {scores.Where(Function(a) a.Item2 >= cutoff).Count} homologous spectrum")

            Yield New DataSet With {
                .ID = scan.lib_guid,
                .Properties = scores.ToDictionary(Function(a) a.id, Function(a) a.Item2)
            }
        Next
    End Function

    <Extension>
    Public Function GetSpectrum(raw As Raw, scanId As String, Optional ByRef properties As SpectrumProperty = Nothing) As LibraryMatrix
        Using cache As New netCDFReader(raw.cache)
            Dim data As CDFData = cache.getDataVariable(cache.getDataVariableEntry(scanId))
            Dim attrs = cache.getDataVariableEntry(scanId).attributes
            Dim rawData As ms2() = data.numerics.AsMs2.ToArray
            Dim scanData As New LibraryMatrix With {
                .name = scanId,
                .centroid = False,
                .ms2 = rawData.Centroid(Tolerance.DeltaMass(0.1), 0.01).ToArray
            }

            properties = New SpectrumProperty(scanId, attrs)

            Return scanData
        End Using
    End Function
End Module
