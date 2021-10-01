#Region "Microsoft.VisualBasic::6e5f92233b5e360032224371ada0e158, src\assembly\Comprehensive\MsImaging\MsImagingRaw.vb"

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

    '     Module MsImagingRaw
    ' 
    '         Function: MeasureRow, MSICombineRowScans
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Expressions

Namespace MsImaging

    ''' <summary>
    ''' raw data file reader helper code
    ''' </summary>
    Public Module MsImagingRaw

        <Extension>
        Public Function MSICombineRowScans(src As IEnumerable(Of mzPack),
                                           correction As Correction,
                                           Optional intocutoff As Double = 0.05,
                                           Optional yscale As Double = 1,
                                           Optional progress As RunSlavePipeline.SetMessageEventHandler = Nothing) As mzPack

            Dim pixels As New List(Of ScanMS1)
            Dim cutoff As New RelativeIntensityCutoff(intocutoff)

            If progress Is Nothing Then
                progress = Sub(msg)
                               ' do nothing
                           End Sub
            End If

            For Each row As mzPack In src
                pixels += row.MeasureRow(yscale, correction, cutoff, progress)
            Next

            Return New mzPack With {.MS = pixels.ToArray}
        End Function

        <Extension>
        Private Iterator Function MeasureRow(row As mzPack,
                                             yscale As Double,
                                             correction As Correction,
                                             cutoff As RelativeIntensityCutoff,
                                             progress As RunSlavePipeline.SetMessageEventHandler) As IEnumerable(Of ScanMS1)
            Dim i As i32 = 1
            Dim y As Integer = row.source _
                .Match("\d+") _
                .DoCall(AddressOf Integer.Parse) * yscale

            Call progress($"load: {row.source}...")

            If TypeOf correction Is ScanMs2Correction Then
                Call DirectCast(correction, ScanMs2Correction).SetMs1Scans(row.MS)
            End If

            For Each scan As ScanMS1 In row.MS
                Dim x As Integer = If(correction Is Nothing, ++i, correction.GetPixelRowX(scan))
                Dim ms As ms2() = cutoff.Trim(scan.GetMs)
                Dim mz As Double() = ms.Select(Function(m) m.mz).ToArray
                Dim into As Double() = ms.Select(Function(m) m.intensity).ToArray

                Yield New ScanMS1 With {
                    .BPC = scan.BPC,
                    .into = into,
                    .mz = mz,
                    .meta = New Dictionary(Of String, String) From {{NameOf(x), x}, {NameOf(y), y}},
                    .rt = scan.rt,
                    .scan_id = $"[{row.source}] {scan.scan_id}",
                    .TIC = scan.TIC,
                    .products = scan.products
                }
            Next
        End Function
    End Module
End Namespace
