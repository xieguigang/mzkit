Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
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
                                    .type = CDFDataTypes.CHAR,
                                    .value = t.Value
                                }
                            End Function) _
                    .ToList
                Dim size As New Dimension With {.name = $"sizeof_scan{++i}", .size = data.Length}

                meta.Add(New attribute With {.name = "n_products", .type = CDFDataTypes.INT, .value = scan.products.TryCount})
                meta.Add(New attribute With {.name = "n_points", .type = CDFDataTypes.INT, .value = scan.size})
                meta.Add(New attribute With {.name = "tic", .type = CDFDataTypes.DOUBLE, .value = scan.TIC})
                meta.Add(New attribute With {.name = "bpc", .type = CDFDataTypes.DOUBLE, .value = scan.BPC})
                meta.Add(New attribute With {.name = "retention_time", .type = CDFDataTypes.DOUBLE, .value = scan.rt})
                meta.Add(New attribute With {.name = "mslevel", .type = CDFDataTypes.INT, .value = 1})

                If Not ms2Only Then
                    writer.AddVariable(scan.scan_id, data, size, meta.ToArray)
                End If

                For Each ms2 As ScanMS2 In scan.products
                    data = New doubles(ms2.mz.JoinIterates(ms2.into))
                    size = New Dimension With {.name = $"sizeof_scan{++i}", .size = data.Length}
                    meta.Clear()
                    meta.Add(New attribute With {.name = "mz", .type = CDFDataTypes.DOUBLE, .value = ms2.parentMz})
                    meta.Add(New attribute With {.name = "rt", .type = CDFDataTypes.DOUBLE, .value = ms2.rt})
                    meta.Add(New attribute With {.name = "n_fragments", .type = CDFDataTypes.DOUBLE, .value = ms2.size})
                    meta.Add(New attribute With {.name = "scan_ms1", .type = CDFDataTypes.CHAR, .value = scan.scan_id})
                    meta.Add(New attribute With {.name = "mslevel", .type = CDFDataTypes.INT, .value = 2})

                    writer.AddVariable(ms2.scan_id, data, size, meta.ToArray)
                Next
            Next
        End Using
    End Sub

    <Extension>
    Private Iterator Function getFileAttributes(file As mzPack) As IEnumerable(Of attribute)
        Yield New attribute With {.name = "application", .type = CDFDataTypes.CHAR, .value = file.Application.Description}
        Yield New attribute With {.name = "url", .type = CDFDataTypes.CHAR, .value = "https://mzkit.org"}
        Yield New attribute With {.name = "n_scans", .type = CDFDataTypes.INT, .value = file.MS.Length}
        Yield New attribute With {.name = "timestamp", .type = CDFDataTypes.CHAR, .value = Now.ToString}
    End Function
End Module
