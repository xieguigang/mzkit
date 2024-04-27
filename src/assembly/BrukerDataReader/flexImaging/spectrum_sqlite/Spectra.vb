#Region "Microsoft.VisualBasic::6ec5d63dca07988a30fb7533d10feae1, G:/mzkit/src/assembly/BrukerDataReader//flexImaging/spectrum_sqlite/Spectra.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 40
    '    Code Lines: 11
    ' Comment Lines: 26
    '   Blank Lines: 3
    '     File Size: 1.17 KB


    ' Class Spectra
    ' 
    '     Properties: Chip, Id, NumPeaks, PeakIntensityValues, PeakMzValues
    '                 RegionNumber, SpotName, XIndexPos, YIndexPos
    ' 
    ' /********************************************************************************/

#End Region

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
