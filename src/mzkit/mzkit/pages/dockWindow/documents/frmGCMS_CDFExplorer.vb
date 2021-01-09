Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS
Imports Microsoft.VisualBasic.Data.IO.netCDF

Public Class frmGCMS_CDFExplorer

    Dim gcms As Raw

    Public Sub loadCDF(file As String)
        gcms = netCDFReader.Open(file).ReadData()
        RtRangeSelector1.SetTIC(gcms.GetTIC.value)
    End Sub
End Class