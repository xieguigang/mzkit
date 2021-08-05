Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components

Public Module ToCDFWriter

    ''' <summary>
    ''' write raw data file to netcdf for read in R script language
    ''' </summary>
    ''' <param name="mzpack"></param>
    ''' <param name="file"></param>
    <Extension>
    Public Sub WriteCDF(mzpack As mzPack, file As Stream)
        Using writer As New CDFWriter(file)
            Dim fileAttr As attribute() = {
                New attribute With {.name = "application", .type = CDFDataTypes.CHAR, .value = mzpack.Application.Description}
            }
        End Using
    End Sub


End Module
