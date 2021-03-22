#Region "Microsoft.VisualBasic::baf821cd73bf8a39b2116cbce1d583ea, src\mzmath\TargetedMetabolomics\GCMS\mzMLReader\TimeScanMatrix.vb"

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

    '     Class TimeScanMatrix
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateMatrixHelper, TimeScan
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports stdNum = System.Math

Namespace GCMS

    Public Class TimeScanMatrix

        ReadOnly mz_scanList As List(Of ms1_scan)()
        ReadOnly mzList As Double()

        Private Sub New(mzScans As ms1_scan()())
            mzList = mzScans.Select(Function(mzi) mzi(Scan0).mz).ToArray
            mz_scanList = mzScans.Select(Function(mz) mz.ToList).ToArray
        End Sub

        Public Iterator Function TimeScan(time As Double) As IEnumerable(Of ms1_scan)
            For i As Integer = 0 To mz_scanList.Length - 1
                Dim list = mz_scanList(i)
                Dim tick As ms1_scan = list _
                    .Where(Function(t) stdNum.Abs(t.scan_time - time) <= 0.5) _
                    .FirstOrDefault

                If tick Is Nothing Then
                    Yield New ms1_scan With {
                        .intensity = 0,
                        .mz = mzList(i),
                        .scan_time = time
                    }
                Else
                    list.Remove(tick)
                    Yield tick
                End If
            Next
        End Function

        Friend Shared Function CreateMatrixHelper(mzScans As ms1_scan()()) As TimeScanMatrix
            Return New TimeScanMatrix(mzScans)
        End Function
    End Class
End Namespace
