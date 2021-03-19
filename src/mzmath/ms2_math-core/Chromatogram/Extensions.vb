Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math

Namespace Chromatogram

    <HideModuleName>
    Public Module Extensions

        <Extension>
        Public Iterator Function XIC(ms1 As IEnumerable(Of ms1_scan), mzErr As Tolerance) As IEnumerable(Of (mz As Double, chromatogram As ChromatogramTick()))
            For Each mz As NamedCollection(Of ms1_scan) In ms1 _
                .GroupBy(Function(p) p.mz, mzErr) _
                .OrderBy(Function(mzi)
                             Return Val(mzi.name)
                         End Function)

                Dim scan = mz.OrderBy(Function(p) p.scan_time).ToArray
                Dim ticks = scan _
                    .Select(Function(p)
                                Return New ChromatogramTick With {
                                    .Intensity = p.intensity,
                                    .Time = p.scan_time
                                }
                            End Function) _
                    .ToArray

                Yield (Val(mz.name), ticks)
            Next
        End Function
    End Module
End Namespace