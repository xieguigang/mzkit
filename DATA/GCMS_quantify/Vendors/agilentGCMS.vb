Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MIME.application.netCDF
Imports Microsoft.VisualBasic.MIME.application.netCDF.Components
Imports SMRUCC.MassSpectrum.Math

Public Module agilentGCMS

    Public Function Read(cdf As netCDFReader) As GCMSJson
        Dim time As CDFData = cdf.getDataVariable("scan_acquisition_time")
        Dim tic = cdf.getDataVariable("total_intensity")
        Dim pointCount = cdf.getDataVariable("point_count")
        Dim massValues = cdf.getDataVariable("mass_values").tiny_num
        Dim intensityValues = cdf.getDataVariable("intensity_values").tiny_num

        Dim ms As ms1_scan()() = New ms1_scan(pointCount.Length - 1)() {}
        Dim index As int = Scan0
        Dim size%

        For i As Integer = 0 To ms.Length - 1
            size = pointCount.integers(i)
            ms(i) = New ms1_scan(size - 1) {}

            For j As Integer = 0 To size - 1
                ms(i)(j) = New ms1_scan With {
                    .mz = massValues(index),
                    .intensity = intensityValues(++index)
                }
            Next
        Next

        Return New GCMSJson With {
            .times = time,
            .tic = tic.numerics,
            .ms = ms
        }
    End Function
End Module
