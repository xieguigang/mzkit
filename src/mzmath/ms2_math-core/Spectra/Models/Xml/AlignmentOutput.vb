Namespace Spectra.Xml

    Public Class AlignmentOutput

        Public Property forward As Double
        Public Property reverse As Double
        Public Property query As Meta
        Public Property reference As Meta
        Public Property alignments As SSM2MatrixFragment()

    End Class

    Public Class Meta

        Public Property id As String
        Public Property mz As Double
        Public Property rt As Double

    End Class
End Namespace