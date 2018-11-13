Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.MassSpectrum.Math.Chromatogram

Public Module Extensions

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ToTable(ROIlist As IEnumerable(Of ROI), Optional getTitle As Func(Of ROI, String) = Nothing) As ROITable()
        Return ROIlist _
            .SafeQuery _
            .Select(Function(ROI, i)
                        Return ROI _
                            .GetROITable(Function(region)
                                             If getTitle Is Nothing Then
                                                 Return "#" & (i + 1)
                                             Else
                                                 Return getTitle(region)
                                             End If
                                         End Function)
                    End Function) _
            .ToArray
    End Function
End Module
