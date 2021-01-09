Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Imaging

Public Class frmGCMS_CDFExplorer

    Dim gcms As Raw

    Public Sub loadCDF(file As String)
        gcms = netCDFReader.Open(file).ReadData()
        gcms.fileName = file.FileName

        TabText = gcms.fileName

        RtRangeSelector1.SetTIC(gcms.GetTIC.value)
    End Sub

    Private Sub RtRangeSelector1_RangeSelect(min As Double, max As Double) Handles RtRangeSelector1.RangeSelect
        Dim scan As ms1_scan() = gcms.GetMsScan(min, max)

        If scan.Length = 0 OrElse scan.All(Function(x) x.intensity = 0.0) Then
            Return
        End If

        Dim spectrum As ms2() = scan _
            .Select(Function(x)
                        Return New ms2 With {
                            .mz = x.mz,
                            .intensity = x.intensity,
                            .quantity = x.intensity
                        }
                    End Function) _
            .ToArray
        Dim scanData As New LibraryMatrix With {.ms2 = spectrum.Centroid(Tolerance.ParseScript("da:0.3"), LowAbundanceTrimming.Default), .name = "SIM ions"}
        Dim q = scanData.OrderByDescending(Function(x) x.intensity).First
        Dim title1$ = $"Scan Time [{min.ToString("F3")},{max.ToString("F3")}] sec"
        Dim title2$ = $"Quantitative [{q.mz.ToString("F4")}:{q.intensity.ToString("G3")}]"

        PictureBox1.BackgroundImage = scanData.MirrorPlot(titles:={title1, title2}).AsGDIImage
    End Sub

    Private Sub frmGCMS_CDFExplorer_Load(sender As Object, e As EventArgs) Handles Me.Load
        PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
        RtRangeSelector1.BackColor = Color.LightBlue
        RtRangeSelector1.FillColor = Color.Green
        RtRangeSelector1.SelectedColor = Color.Black

        TabText = "Targetted GCMS Viewer"
    End Sub
End Class