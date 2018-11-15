Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MIME.application.netCDF
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
    End Module
End Namespace