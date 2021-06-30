Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq

Module Extensions

    <Extension>
    Public Function GetAllCentroidScanMs1(MS As ScanMS1(), centroid As Tolerance) As IEnumerable(Of ms1_scan)
        Return MS _
            .Select(Function(scan)
                        Dim MSproducts As ms2() = scan.GetMs _
                            .ToArray _
                            .Centroid(centroid, LowAbundanceTrimming.intoCutff) _
                            .ToArray

                        Return MSproducts _
                            .Select(Function(mzi)
                                        Return New ms1_scan With {
                                            .mz = mzi.mz,
                                            .intensity = mzi.intensity,
                                            .scan_time = scan.rt
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL
    End Function
End Module
