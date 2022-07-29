Public Class Spectra

    ' CREATE TABLE Spectra (
    ' Id INTEGER PRIMARY KEY,
    ' Chip INTEGER NOT NULL,
    ' SpotName TEXT,
    ' RegionNumber INTEGER,
    ' XIndexPos INTEGER,
    ' YIndexPos INTEGER,
    ' AcquisitionKey INTEGER NOT NULL,
    ' ParentMass REAL,
    ' DateTime TEXT,
    ' CalibrationDateTime TEXT,
    ' MotorPositionX REAL,
    ' MotorPositionY REAL,
    ' NumSummations INTEGER,
    ' LaserPower REAL,
    ' LaserRepRate REAL,
    ' NumPeaks INTEGER NOT NULL,
    ' PeakMzValues BLOB NOT NULL,
    ' PeakIntensityValues BLOB NOT NULL,
    ' PeakFwhmValues BLOB NOT NULL,
    ' PeakSnrValues BLOB,
    ' PeakFlags BLOB,
    ' BH BLOB,
    ' BC TEXT,
    ' FOREIGN KEY (AcquisitionKey) REFERENCES AcquisitionKeys(Id)
    ' )

    Public Property Id As Integer
    Public Property Chip As Integer
    Public Property SpotName As String
    Public Property RegionNumber As Integer
    Public Property XIndexPos As Integer
    Public Property YIndexPos As Integer
    Public Property NumPeaks As Integer
    Public Property PeakMzValues As Double()
    Public Property PeakIntensityValues As Double()

End Class
