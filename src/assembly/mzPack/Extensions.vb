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

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

<HideModuleName>
Public Module Extensions

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
    Public Function Scan1(list As NamedCollection(Of PeakMs2)) As ScanMS1
        Dim scan2 As ScanMS2() = list _
            .Select(Function(i)
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
End Module
