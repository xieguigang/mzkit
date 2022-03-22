#Region "Microsoft.VisualBasic::d5d272999c9bd0e0505d723fd120a6bb, mzkit\src\assembly\sciexWiffReader\WiffFileReader\ScanInfo.vb"

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

'   Total Lines: 46
'    Code Lines: 24
' Comment Lines: 0
'   Blank Lines: 22
'     File Size: 976.00 B


' Class ScanInfo
' 
'     Properties: BasePeakIntensity, BasePeakMz, CollisionEnergy, CycleId, DataTitle
'                 ExperimentId, FragmentationType, HighMz, IsolationCenter, IsolationWidth
'                 LowMz, MSLevel, PeaksCount, Polarity, PrecursorCharge
'                 PrecursorIntensity, PrecursorMz, RetentionTime, ScanMode, ScanNumber
'                 ScanType, TotalIonCurrent
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math

Public Class ScanInfo : Implements IRetentionTime

    Public Property CycleId As Integer
    Public Property ExperimentId As Integer

    Public Property ScanType As ScanType

    Public Property ScanMode As ScanMode

    Public Property Polarity As Polarity

    Public Property MSLevel As Integer

    Public Property ScanNumber As Integer

    Public Property RetentionTime As Double Implements IRetentionTime.rt

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
