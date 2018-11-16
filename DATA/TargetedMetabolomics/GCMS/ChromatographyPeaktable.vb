Namespace GCMS

    Public Class ChromatographyPeaktable : Inherits ROITable

        Public Property content As Double
        Public Property rawFile As String
        ''' <summary>
        ''' 进行内标校正之后的峰面积比值
        ''' </summary>
        ''' <returns></returns>
        Public Property TPACalibration As Double

    End Class
End Namespace