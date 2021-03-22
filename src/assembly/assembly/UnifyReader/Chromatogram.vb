#Region "Microsoft.VisualBasic::c8bd2b0bc76c07be08d420b75bb1f556, src\assembly\assembly\UnifyReader\Chromatogram.vb"

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

    '     Class Chromatogram
    ' 
    '         Properties: BPC, length, scan_time, TIC
    ' 
    '         Function: GetChromatogram, GetSignal, GetTicks, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.Math.SignalProcessing

Namespace DataReader

    ''' <summary>
    ''' A data union model of TIC/BPC
    ''' </summary>
    Public Class Chromatogram

        Public Property scan_time As Double()

        ''' <summary>
        ''' total ion current
        ''' </summary>
        ''' <returns></returns>
        Public Property TIC As Double()
        ''' <summary>
        ''' base peak intensity
        ''' </summary>
        ''' <returns></returns>
        Public Property BPC As Double()

        Public ReadOnly Property length As Integer
            Get
                Return scan_time.Length
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"Chromatogram between scan_time [{CInt(scan_time.Min)},{CInt(scan_time.Max)}]"
        End Function

        Public Iterator Function GetTicks(Optional isbpc As Boolean = False) As IEnumerable(Of ChromatogramTick)
            For i As Integer = 0 To scan_time.Length - 1
                If isbpc Then
                    Yield New ChromatogramTick With {.Time = scan_time(i), .Intensity = BPC(i)}
                Else
                    Yield New ChromatogramTick With {.Time = scan_time(i), .Intensity = TIC(i)}
                End If
            Next
        End Function

        Public Function GetSignal(Optional isbpc As Boolean = False) As GeneralSignal
            Return New GeneralSignal With {
                .description = If(isbpc, "BPC", "TIC"),
                .Measures = scan_time,
                .measureUnit = "seconds",
                .reference = "n/a",
                .Strength = If(isbpc, BPC, TIC)
            }
        End Function

        Public Shared Function GetChromatogram(Of Scan)(scans As IEnumerable(Of Scan)) As Chromatogram
            Dim scan_time As New List(Of Double)
            Dim tic As New List(Of Double)
            Dim bpc As New List(Of Double)
            Dim reader As MsDataReader(Of Scan) = MsDataReader(Of Scan).ScanProvider

            For Each scanVal As Scan In scans.Where(Function(s) reader.GetMsLevel(s) = 1)
                Call scan_time.Add(reader.GetScanTime(scanVal))
                Call tic.Add(reader.GetTIC(scanVal))
                Call bpc.Add(reader.GetBPC(scanVal))
            Next

            Return New Chromatogram With {
                .BPC = bpc.ToArray,
                .scan_time = scan_time.ToArray,
                .TIC = tic.ToArray
            }
        End Function
    End Class
End Namespace
