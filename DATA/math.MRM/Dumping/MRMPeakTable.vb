Namespace Dumping

    Public Class MRMPeakTable

        Public Property ID As String
        Public Property Name As String
        Public Property rtmin As Double
        Public Property rtmax As Double

        Public Property content As Double

        Public Property maxinto As Double
        Public Property maxinto_IS As Double

        ''' <summary>
        ''' 峰面积
        ''' </summary>
        ''' <returns></returns>
        Public Property TPA As Double
        Public Property TPA_IS As Double
        Public Property [IS] As String
        Public Property base As Double
        Public Property raw As String

    End Class

    Public Class MRMStandards

        Public Property ID As String
        Public Property Name As String

        Public Property AIS As Double
        Public Property Ati As Double
        Public Property cIS As Double
        Public Property Cti As Double

        Public Property level As String
    End Class
End Namespace