Public Class PeakList

	Public ReadOnly Masses As Double()
	Public ReadOnly Intensities As Double()

	Public Sub New(masses As Double(), intensities As Double())
		Me.Masses = masses
		Me.Intensities = intensities
	End Sub

End Class
