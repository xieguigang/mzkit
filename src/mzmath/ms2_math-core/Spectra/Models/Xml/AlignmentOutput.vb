Imports Microsoft.VisualBasic.Linq

Namespace Spectra.Xml

    Public Class AlignmentOutput

        Public Property forward As Double
        Public Property reverse As Double
        Public Property query As Meta
        Public Property reference As Meta
        Public Property alignments As SSM2MatrixFragment()

        Public Function GetAlignmentMirror() As (query As LibraryMatrix, ref As LibraryMatrix)
            With New Ms2AlignMatrix(alignments)
                Dim q = .GetQueryMatrix.With(Sub(a) a.name = query.id)
                Dim r = .GetReferenceMatrix.With(Sub(a) a.name = reference.id)

                Return (q, r)
            End With
        End Function

    End Class

    Public Class Meta

        Public Property id As String
        Public Property mz As Double
        Public Property rt As Double

    End Class
End Namespace