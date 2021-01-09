Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS
Imports Microsoft.VisualBasic.Data.IO.netCDF

Public Class frmCDFExplorer

    Dim gcms As Raw

    Public Sub loadCDF(file As String)
        gcms = netCDFReader.Open(file).ReadData()
    End Sub
End Class