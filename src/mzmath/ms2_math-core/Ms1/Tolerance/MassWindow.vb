Namespace Ms1

    Public Class MassWindow

        Public ReadOnly Property mass As Double
        Public ReadOnly Property mzmin As Double
        Public ReadOnly Property mzmax As Double

        Sub New(mass As Double, ppm As Double)
            Me.mass = mass

            With MzWindow(mass, ppm)
                mzmin = .lowerMz
                mzmax = .upperMz
            End With
        End Sub

        Sub New(mass As Double, mzdiff As Tolerance)
            Me.mass = mass

            If mzdiff.Type = MassToleranceType.Da Then
                mzmin = mass - mzdiff.DeltaTolerance
                mzmax = mass + mzdiff.DeltaTolerance
            Else
                With MzWindow(mass, ppm:=mzdiff.DeltaTolerance)
                    mzmin = .lowerMz
                    mzmax = .upperMz
                End With
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return $"{mass} [{mzmin}, {mzmax}]"
        End Function

    End Class
End Namespace