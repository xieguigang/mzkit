Imports Microsoft.VisualBasic.MIME.application.netCDF

Public Module QuantifyAnalysis

    ''' <summary>
    ''' 读取CDF文件然后读取原始数据
    ''' </summary>
    ''' <param name="cdfPath"></param>
    ''' <returns></returns>
    Public Function ReadData(cdfPath$, Optional vendor$ = "agilentGCMS", Optional showSummary As Boolean = True) As GCMSJson
        Dim cdf As New netCDFReader(cdfPath)

        If showSummary Then
            Call Console.WriteLine(cdf.ToString)
        End If

        Select Case vendor
            Case "agilentGCMS" : Return agilentGCMS.Read(cdf)
            Case Else
                Throw New NotImplementedException(vendor)
        End Select
    End Function
End Module
