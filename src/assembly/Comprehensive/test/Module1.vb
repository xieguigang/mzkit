Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports Microsoft.VisualBasic.Data.IO.netCDF

Module Module1

    Sub Main()
        Dim demo As String = "F:\20211123_CDF\P210702366.netcdf"
        Dim gcxgc = netCDFReader.Open(demo).ToMzPack

        Pause()
    End Sub
End Module
