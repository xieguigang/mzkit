#Region "Microsoft.VisualBasic::98a18dd78e95e5c9a6b047836bd225f9, src\mzmath\ms2_math-core\Spectra\MzGrouping.vb"

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

    '     Module MzGrouping
    ' 
    '         Function: (+2 Overloads) PopulateUniquePeakMatrix, Unique
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
            Dim comparision = SpectrumTreeCluster.SSMCompares(Ms1.Tolerance.ParseScript(tolerance), Nothing, eq, gt)
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
        Public Function PopulateUniquePeakMatrix(clusters As IEnumerable(Of SpectrumCluster),
                                                 Optional precursor$ = "ppm:20",
                                                 Optional rtwidth# = 5,
                                                 Optional mzwidth$ = "da:0.1",
                                                 Optional trim$ = "0.05") As IEnumerable(Of PeakMs2)

            Dim parentErr As Tolerance = Ms1.Tolerance.ParseScript(precursor)
            Dim centroidErr As Tolerance = Ms1.Tolerance.ParseScript(mzwidth)
            Dim intocutoff As LowAbundanceTrimming = LowAbundanceTrimming.ParseScript(trim)
            Dim unique As IEnumerable(Of PeakMs2) = clusters _
                .AsParallel _
                .Select(Function(cluster)
                            Return cluster.PopulateUniquePeakMatrix(parentErr, rtwidth, centroidErr, intocutoff)
                        End Function) _
                .IteratesALL

            Return unique
        End Function

        ''' <summary>
        ''' group by mz/rt
        ''' </summary>
        ''' <param name="cluster"></param>
        ''' <param name="parentErr"></param>
        ''' <param name="rtwidth#"></param>
        ''' <param name="centroidErr"></param>
        ''' <param name="intocutoff"></param>
        ''' <returns></returns>
        <Extension>
        Private Iterator Function PopulateUniquePeakMatrix(cluster As SpectrumCluster,
                                                           parentErr As Tolerance,
                                                           rtwidth#,
                                                           centroidErr As Tolerance,
                                                           intocutoff As LowAbundanceTrimming) As IEnumerable(Of PeakMs2)
            ' group by mz/rt
            ' by mz
            Dim mzgroups As NamedCollection(Of PeakMs2)() = cluster.cluster _
                .GroupBy(Function(a) a.mz, parentErr) _
                .ToArray

            ' by rt
            For Each mz As NamedCollection(Of PeakMs2) In mzgroups
                Dim rtgroups As NamedCollection(Of PeakMs2)() = mz _
                    .GroupBy(Function(a) a.rt, offsets:=rtwidth) _
                    .ToArray
                Dim mzval As Double = Aggregate ion As PeakMs2
                                      In mz
                                      Into Average(ion.mz)

                For Each rt As NamedCollection(Of PeakMs2) In rtgroups
                    Dim members As String() = rt _
                        .Select(Function(a) a.lib_guid) _
                        .Distinct _
                        .ToArray
                    Dim rtval As Double = Aggregate ion As PeakMs2
                                          In rt
                                          Into Average(ion.rt)
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
        End Function
    End Module
End Namespace
