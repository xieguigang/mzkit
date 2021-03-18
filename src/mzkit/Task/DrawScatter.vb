Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS

Public Module DrawScatter

    <Extension>
    Public Function DrawScatter(raw As Raw) As Image
        Dim ms1 As New List(Of ms1_scan)

        Using cache As New netCDFReader(raw.ms1_cache)
            For Each scan In raw.scans
                Dim data As CDFData = cache.getDataVariable(cache.getDataVariableEntry(scan.id))
                Dim rawData As ms2() = data.numerics.AsMs2.ToArray

                Call rawData _
                    .Centroid(Tolerance.DeltaMass(0.3), New RelativeIntensityCutoff(0.01)) _
                    .Select(Function(a)
                                Return New ms1_scan With {
                                    .intensity = a.intensity,
                                    .mz = a.mz,
                                    .scan_time = scan.rt
                                }
                            End Function) _
                    .DoCall(AddressOf ms1.AddRange)

                Call Application.DoEvents()
            Next
        End Using

        Return RawScatterPlot.Plot(
            samples:=ms1,
            margin:="padding:200px 600px 300px 300px;",
            rawfile:=raw.source.FileName,
            tickCSS:=CSSFont.Win7VeryLarge,
            legendTitleCSS:=CSSFont.Win7VeryVeryLarge,
            labelFontStyle:=CSSFont.Win7VeryVeryLarge
        ).AsGDIImage
    End Function
End Module
