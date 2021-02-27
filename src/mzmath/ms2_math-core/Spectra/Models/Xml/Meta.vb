Namespace Spectra.Xml

    Public Class Meta

        Public Property id As String
        Public Property mz As Double
        Public Property rt As Double

        Public Overrides Function ToString() As String
            Return id
        End Function

    End Class
End Namespace