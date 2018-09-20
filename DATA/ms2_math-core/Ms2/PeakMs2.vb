Namespace MSMS

    Public Structure PeakMs2
        Dim mz As Double
        Dim rt As Double
        Dim file As String
        Dim scan As Integer
        Dim mzInto As LibraryMatrix

        Public Shared Function RtInSecond(rt As String) As Double
            rt = rt.Substring(2)
            rt = rt.Substring(0, rt.Length - 1)
            Return Double.Parse(rt)
        End Function
    End Structure

End Namespace