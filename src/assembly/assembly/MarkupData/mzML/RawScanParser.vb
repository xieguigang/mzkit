Imports Microsoft.VisualBasic.Linq

Namespace MarkupData.mzML

    Public Class RawScanParser


        Public Shared Function CreateScanReader(rawScan As spectrum, basename$) As RawScanParser
            Dim msLevel As Integer = rawScan.ms_level.DoCall(AddressOf ParseInteger)

            Throw New NotImplementedException
        End Function

    End Class

    Public Class Ms1RawScan : Inherits RawScanParser


    End Class

    Public Class MsnRawScan : Inherits RawScanParser

    End Class
End Namespace