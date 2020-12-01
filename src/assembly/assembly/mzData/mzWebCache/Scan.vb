Namespace mzData.mzWebCache

    Public Class ScanMS2 : Inherits MSScan

        Public Property parentMz As Double
        Public Property intensity As Double
        Public Property polarity As Integer

    End Class

    Public Class ScanMS1 : Inherits MSScan

        Public Property TIC As Double
        Public Property BPC As Double
        Public Property products As ScanMS2()

    End Class

    Public Class MSScan
        Public Property rt As Integer
        Public Property scan_id As String
        Public Property mz As Double()
        Public Property into As Double()
    End Class
End Namespace