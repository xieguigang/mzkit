Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.netCDF
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports SMRUCC.MassSpectrum.Math.GCMS.Vendors

Namespace GCMS

    Public Module ReaderExtensions

        ''' <summary>
        ''' 读取CDF文件然后读取原始数据
        ''' </summary>
        ''' <param name="cdfPath"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ReadData(cdfPath$, Optional vendor$ = "agilentGCMS", Optional showSummary As Boolean = True) As Raw
            Return New netCDFReader(cdfPath).ReadData(vendor, showSummary)
        End Function

        <Extension>
        Public Function ReadData(cdf As netCDFReader, Optional vendor$ = "agilentGCMS", Optional showSummary As Boolean = True) As Raw
            If showSummary Then
                Call Console.WriteLine(cdf.ToString)
            End If

            Select Case vendor
                Case "agilentGCMS" : Return agilentGCMS.Read(cdf)
                Case Else
                    Throw New NotImplementedException(vendor)
            End Select
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetROITable(ROIobj As ROI, Optional getTitle As Func(Of ROI, String) = Nothing) As ROITable
            Static defaultRtTitle As New DefaultValue(Of Func(Of ROI, String))(
               Function(roi)
                   Return $"[{roi.Time.Min.ToString("F0")},{roi.Time.Max.ToString("F0")}]"
               End Function)

            Return New ROITable With {
                .baseline = ROIobj.Baseline,
                .ID = (getTitle Or defaultRtTitle)(ROIobj),
                .integration = ROIobj.Integration,
                .maxInto = ROIobj.MaxInto,
                .rtmax = ROIobj.Time.Max,
                .rtmin = ROIobj.Time.Min,
                .rt = ROIobj.rt,
                .sn = ROIobj.snRatio
            }
        End Function

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
End Namespace