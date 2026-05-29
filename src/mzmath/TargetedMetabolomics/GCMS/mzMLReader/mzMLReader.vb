#Region "Microsoft.VisualBasic::3229b1f9f8eb9b0912484b27f578638e, mzmath\TargetedMetabolomics\GCMS\mzMLReader\mzMLReader.vb"

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

    '   Total Lines: 72
    '    Code Lines: 63 (87.50%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (12.50%)
    '     File Size: 2.91 KB


    '     Module mzMLReader
    ' 
    '         Function: LoadFile, readMs, TimelineTranspose
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.SignalReader.ChromatogramReader
Imports RawChromatogram = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.chromatogram

Namespace GCMS

    Public Module mzMLReader

        Public Function LoadFile(path As String) As Raw
            Dim chromatogramList = mzML _
                .LoadChromatogramList(path) _
                .ToArray
            Dim TIC As ChromatogramTick() = chromatogramList _
                .Where(Function(c) c.id = "TIC") _
                .First _
                .Ticks _
                .OrderBy(Function(tick) tick.Time) _
                .ToArray

            Return New Raw With {
                .fileName = path,
                .title = path.BaseName,
                .attributes = New Dictionary(Of String, String),
                .tic = TIC.Select(Function(tick) tick.Intensity).ToArray,
                .times = TIC.Select(Function(tick) tick.Time).ToArray,
                .ms = chromatogramList _
                    .Where(Function(c) c.id <> "TIC") _
                    .Select(AddressOf readMs) _
                    .ToArray _
                    .TimelineTranspose(.times),
                .mz = .ms.MzList
            }
        End Function

        <Extension>
        Private Function TimelineTranspose(mzScans As ms1_scan()(), times As Double()) As ms1_scan()()
            Dim time_scans As New List(Of ms1_scan())
            Dim checkScan As TimeScanMatrix = TimeScanMatrix.CreateMatrixHelper(mzScans)

            For Each time As Double In times
                Call time_scans.Add(checkScan.TimeScan(time).ToArray)
            Next

            Return time_scans.ToArray
        End Function

        Private Function readMs(chromatogram As RawChromatogram) As ms1_scan()
            Dim mz As Double = chromatogram _
                .precursor _
                .isolationWindow _
                .cvParams _
                .Where(Function(par) par.name = "isolation window target m/z") _
                .First _
                .GetDouble
            Dim ticks As ChromatogramTick() = chromatogram.Ticks
            Dim ms1 As ms1_scan() = ticks _
                .Select(Function(tick)
                            Return New ms1_scan With {
                                .intensity = tick.Intensity,
                                .mz = mz,
                                .scan_time = tick.Time
                            }
                        End Function) _
                .ToArray

            Return ms1
        End Function
    End Module
End Namespace
