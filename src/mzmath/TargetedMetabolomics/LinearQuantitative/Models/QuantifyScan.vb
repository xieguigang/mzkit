Imports Microsoft.VisualBasic.Data.csv.IO

Namespace LinearQuantitative

    Public Class QuantifyScan

        Public Property MRMPeaks As IonPeakTableRow()

        ''' <summary>
        ''' 定量结果
        ''' </summary>
        ''' <returns></returns>
        Public Property quantify As DataSet

        ''' <summary>
        ''' 原始的峰面积数据
        ''' </summary>
        ''' <returns></returns>
        Public Property rawX As DataSet

    End Class
End Namespace