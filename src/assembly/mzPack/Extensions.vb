#Region "Microsoft.VisualBasic::38869a9a99bff99d249c15c1d9d83b76, mzkit\src\assembly\mzPack\Extensions.vb"

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

'   Total Lines: 30
'    Code Lines: 27
' Comment Lines: 0
'   Blank Lines: 3
'     File Size: 1.31 KB


' Module Extensions
' 
'     Function: GetAllCentroidScanMs1
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
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports stdVec = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

<HideModuleName>
Public Module Extensions

    ''' <summary>
    ''' get of the mzpack file reader format version number
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns>
    ''' +  1 for v1 legacy version
    ''' +  2 for v2 HDS advanced version
    ''' + -1 means invalid file format
    ''' </returns>
    <Extension>
    Public Function GetFormatVersion(file As Stream) As Integer
        Dim buf1 As Byte() = New Byte(mzPackWriter.Magic.Length - 1) {}
        Dim buf2 As Byte() = New Byte(StreamPack.Magic.Length - 1) {}

        Call file.Seek(Scan0, origin:=SeekOrigin.Begin)
        Call file.Read(buf1, Scan0, buf1.Length)
        Call file.Seek(Scan0, origin:=SeekOrigin.Begin)
        Call file.Read(buf2, Scan0, buf2.Length)
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

    <Extension>
    Public Function Scan2(i As PeakMs2) As ScanMS2
        Return New ScanMS2 With {
            .centroided = True,
            .mz = i.mzInto.Select(Function(mzi) mzi.mz).ToArray,
            .into = i.mzInto.Select(Function(mzi) mzi.intensity).ToArray,
            .parentMz = i.mz,
            .intensity = i.intensity,
            .rt = i.rt,
            .scan_id = $"{i.file}#{i.lib_guid}",
            .collisionEnergy = i.collisionEnergy
        }
    End Function

    <Extension>
    Public Function Scan1(list As NamedCollection(Of PeakMs2)) As ScanMS1
        Dim scan2 As ScanMS2() = list _
            .Select(Function(i)
                        Return i.Scan2
                    End Function) _
            .ToArray

        Return New ScanMS1 With {
           .into = scan2 _
               .Select(Function(i) i.intensity) _
               .ToArray,
           .mz = scan2 _
               .Select(Function(i) i.parentMz) _
               .ToArray,
           .products = scan2,
           .rt = Val(list.name),
           .scan_id = list.name,
           .TIC = .into.Sum,
           .BPC = .into.Max
        }
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
                    End Function) _
            .ToArray

        Return data
    End Function
End Module
