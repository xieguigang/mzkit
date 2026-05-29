Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.Linq

Namespace mzData.mzWebCache

    Public Module Utils

        <Extension>
        Public Function GetMs1Points(raw As IEnumerable(Of ScanMS1)) As ms1_scan()
            Return raw _
            .Select(Function(m1)
                        Return m1.mz _
                            .Select(Function(mzi, i)
                                        Return New ms1_scan With {
                                            .mz = mzi,
                                            .intensity = m1.into(i),
                                            .scan_time = m1.rt
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToArray
        End Function
    End Module
End Namespace