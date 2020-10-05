Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace Spectra

    <HideModuleName>
    Public Module MzGrouping

        <Extension>
        Public Function Unique(ions As IEnumerable(Of PeakMs2),
                                        Optional eq# = 0.85,
                                        Optional gt# = 0.6,
                                        Optional mzwidth$ = "da:0.1",
                                        Optional tolerance$ = "da:0.3",
                                        Optional precursor$ = "ppm:20",
                                        Optional rtwidth# = 5,
                                        Optional trim$ = "0.05") As IEnumerable(Of PeakMs2)

            ' group by peak clustering at first
            Dim comparision = SpectrumTreeCluster.SSMCompares(Ms1.Tolerance.ParseScript(tolerance), eq, gt)
            Dim centroidErr As Tolerance = Ms1.Tolerance.ParseScript(mzwidth)
            Dim intocutoff As LowAbundanceTrimming = LowAbundanceTrimming.ParseScript(trim)
            Dim tree As New SpectrumTreeCluster(
                compares:=comparision,
                mzwidth:=centroidErr,
                intocutoff:=intocutoff,
                showReport:=False
            )

            Call ions.ToArray.DoCall(AddressOf tree.doCluster)

            Return tree _
                .PopulateClusters _
                .PopulateUniquePeakMatrix(precursor, rtwidth, mzwidth, trim)
        End Function

        <Extension>
        Public Iterator Function PopulateUniquePeakMatrix(clusters As IEnumerable(Of SpectrumCluster),
                                                          Optional precursor$ = "ppm:20",
                                                          Optional rtwidth# = 5,
                                                          Optional mzwidth$ = "da:0.1",
                                                          Optional trim$ = "0.05") As IEnumerable(Of PeakMs2)

            Dim parentErr As Tolerance = Ms1.Tolerance.ParseScript(precursor)
            Dim centroidErr As Tolerance = Ms1.Tolerance.ParseScript(mzwidth)
            Dim intocutoff As LowAbundanceTrimming = LowAbundanceTrimming.ParseScript(trim)

            For Each cluster As SpectrumCluster In clusters
                ' group by mz/rt
                ' by mz
                Dim mzgroups = cluster.cluster.GroupBy(Function(a) a.mz, parentErr)

                ' by rt
                For Each mz As NamedCollection(Of PeakMs2) In mzgroups
                    Dim rtgroups = mz.GroupBy(Function(a) a.rt, offsets:=rtwidth)
                    Dim mzval As Double = Aggregate ion In mz Into Average(ion.mz)

                    For Each rt As NamedCollection(Of PeakMs2) In rtgroups
                        Dim members As String() = rt.Select(Function(a) a.lib_guid).ToArray
                        Dim rtval As Double = Aggregate ion In rt Into Average(ion.rt)
                        Dim peaks As ms2() = rt _
                            .Select(Function(a) a.mzInto) _
                            .IteratesALL _
                            .ToArray _
                            .Centroid(centroidErr, intocutoff) _
                            .ToArray

                        Yield New PeakMs2 With {
                            .lib_guid = members.JoinBy(", "),
                            .rt = rtval,
                            .mz = mzval,
                            .mzInto = peaks
                        }
                    Next
                Next
            Next
        End Function
    End Module
End Namespace