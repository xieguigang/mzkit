#Region "Microsoft.VisualBasic::41ba9f45bc8152f6f446f03beea604a6, src\assembly\assembly\mzPack\Stream\ScanMS1.vb"

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

    '     Class ScanMS1
    ' 
    '         Properties: BPC, products, TIC
    ' 
    '         Function: GetMs1Scans
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math

Namespace mzData.mzWebCache

    ''' <summary>
    ''' MS scan
    ''' </summary>
    Public Class ScanMS1 : Inherits MSScan

        Public Property TIC As Double
        Public Property BPC As Double
        Public Property products As ScanMS2()

        Public Iterator Function GetMs1Scans() As IEnumerable(Of ms1_scan)
            For i As Integer = 0 To mz.Length - 1
                Yield New ms1_scan With {
                    .mz = mz(i),
                    .intensity = into(i),
                    .scan_time = rt
                }
            Next
        End Function
    End Class
End Namespace
