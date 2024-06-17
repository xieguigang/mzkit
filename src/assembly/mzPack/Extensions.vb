#Region "Microsoft.VisualBasic::607d2cc1258500b8b6caadf3c2f40d2a, assembly\mzPack\Extensions.vb"

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

    '   Total Lines: 164
    '    Code Lines: 110 (67.07%)
    ' Comment Lines: 32 (19.51%)
    '    - Xml Docs: 81.25%
    ' 
    '   Blank Lines: 22 (13.41%)
    '     File Size: 5.89 KB


    ' Module Extensions
    ' 
    '     Function: CentroidMzPack, GetAllCentroidScanMs1, GetFormatVersion, GetMSApplication, MassCalibration
    '               ProcessScan1, ProcessScan2
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq

<HideModuleName>
Public Module Extensions

    <Extension>
    Public Function GetMSApplication(file As Stream) As FileApplicationClass
        Dim ver As Integer = GetFormatVersion(file)

        If ver = 1 Then
            ' i'm not sure for version 1
            ' due to the reason of no class attribute in the raw data file
            Return FileApplicationClass.LCMS
        Else
            ' version 2 has the tag attribute about the file class
            Dim pack = New StreamPack(file, [readonly]:=True)
            Dim application As FileApplicationClass = mzStream.SafeParseClassType(pack)

            Return application
        End If
    End Function

    ''' <summary>
    ''' get of the mzpack file reader format version number
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns>
    ''' +  1 for v1 legacy version
    ''' +  2 for v2 HDS advanced version
    ''' + -1 means invalid file format
    ''' </returns>
    ''' <remarks>
    ''' this function will parse the magic header and 
    ''' then move the file pointer to the start 
    ''' location
    ''' </remarks>
    <Extension>
    Public Function GetFormatVersion(file As Stream) As Integer
        Dim buf1 As Byte() = New Byte(mzPackWriter.Magic.Length - 1) {}
        Dim buf2 As Byte() = New Byte(StreamPack.Magic.Length - 1) {}

        Call file.Seek(Scan0, origin:=SeekOrigin.Begin)
        Call file.Read(buf1, Scan0, buf1.Length)
        Call file.Seek(Scan0, origin:=SeekOrigin.Begin)
        Call file.Read(buf2, Scan0, buf2.Length)
        ' move to stream start
        Call file.Seek(Scan0, origin:=SeekOrigin.Begin)

        If Encoding.ASCII.GetString(buf1) = mzPackWriter.Magic Then
            Return 1
        ElseIf Encoding.ASCII.GetString(buf2) = StreamPack.Magic Then
            Return 2
        Else
            Return -1
        End If
    End Function

    <Extension>
    Public Function GetAllCentroidScanMs1(MS As ScanMS1(), centroid As Tolerance) As IEnumerable(Of ms1_scan)
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
    End Function

    ''' <summary>
    ''' do mass calibration
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function MassCalibration(data As mzPack, Optional da As Double = 0.1) As mzPack
        Dim mzdiff As Tolerance = Tolerance.DeltaMass(da)

        data.MS = data.MS _
            .Select(Function(ms1)
                        Return ms1.ProcessScan1(mzdiff)
                    End Function) _
            .ToArray

        Return data
    End Function

    <Extension>
    Private Function ProcessScan1(ms1 As ScanMS1, mzdiff As Tolerance) As ScanMS1
        Dim mass2 As New List(Of ScanMS2)

        For Each scan2 As ScanMS2 In ms1.products.SafeQuery
            Dim calibration As ms2 = ms1.GetMs _
                .Where(Function(d)
                           Return mzdiff(d.mz, scan2.parentMz)
                       End Function) _
                .OrderByDescending(Function(d) d.intensity) _
                .FirstOrDefault

            If Not calibration Is Nothing Then
                scan2.parentMz = calibration.mz
            End If

            mass2.Add(scan2)
        Next

        ms1.products = mass2.ToArray

        Return ms1
    End Function

    ''' <summary>
    ''' make the ms2 spectrum data inside the mzpack object centroid
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="errors"></param>
    ''' <param name="threshold"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function CentroidMzPack(data As mzPack, errors As Tolerance, threshold As LowAbundanceTrimming) As mzPack
        data.MS = data.MS _
            .AsParallel _
            .Select(Function(s1)
                        Return s1.ProcessScan2(errors, threshold)
                    End Function) _
            .ToArray

        Return data
    End Function

    <Extension>
    Private Function ProcessScan2(ms1 As ScanMS1, mzdiff As Tolerance, intocutoff As RelativeIntensityCutoff) As ScanMS1
        ms1.products = ms1.products _
            .SafeQuery _
            .Select(Function(s2)
                        Dim centroid = s2.GetMs.ToArray.Centroid(mzdiff, intocutoff).ToArray
                        s2.mz = centroid.Select(Function(a) a.mz).ToArray
                        s2.into = centroid.Select(Function(a) a.intensity).ToArray
                        Return s2
                    End Function) _
            .ToArray

        Return ms1
    End Function
End Module
