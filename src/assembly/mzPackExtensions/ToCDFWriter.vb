#Region "Microsoft.VisualBasic::1b9144d438a659e09de7e7fcf2cbb940, E:/mzkit/src/assembly/mzPackExtensions//ToCDFWriter.vb"

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

    '   Total Lines: 74
    '    Code Lines: 60
    ' Comment Lines: 5
    '   Blank Lines: 9
    '     File Size: 4.11 KB


    ' Module ToCDFWriter
    ' 
    '     Function: getFileAttributes
    ' 
    '     Sub: WriteCDF
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.DataStorage.netCDF.Components
Imports Microsoft.VisualBasic.DataStorage.netCDF.Data
Imports Microsoft.VisualBasic.DataStorage.netCDF.DataVector
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Module ToCDFWriter

    ''' <summary>
    ''' write raw data file to netcdf for read in R script language
    ''' </summary>
    ''' <param name="mzpack"></param>
    ''' <param name="file"></param>
    <Extension>
    Public Sub WriteCDF(mzpack As mzPack, file As Stream, Optional ms2Only As Boolean = False)
        Using writer As New CDFWriter(file)
            Dim fileAttr As attribute() = mzpack.getFileAttributes.ToArray
            Dim i As i32 = 1

            Call writer.GlobalAttributes(fileAttr)

            For Each scan As ScanMS1 In mzpack.MS
                Dim data As New doubles(scan.mz.JoinIterates(scan.into))
                Dim meta = scan.meta _
                    .SafeQuery _
                    .Select(Function(t)
                                Return New attribute With {
                                    .name = t.Key,
                                    .type = CDFDataTypes.NC_CHAR,
                                    .value = t.Value
                                }
                            End Function) _
                    .ToList
                Dim size As New Dimension With {.name = $"sizeof_scan{++i}", .size = data.Length}

                meta.Add(New attribute With {.name = "n_products", .type = CDFDataTypes.NC_INT, .value = scan.products.TryCount})
                meta.Add(New attribute With {.name = "n_points", .type = CDFDataTypes.NC_INT, .value = scan.size})
                meta.Add(New attribute With {.name = "tic", .type = CDFDataTypes.NC_DOUBLE, .value = scan.TIC})
                meta.Add(New attribute With {.name = "bpc", .type = CDFDataTypes.NC_DOUBLE, .value = scan.BPC})
                meta.Add(New attribute With {.name = "retention_time", .type = CDFDataTypes.NC_DOUBLE, .value = scan.rt})
                meta.Add(New attribute With {.name = "mslevel", .type = CDFDataTypes.NC_INT, .value = 1})

                If Not ms2Only Then
                    writer.AddVariable(scan.scan_id, data, size, meta.ToArray)
                End If

                For Each ms2 As ScanMS2 In scan.products
                    data = New doubles(ms2.mz.JoinIterates(ms2.into))
                    size = New Dimension With {.name = $"sizeof_scan{++i}", .size = data.Length}
                    meta.Clear()
                    meta.Add(New attribute With {.name = "mz", .type = CDFDataTypes.NC_DOUBLE, .value = ms2.parentMz})
                    meta.Add(New attribute With {.name = "rt", .type = CDFDataTypes.NC_DOUBLE, .value = ms2.rt})
                    meta.Add(New attribute With {.name = "n_fragments", .type = CDFDataTypes.NC_DOUBLE, .value = ms2.size})
                    meta.Add(New attribute With {.name = "scan_ms1", .type = CDFDataTypes.NC_CHAR, .value = scan.scan_id})
                    meta.Add(New attribute With {.name = "mslevel", .type = CDFDataTypes.NC_INT, .value = 2})

                    writer.AddVariable(ms2.scan_id, data, size, meta.ToArray)
                Next
            Next
        End Using
    End Sub

    <Extension>
    Private Iterator Function getFileAttributes(file As mzPack) As IEnumerable(Of attribute)
        Yield New attribute With {.name = "application", .type = CDFDataTypes.NC_CHAR, .value = file.Application.Description}
        Yield New attribute With {.name = "url", .type = CDFDataTypes.NC_CHAR, .value = "https://mzkit.org"}
        Yield New attribute With {.name = "n_scans", .type = CDFDataTypes.NC_INT, .value = file.MS.Length}
        Yield New attribute With {.name = "timestamp", .type = CDFDataTypes.NC_CHAR, .value = Now.ToString}
    End Function
End Module
