#Region "Microsoft.VisualBasic::3b1ee5e83c1d4ec1bab0248ef166fec0, mzkit\src\assembly\sciexWiffReader\WiffFileReader\ScanInfo.vb"

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

'   Total Lines: 68
'    Code Lines: 42
' Comment Lines: 0
'   Blank Lines: 26
'     File Size: 2.00 KB


' Class ScanInfo
' 
'     Properties: BasePeakIntensity, BasePeakMz, CollisionEnergy, CycleId, DataTitle
'                 ExperimentId, FragmentationType, HighMz, IsolationCenter, IsolationWidth
'                 LowMz, MSLevel, PeaksCount, Polarity, PrecursorCharge
'                 PrecursorIntensity, PrecursorMz, RetentionTime, SampleName, ScanMode
'                 ScanNumber, ScanType, TotalIonCurrent
' 
'     Function: ToString
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Clearcore2.Data.DataAccess.SampleData
Imports stdNum = System.Math

Public Class ScanInfo : Implements IRetentionTime, IMsScanData

    Public Property CycleId As Integer
    Public Property ExperimentId As Integer

    Public Property ScanType As ScanType

    Public Property ScanMode As ScanMode

    Public Property Polarity As Polarity

    Public Property MSLevel As Integer Implements IMsScanData.MSLevel

    Public Property ScanNumber As Integer

    Public Property RetentionTime As Double Implements IRetentionTime.rt, IMsScanData.ScanTime

    Public Property PeaksCount As Integer

    Public Property LowMz As Double

    Public Property HighMz As Double

    Public Property TotalIonCurrent As Double Implements IMsScanData.TotalIonCurrent

    Public Property BasePeakMz As Double

    Public Property BasePeakIntensity As Double Implements IMsScanData.BasePeakIntensity

    Public Property DataTitle As String
    Public Property SampleName As String

    Public Property PrecursorMz As Double

    Public Property PrecursorCharge As Integer

    Public Property PrecursorIntensity As Single

    Public Property CollisionEnergy As Single

    Public Property FragmentationType As String

    Public Property IsolationCenter As Double

    Public Property IsolationWidth As Double
    Public Property details As MSExperimentInfo

    Public Overrides Function ToString() As String
        Dim xcms_id As String
        Dim nT As Integer = stdNum.Round(RetentionTime * 60)
        Dim datatitle As String = $"{Me.DataTitle}; {Polarity} {ScanMode} npeaks={PeaksCount}, m/z scan=[{LowMz}, {HighMz}], basepeak={BasePeakMz}({BasePeakIntensity})"

        If MSLevel = 1 Then
            xcms_id = $"RT={RetentionTime / 60}min"
        Else
            If nT = 0 Then
                xcms_id = $" M{stdNum.Round(PrecursorMz)}"
            Else
                xcms_id = $" M{stdNum.Round(PrecursorMz)}T{nT}"
            End If
        End If

        Return $"{If(MSLevel = 1, "[MS1]", "[MSn]")}[Scan_{ScanNumber}@{SampleName}] {datatitle}{xcms_id}"
    End Function

End Class
