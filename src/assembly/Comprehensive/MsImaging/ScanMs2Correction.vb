#Region "Microsoft.VisualBasic::46a0517e75a1de13b62cf4c13ebc4f89, assembly\Comprehensive\MsImaging\ScanMs2Correction.vb"

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

    '   Total Lines: 70
    '    Code Lines: 54 (77.14%)
    ' Comment Lines: 3 (4.29%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 13 (18.57%)
    '     File Size: 2.51 KB


    '     Class ScanMs2Correction
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetPixelRowX, (+3 Overloads) GetTotalScanNumbers
    ' 
    '         Sub: SetMs1Scans
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Linq

Namespace MsImaging

    ''' <summary>
    ''' processing the MSI raw data contains with ms2 scan data
    ''' </summary>
    Public Class ScanMs2Correction : Inherits Correction

        Dim scans As ScanMS1()
        Dim scanId As Dictionary(Of String, Integer)
        Dim rtPixel As ScanTimeCorrection

        Sub New(totalTime As Double, pixels As Integer)
            rtPixel = New ScanTimeCorrection(totalTime, pixels)
        End Sub

        Public Sub SetMs1Scans(scans As IEnumerable(Of ScanMS1))
            Me.scans = scans.OrderBy(Function(i) i.rt).ToArray
            Me.scanId = Me.scans _
                .SeqIterator _
                .ToDictionary(Function(scan) scan.value.scan_id,
                              Function(scan)
                                  Return scan.i
                              End Function)
        End Sub

        Public Overrides Function GetPixelRowX(scanMs1 As ScanMS1) As Integer
            Dim i As Integer = scanId(scanMs1.scan_id)
            Dim nMs1 As Integer = -1
            Dim total As Integer = GetTotalScanNumbers(index:=i, ms1Count:=nMs1)
            Dim skipMs2 As Integer = total - nMs1

            Return rtPixel.GetPixelRow(scanMs1.rt) - skipMs2
        End Function

        Public Function GetTotalScanNumbers(index As Integer, Optional ByRef ms1Count As Integer = -1) As Integer
            If index = 0 Then
                Return 0
            End If

            Dim before As ScanMS1() = scans _
                .Take(index) _
                .ToArray

            ms1Count = before.Length

            Return Aggregate scan As ScanMS1
                   In before
                   Let total As Integer = GetTotalScanNumbers(scan)
                   Into Sum(total)
        End Function

        Public Shared Function GetTotalScanNumbers(scan As ScanMS1) As Integer
            If scan.products.IsNullOrEmpty Then
                Return 1
            Else
                Return scan.products.Length + 1
            End If
        End Function

        Public Shared Function GetTotalScanNumbers(raw As mzPack) As Integer
            Return Aggregate scan As ScanMS1
                   In raw.MS
                   Let total As Integer = GetTotalScanNumbers(scan)
                   Into Sum(total)
        End Function
    End Class
End Namespace
