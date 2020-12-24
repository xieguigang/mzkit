Namespace Spectra

    Public Class Comparison

        ReadOnly equalsScore# = 0.85
        ReadOnly gtScore# = 0.6
        ReadOnly alignment As AlignmentProvider

        Const InvalidScoreRange$ = "Scores for x < y should be in range (0, 1] and its value is also less than score for spectra equals!"

        Sub New(align As AlignmentProvider, Optional equalsScore# = 0.85, Optional gtScore# = 0.6)
            Me.gtScore = gtScore
            Me.equalsScore = equalsScore
            Me.alignment = align

            If equalsScore < 0 OrElse equalsScore > 1 Then
                Throw New InvalidConstraintException("Scores for spectra equals is invalid, it should be in range (0, 1].")
            End If
            If gtScore < 0 OrElse gtScore > 1 OrElse gtScore > equalsScore Then
                Throw New InvalidConstraintException(InvalidScoreRange)
            End If
        End Sub

        Public Function Compares(x As PeakMs2, y As PeakMs2) As Integer
            Dim scoreVal As Double = alignment.GetScore(x.mzInto, y.mzInto)

            If scoreVal >= equalsScore Then
                Return 0
            ElseIf scoreVal >= gtScore Then
                Return 1
            Else
                Return -1
            End If
        End Function
    End Class
End Namespace