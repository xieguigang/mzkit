Public Class ScanInfo

	Public Property CycleId As Integer
	Public Property ExperimentId As Integer

	Public Property ScanType As ScanType

	Public Property ScanMode As ScanMode

	Public Property Polarity As Polarity

	Public Property MSLevel As Integer

	Public Property ScanNumber As Integer

	Public Property RetentionTime As Single

	Public Property PeaksCount As Integer

	Public Property LowMz As Double

	Public Property HighMz As Double

	Public Property TotalIonCurrent As Single

	Public Property BasePeakMz As Double

	Public Property BasePeakIntensity As Single

	Public Property DataTitle As String

	Public Property PrecursorMz As Double

	Public Property PrecursorCharge As Integer

	Public Property PrecursorIntensity As Single

	Public Property CollisionEnergy As Single

	Public Property FragmentationType As String

	Public Property IsolationCenter As Double

	Public Property IsolationWidth As Double

End Class
