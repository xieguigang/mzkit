Namespace Ms1

    Public Interface IMassBin

        Property mass As Double
        Property min As Double
        Property max As Double

    End Interface

    ''' <summary>
    ''' the m/z bin data
    ''' </summary>
    Public Class MassWindow : Implements IMassBin

        Public Property mass As Double Implements IMassBin.mass
        Public Property mzmin As Double Implements IMassBin.min
        Public Property mzmax As Double Implements IMassBin.max

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