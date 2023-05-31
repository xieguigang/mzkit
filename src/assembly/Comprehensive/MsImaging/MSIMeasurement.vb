#Region "Microsoft.VisualBasic::50732017135c8a3eee600b6bea2b7c53, mzkit\src\assembly\Comprehensive\MsImaging\MSIMeasurement.vb"

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

'   Total Lines: 97
'    Code Lines: 75
' Comment Lines: 3
'   Blank Lines: 19
'     File Size: 3.26 KB


'     Class MSIMeasurement
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: GetCorrection, (+3 Overloads) Measure
' 
' 
' /********************************************************************************/

#End Region

#If NET48 Then
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
#End If

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports stdNum = System.Math

Namespace MsImaging

    ''' <summary>
    ''' combine helper for row scans MSI raw data files
    ''' </summary>
    Public Class MSIMeasurement

        ReadOnly maxScans As Integer
        ReadOnly maxRt As Double
        ReadOnly hasMs2 As Boolean

        Sub New(totalTime As Double, pixels As Integer, Optional hasMs2 As Boolean = False)
            Me.maxScans = pixels
            Me.hasMs2 = hasMs2
            Me.maxRt = totalTime
        End Sub

        Public Function GetCorrection() As Correction
            If hasMs2 Then
                Return New ScanMs2Correction(maxRt, maxScans)
            Else
                Return New ScanTimeCorrection(maxRt, maxScans)
            End If
        End Function

        Public Shared Function Measure(raw As IEnumerable(Of IMzPackReader)) As MSIMeasurement
            Dim scans As New List(Of Integer)
            Dim maxrt As New List(Of Double)
            Dim maxScan As Integer
            Dim scanMs2 As Boolean = False

            For Each file As IMzPackReader In raw
                maxScan = file.EnumerateIndex.Count
                scans.Add(maxScan)
                maxrt.Add(file.rtmax)

                If Not scanMs2 Then
                    scanMs2 = file.hasMs2
                End If
            Next

            Return New MSIMeasurement(maxrt.Average, scans.Average, hasMs2:=scanMs2)
        End Function

        Public Shared Function Measure(raw As IEnumerable(Of mzPack)) As MSIMeasurement
            Dim scans As New List(Of Integer)
            Dim maxrt As New List(Of Double)
            Dim maxScan As Integer
            Dim scanMs2 As Boolean = False

            For Each file As mzPack In raw
                maxScan = file.MS.Length
                scans.Add(maxScan)
                maxrt.Add(file.MS.Select(Function(scan) scan.rt).Max)

                If Not scanMs2 Then
                    scanMs2 = file.hasMs2
                End If
            Next

            Return New MSIMeasurement(maxrt.Average, scans.Average, hasMs2:=scanMs2)
        End Function

#If NET48 Then

        Public Shared Function Measure(raw As IEnumerable(Of MSFileReader), Optional scanMs2 As Boolean = False) As MSIMeasurement
            Dim scans As New List(Of Integer)
            Dim maxrt As New List(Of Double)
            Dim maxScan As Integer

            For Each file As MSFileReader In raw
                maxScan = file.ThermoReader.GetNumScans
                scans.Add(maxScan)
                maxrt.Add(file.ScanTimeMax * 60)

                If Not scanMs2 Then
                    For i As Integer = 0 To stdNum.Min(64, maxScan)
                        If file.ThermoReader.GetMSLevel(scan:=i) > 1 Then
                            scanMs2 = True
                            Exit For
                        End If
                    Next
                End If
            Next

            Return New MSIMeasurement(maxrt.Average, scans.Average, hasMs2:=scanMs2)
        End Function
#End If

    End Class
End Namespace
