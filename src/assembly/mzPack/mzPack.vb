#Region "Microsoft.VisualBasic::85d1a10a767a51a58360904cf78f7e32, src\assembly\mzPack\mzPack.vb"

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

    ' Class mzPack
    ' 
    '     Properties: Chromatogram, MS, Scanners, Thumbnail
    ' 
    '     Function: GetAllScanMs1, GetMs2Peaks, ReadAll, Write
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' mzPack文件格式模型
''' </summary>
Public Class mzPack

    ''' <summary>
    ''' 一般为二维散点图
    ''' </summary>
    ''' <returns></returns>
    Public Property Thumbnail As Image
    Public Property MS As ScanMS1()

    ''' <summary>
    ''' TIC/BPC
    ''' </summary>
    ''' <returns></returns>
    Public Property Chromatogram As Chromatogram
    ''' <summary>
    ''' the file name of the raw data source file
    ''' </summary>
    ''' <returns></returns>
    Public Property source As String

    ''' <summary>
    ''' 其他的扫描器数据，例如紫外扫描
    ''' </summary>
    ''' <returns></returns>
    Public Property Scanners As Dictionary(Of String, ChromatogramOverlap)

    Public Function GetAllScanMs1(Optional centroid As Tolerance = Nothing) As IEnumerable(Of ms1_scan)
        If Not centroid Is Nothing Then
            Return MS _
                .Select(Function(scan)
                            Dim MSproducts As ms2() = scan.GetMs _
                                .ToArray _
                                .Centroid(centroid, LowAbundanceTrimming.intoCutff) _
                                .ToArray

                            Return MSproducts _
                                .Select(Function(mzi)
                                            Return New ms1_scan With {
                                                .mz = mzi.mz,
                                                .intensity = mzi.intensity,
                                                .scan_time = scan.rt
                                            }
                                        End Function)
                        End Function) _
                .IteratesALL
        Else
            Return MS _
                .Select(Function(scan)
                            Return scan.mz _
                                .Select(Function(mzi, i)
                                            Return New ms1_scan With {
                                                .mz = mzi,
                                                .intensity = scan.into(i),
                                                .scan_time = scan.rt
                                            }
                                        End Function)
                        End Function) _
                .IteratesALL
        End If
    End Function

    Public Iterator Function GetMs2Peaks() As IEnumerable(Of PeakMs2)
        For Each ms1 As ScanMS1 In MS
            For Each ms2 As ScanMS2 In ms1.products
                Yield New PeakMs2 With {
                    .activation = ms2.activationMethod.ToString,
                    .collisionEnergy = ms2.collisionEnergy,
                    .file = "n/a",
                    .intensity = ms2.intensity,
                    .lib_guid = ms2.ToString,
                    .meta = New Dictionary(Of String, String),
                    .mz = ms2.parentMz,
                    .precursor_type = "",
                    .rt = ms2.rt,
                    .scan = ms2.scan_id,
                    .mzInto = ms2.GetMs.ToArray
                }
            Next
        Next
    End Function

    Public Shared Function Read(filepath As String, Optional ignoreThumbnail As Boolean = False) As mzPack
        Using file As FileStream = filepath.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Return ReadAll(file, ignoreThumbnail)
        End Using
    End Function

    ''' <summary>
    ''' 一次性加载所有原始数据
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Shared Function ReadAll(file As Stream, Optional ignoreThumbnail As Boolean = False) As mzPack
        Using mzpack As New mzPackReader(file)
            Dim allMSscans As ScanMS1() = mzpack _
                .EnumerateIndex _
                .Select(AddressOf mzpack.ReadScan) _
                .ToArray
            Dim scanners As New Dictionary(Of String, ChromatogramOverlap)
            Dim source As String = Nothing

            If TypeOf file Is FileStream Then
                source = DirectCast(file, FileStream).Name.FileName
            End If

            For Each id As String In mzpack.ChromatogramScanners
                Using buffer As Stream = mzpack.OpenScannerData(id)
                    scanners(id) = buffer.ReadPackData
                End Using
            Next

            Return New mzPack With {
                .Thumbnail = If(ignoreThumbnail, Nothing, mzpack.GetThumbnail),
                .MS = allMSscans,
                .Scanners = scanners,
                .Chromatogram = mzpack.chromatogram,
                .source = source
            }
        End Using
    End Function

    Public Function Write(file As Stream) As Boolean
        Using mzpack As New mzPackWriter(file)
            For Each scan As ScanMS1 In MS
                Call mzpack.Write(scan)
            Next

            For Each scanner In Scanners.SafeQuery
                Call mzpack.AddOtherScanner(scanner.Key, scanner.Value)
            Next

            Call mzpack.SetChromatogram(Chromatogram)
            Call mzpack.SetThumbnail(Thumbnail)
            Call file.Flush()
        End Using

        Return True
    End Function
End Class
