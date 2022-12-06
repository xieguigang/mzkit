Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.NMRFidTool
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Public Class nmrSpectrumPlot : Inherits Plot

    ReadOnly freq As Spectrum

    Public Sub New(freq As Spectrum, theme As Theme)
        MyBase.New(theme)

        Me.freq = freq
        Me.main = "NMR frequency Plot"
        Me.xlabel = "frequency(Hz)"
        Me.ylabel = "Amplitude"
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim freq As ms2() = Me.freq _
            .RealChannelData _
            .Select(Function(f, i)
                        Return New ms2 With {
                            .mz = f,
                            .intensity = Me.freq.ImaginaryChannelData(i)
                        }
                    End Function) _
            .ToArray
        Dim peaks As New PeakAssign(main, freq, Nothing, 0.25, theme) With {
            .main = main,
            .legendTitle = legendTitle,
            .xlabel = xlabel,
            .ylabel = ylabel,
            .zlabel = zlabel
        }

        Call peaks.Plot(g, canvas)
    End Sub
End Class
