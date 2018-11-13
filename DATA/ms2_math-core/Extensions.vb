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

    ''' <summary>
    ''' 根据保留时间来计算出保留指数
    ''' </summary>
    ''' <param name="rt"></param>
    ''' <param name="A"></param>
    ''' <param name="B"></param>
    ''' <returns></returns>
    <Extension>
    Public Function RetentionIndex(rt As IRetentionTime, A As (rt#, ri#), B As (rt#, ri#)) As Double
        Dim rtScale = (rt.rt - A.rt) / (B.rt - A.rt)
        Dim riScale = (B.ri - A.ri) * rtScale
        Dim ri = A.ri + riScale
        Return ri
    End Function
End Module
